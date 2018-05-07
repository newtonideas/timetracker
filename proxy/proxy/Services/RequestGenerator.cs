using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace proxy.Services
{
    public static class RequestGenerator
    {

        public static async Task<string> generateRequest(string URI, Dictionary<string, string> authCookies, IConfiguration config)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(config["ExtranetDomain"]);
                client.DefaultRequestHeaders.Accept.Clear();

                //Setting Cookie
                string cookie = "XCMWSERV = default; require_ssl=true; language_code=en-US;"
                    + "FedAuth=" + authCookies["FedAuth"] + "; FedAuth1=" + authCookies["FedAuth1"]
                    + "; ASP.NET_SessionId=" + authCookies["ASP.NET_SessionId"] + ";";

                /*foreach (var field in authCookies) {
                    cookie += field.Key + '=' + field.Value + ';';
                }*/
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Getting Response
                var response = await client.GetAsync(URI);
                response.EnsureSuccessStatusCode();
                var stringResult = await response.Content.ReadAsStringAsync();
                return stringResult;
            }
        }

    }
}
