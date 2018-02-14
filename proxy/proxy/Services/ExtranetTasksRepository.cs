using System;
using System.Collections.Generic;
using System.Linq;
using proxy.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using proxy.AuthServices;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace proxy.Services {
    public class ExtranetTasksRepository : ITaskRepository {

        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public ExtranetTasksRepository(IAuthService authService, IConfiguration config)
        {
            _config = config;
            _authService = authService;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetAll(string token)
        {
            using (var client = new HttpClient())
            {

                List<Task> tasks = new List<Task>();
                
                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = generateRequest("/api/ApiAlpha.ashx/w/TTI/a/TASK/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies).Result;

                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JToken t in results)
                {
                    Task task = t.ToObject<Task>();
                    tasks.Add(task);
                }

                return tasks;
            }
        }

        public async System.Threading.Tasks.Task<Task> GetById(string id, string token) {
            var allTasks = await GetAll(token);
            foreach (Task t in allTasks) {
                if (t.Id == id) {
                    return t;
                }  
            }
            return null;
        }
        public void Create(Task task)
        {
            throw new NotImplementedException();
        }
        public void Update(Task task)
        {
            throw new NotImplementedException();
        }
        public void Delete(string id) {
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