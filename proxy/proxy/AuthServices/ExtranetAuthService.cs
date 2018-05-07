using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using proxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace proxy.AuthServices
{
    public class ExtranetAuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ITokenStorage _tokenStorage;

        public ExtranetAuthService(IConfiguration config, ITokenStorage tokenStorage)
        {
            _config = config;
            _tokenStorage = tokenStorage;
        }

        public async Task<Dictionary<string, string>> getAuthCredentials(string token)
        {
            AccessToken accessToken = (AccessToken)await _tokenStorage.SingleOrDefaultAsync(token);
            if (accessToken == null)
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }

            var authCookie = new Dictionary<string, string>();
            authCookie.Add("ASP.NET_SessionId", accessToken.SessionId);
            authCookie.Add("FedAuth", accessToken.FedAuth);
            authCookie.Add("FedAuth1", accessToken.FedAuth1);

            return authCookie;
        }

        public async Task<string> createAuthCredentials(string email, string password)
        {
            var DOMAIN = "https://accounts.newtonideas.com/"; //_config["ExtranetDomain"]; //"https://accounts.newtonideas.com/";
            var URI = "https://accounts.newtonideas.com/Account/Login";
            //var URI = DOMAIN + "Login.aspx";
            var authCookie = new Dictionary<string, string>();

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                var response = await client.GetAsync(URI);

                var antiforgery = response.Headers.GetValues("Set-Cookie").ElementAt(0);
                antiforgery = antiforgery.Substring(0,antiforgery.Length - 42);

                Dictionary<string, string> inputs = parseInputFields(await response.Content.ReadAsStringAsync());

                inputs["Email"] = email;
                inputs["Password"] = password;
                inputs["button"] = "login";
                inputs["RememberLogin"] = "false";
                var formContent = new FormUrlEncodedContent(inputs);
                var response2 = await client.PostAsync(URI, formContent);

                CookieCollection cookies = handler.CookieContainer.GetCookies(new Uri("https://accounts.newtonideas.com/"));

                // Check if valid
                if (cookies["idsrv"] is null)
                    throw new UnauthorizedAccessException("Invalid login/password");

                // Setting cookies
                cookieContainer.Add(new Uri("https://accounts.newtonideas.com"), new Cookie("cmweml", email) );
                cookieContainer.Add(new Uri("https://accounts.newtonideas.com"), cookies);
                cookieContainer.Add(new Uri("https://extranet.newtonideas.com"), new Cookie("XCMWSERV", "default"));
                cookieContainer.Add(new Uri("https://extranet.newtonideas.com"), new Cookie("language_code", "en-US"));

                await client.GetAsync("https://accounts.newtonideas.com/");

                var wsfederationResponse = await client.GetAsync("https://extranet.newtonideas.com/");
                var accessInputs = parseInputFields(wsfederationResponse.Content.ReadAsStringAsync().Result);

                var accessFormContent = new FormUrlEncodedContent(accessInputs);
                var accessResponse = await client.PostAsync("https://extranet.newtonideas.com/", accessFormContent);

                CookieCollection fedAuthCookies = handler.CookieContainer.GetCookies(new Uri("https://extranet.newtonideas.com/"));

                cookieContainer.Add(new Uri("https://extranet.newtonideas.com"), new Cookie("FedAuth", fedAuthCookies["FedAuth"].Value));
                cookieContainer.Add(new Uri("https://extranet.newtonideas.com"), new Cookie("FedAuth1", fedAuthCookies["FedAuth1"].Value));

                await client.GetAsync("https://extranet.newtonideas.com/");
                await client.GetAsync("https://extranet.newtonideas.com/web2.aspx/DB/METAGANTT");

                fedAuthCookies = handler.CookieContainer.GetCookies(new Uri("https://extranet.newtonideas.com/"));

                authCookie.Add("ASP.NET_SessionId", fedAuthCookies["ASP.NET_SessionId"].Value);
                authCookie.Add("FedAuth", fedAuthCookies["FedAuth"].Value);
                authCookie.Add("FedAuth1", fedAuthCookies["FedAuth1"].Value);

                var token = TokenGenerator.GetUniqueToken();
                authCookie.Add("token", token);

                await saveAccessToken(authCookie);
            }

            return authCookie.GetValueOrDefault("token");

        }

        private async Task<AccessToken> saveAccessToken(Dictionary<string,string> authCookie)
        {
            var accessToken = new AccessToken() {
                Token = authCookie.GetValueOrDefault("token"),
                FedAuth = authCookie.GetValueOrDefault("FedAuth"),
                FedAuth1 = authCookie.GetValueOrDefault("FedAuth1"),
                SessionId = authCookie.GetValueOrDefault("ASP.NET_SessionId"),
            };
            _tokenStorage.Add(accessToken);
            await _tokenStorage.SaveChangesAsync();

            return accessToken;
        }

        private Dictionary<string, string> parseInputFields(string html)
        {
            var htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(html);

            var inputs = new Dictionary<string, string>();

            foreach (HtmlNode input in htmlSnippet.DocumentNode.SelectNodes("//input"))
            {
                HtmlAttribute name = input.Attributes["name"];
                HtmlAttribute value = input.Attributes["value"];
                if (name != null)
                    inputs.Add(WebUtility.HtmlDecode(name.Value), value != null ? WebUtility.HtmlDecode(value.Value) : "");
            }

            return inputs;
        }

    }
}
