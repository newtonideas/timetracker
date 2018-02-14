using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using proxy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
            authCookie.Add(".auth", accessToken.Auth);
            authCookie.Add("ASP.NET_SessionId", accessToken.SessionId);

            return authCookie;
        }

        public async Task<string> createAuthCredentials(string login, string password)
        {
            var DOMAIN = _config["ExtranetDomain"];
            var URI = DOMAIN + "Login.aspx";

            var authCookie = new Dictionary<string, string>();

            HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {

                var response1 = await client.GetAsync(URI);
                var response1Body = await response1.Content.ReadAsStringAsync();

                Dictionary<string, string> inputs = parseInputFields(response1Body);
                inputs["ctl03$ctl00$ctl03$login"] = login;
                inputs["ctl03$ctl00$ctl03$password"] = password;

                var formContent = new FormUrlEncodedContent(inputs);
                var response2 = await client.PostAsync(URI, formContent);

                CookieCollection cookies = handler.CookieContainer.GetCookies(new Uri(DOMAIN));

                if (cookies[".auth"] is null)
                    throw new UnauthorizedAccessException("Invalid login/password");

                authCookie.Add("ASP.NET_SessionId", cookies["ASP.NET_SessionId"].Value);
                authCookie.Add(".auth", cookies[".auth"].Value);

                var token = TokenGenerator.GetUniqueToken();
                authCookie.Add("token", token);

                await saveAccessToken(authCookie);
            }

            return authCookie.GetValueOrDefault("token");

        }

        private async Task<AccessToken> saveAccessToken(Dictionary<string,string> authCookie)
        {
            var accessToken = new AccessToken() { Token = authCookie.GetValueOrDefault("token"), Auth = authCookie.GetValueOrDefault(".auth"), SessionId = authCookie.GetValueOrDefault("ASP.NET_SessionId") };
            //_tokenStorage.getStorage().Add(accessToken);
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
                    inputs.Add(name.Value, value != null ? value.Value : "");
            }

            return inputs;
        }

    }
}
