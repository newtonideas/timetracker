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
using System.Threading;
using System.Globalization;

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

        public async Task<Timelog> Create(string token, Timelog timelog, string project_id)
        {

            Timelog newTimelog = timelog;
            if (newTimelog.Project_id != project_id)
            {
                newTimelog.Project_id = project_id;
            }


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
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                inputs["start_on"] = newTimelog.Start_on.ToString();
                inputs["finish_on"] = newTimelog.Finish_on.ToString();

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired");
                }
                if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception("Invalid timelog input data");
                }
                //Getting id of created Timelog
                var stringResult = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(stringResult);
                var responseJson = JObject.Parse(jsonArray[0].ToString());
                var id = responseJson["id"];

                newTimelog.Id = id.ToString();
            }

            return newTimelog;
         }

        public async Task<bool> Delete(string token, string id)
        {
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

                
                Dictionary<string, string> inputs = new Dictionary<string, string>();
                inputs["process_template_id"] = "default-app-timelog";
                inputs["id"] = id;
                inputs["transition"] = "delete";

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Invalid id");
                }

                var stringResult = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(stringResult);
                var responseJson = JObject.Parse(jsonArray[0].ToString());
                var deletedJson = responseJson["deleted"];
                var deletedString = deletedJson.ToString();
                
                return deletedString.Equals("True");
                
            }
        }

        public async Task<IEnumerable<Timelog>> GetAll(string token, string project_id) {
            using (var client = new HttpClient()) {


                List<Timelog> timelogs = new List<Timelog>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //get current user
                var responseForCurrentUser = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/projects/!BLA", authCookies, _config).Result;
                var jsonForCurrentUser = JObject.Parse(responseForCurrentUser);
                var currentUserId = jsonForCurrentUser["current_user_id"].ToString();

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/w/TIMELOG/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;

                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JObject t in results) {

                    var projectId = (string)t["project_id"];
                    var userId = (string)t["accountable_account_id"];

                    if (projectId.Equals(project_id) && userId.Equals(currentUserId))
                    {

                        Timelog timelog = new Timelog();
                        timelog.Id = (string)t["id"];
                        timelog.Title = (string)t["title"];
                        timelog.User_id = userId;
                        timelog.Project_id = projectId;
                        timelog.Task_id = (string)t["parent_id"];

                        try
                        {
                            timelog.Start_on = (DateTime)t["start_on"];
                        }
                        catch (ArgumentException) { }
                        try
                        {
                            timelog.Finish_on = (DateTime)t["finish_on"];
                        }
                        catch (ArgumentException) { }
                        timelog.Effort = (timelog.Finish_on - timelog.Start_on).TotalHours;
                        if(timelog.Task_id != null)
                        {
                            timelogs.Add(timelog);
                        }
                    }
                }
                    
                return timelogs;
            }
        }

        public async Task<Timelog> GetById(string id, string token, string project_id)
        {
            var allTimelogs = await GetAll(token, project_id);
            foreach (Timelog t in allTimelogs) {
                if (t.Id == id) {
                    return t;
                }
            }
            return null;
        }

        public async Task<Timelog> Update(string token, string id, Timelog timelog, string project_id)
        {
            Timelog editedTimelog = timelog;
            if (editedTimelog.Project_id != project_id)
            {
                editedTimelog.Project_id = project_id;
            }

            if (editedTimelog.Id != id)
            {
                editedTimelog.Id = id;
            }


            var DOMAIN = _config["ExtranetDomain"];
            var URI = DOMAIN + "/api/ApiAlpha.ashx/tickets/multi?check_conflict=1&layout_media=ui-form";


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

                //adding properties for editing timelog
                Dictionary<string, string> inputs = new Dictionary<string, string>();
                inputs["id"] = editedTimelog.Id;
                inputs["accountable_account_id"] = editedTimelog.User_id;
                inputs["project_id"] = editedTimelog.Project_id;
                inputs["parent_id"] = editedTimelog.Task_id;
                inputs["process_template_id"] = "default-app-timelog";
                inputs["title"] = editedTimelog.Title;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                inputs["start_on"] = editedTimelog.Start_on.ToString();
                inputs["finish_on"] = editedTimelog.Finish_on.ToString();
                inputs["transition"] = "edit";

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new Exception("Invalid timelog input data");
                }
            }

            return editedTimelog;
        }
    }
}
