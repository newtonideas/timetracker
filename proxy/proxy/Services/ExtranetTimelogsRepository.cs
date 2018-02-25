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
using Newtonsoft.Json;
using System.Text;

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

        public async Task<Timelog> Create(string token, Timelog timelog)
        {
            Timelog newTimelog = timelog;


            var DOMAIN = _config["ExtranetDomain"];
            var URI = DOMAIN + "api/ApiAlpha.ashx/tickets/multi?check_conflict=1&layout_media=ui-form";


            HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                //getting authentication cookies
                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                client.BaseAddress = new Uri(DOMAIN);
                client.DefaultRequestHeaders.Accept.Clear();

                //adding cookies to request
                string cookie = "XCMWSERV = default; require_ssl=true; language_code=en-US;";
                cookie += "; ASP.NET_SessionId=" + authCookies["ASP.NET_SessionId"] + "; .auth=" + authCookies[".auth"];
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //adding properties for new timelog
                Dictionary<string, string> inputs = new Dictionary<string, string>();
                inputs["accountable_account_id"] = newTimelog.User_id;
                inputs["project_id"] = newTimelog.Project_id;
                inputs["parent_id"] = newTimelog.Task_id;
                inputs["process_template_id"] = "default-app-timelog";
                inputs["title"] = newTimelog.Title;
                inputs["start_on"] = newTimelog.Start_on.ToString();
                inputs["finish_on"] = newTimelog.Finish_on.ToString();

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);

                //Getting id of created Timelog
                var stringResult = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(stringResult);
                var responseJson = JObject.Parse(jsonArray[0].ToString());
                var id = responseJson["id"];

                newTimelog.Id = id.ToString();
            }

            return newTimelog;
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
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/w/TTI/a/TIMELOG/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;

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
    }
}
