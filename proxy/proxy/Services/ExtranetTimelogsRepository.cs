using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using proxy.Models;
using Newtonsoft.Json.Linq;
using proxy.AuthServices;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace proxy.Services {
    public class ExtranetTimelogsRepository : ITimelogRepository
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public ExtranetTimelogsRepository(IAuthService authService, IConfiguration config)
        {
            _config = config;
            _authService = authService;
        }

        public void Create(Timelog timelog)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Timelog>> GetAll(string token) {
            using (var client = new HttpClient()) {


                List<Timelog> timelogs = new List<Timelog>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = generateRequest("/api/ApiAlpha.ashx/w/TTI/a/TIMELOG/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies).Result;

                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JToken t in results) {
                    Timelog timelog = t.ToObject<Timelog>();
                    timelogs.Add(timelog);
                }
                    
                return timelogs;
            }
        }

        public async Task<Timelog> GetById(string id, string token)
        {
            var allTimelogs = await GetAll(token);
            foreach (Timelog t in allTimelogs) {
                if (t.Id == id) {
                    return t;
                }
            }
            return null;
        }

        public void Update(Timelog timelog)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task<string> generateRequest(string URI, Dictionary<string, string> authCookies)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(_config["ExtranetDomain"]);
                client.DefaultRequestHeaders.Accept.Clear();

                //Setting Cookie
                string cookie = "XCMWSERV = default; require_ssl=true; language_code=en-US;";
                cookie += "; ASP.NET_SessionId=" + authCookies["ASP.NET_SessionId"] + "; .auth=" + authCookies[".auth"];
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
