using System;
using System.Collections.Generic;
using System.Linq;
using proxy.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using proxy.AuthServices;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace proxy.Services {
    public class ExtranetTasksRepository : ITaskRepository {

        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public ExtranetTasksRepository(IAuthService authService, IConfiguration config)
        {
            _config = config;
            _authService = authService;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetAll(string token, string project_id)
        {
            using (var client = new HttpClient())
            {

                List<Task> tasks = new List<Task>();
                
                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx//pid/default-transaction-process-template/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JObject t in results)
                {
                    var projectId = (string)t["project_id"];

                    if (projectId == project_id) {

                        Task task = new Task();
                        task.Id = (string)t["id"];
                        task.ProjectId = projectId;
                        task.Name = (string)t["title"];
                        task.Description = "";
                        task.Status = (string)t["state"];
                        task.Priority = (string)t["priority"];
                        task.TimeCreated = (DateTime)t["creation_date"];
                        tasks.Add(task);

                    }
                }

                return tasks;
            }
        }

        public async System.Threading.Tasks.Task<Task> GetById(string id, string token, string project_id) {
            var allTasks = await GetAll(token, project_id);
            foreach (Task t in allTasks) {
                if (t.Id == id) {
                    return t;
                }  
            }
            return null;
        }
        public async System.Threading.Tasks.Task<Task> Create(string token, Task task, string project_id, string create_by_id, string responsible_user_id)
        {
            Task newTask = task;
            if (newTask.ProjectId != project_id)
            {
                newTask.ProjectId = project_id;
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

                //adding properties for new task
                Dictionary<string, string> inputs = new Dictionary<string, string>();
                inputs["accountable_account_id"] = create_by_id;
                inputs["keeper_id"] = responsible_user_id;
                inputs["project_id"] = newTask.ProjectId;
                inputs["title"] = newTask.Name;
                inputs["description"] = newTask.Description;
                inputs["process_template_id"] = "default-transaction-process-template";
                inputs["state"] = newTask.Status;
                inputs["priority"] = newTask.Priority;

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);

                //Getting id of created Task
                var stringResult = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(stringResult);
                var responseJson = JObject.Parse(jsonArray[0].ToString());
                var id = responseJson["id"];

                newTask.Id = id.ToString();
            }

            return newTask;
        }
        public void Update(Task task)
        {
            throw new NotImplementedException();
        }
        public async System.Threading.Tasks.Task<string> Delete(string token, string id)
        {
            var DOMAIN = _config["ExtranetDomain"];
            var URI = DOMAIN + "/api/ApiAlpha.ashx/tickets/multi?check_conflict=1&layout_media=ui-form";

            var stringResult = "";

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
                inputs["process_template_id"] = "default-transaction-process-template";
                inputs["id"] = id;
                inputs["transition"] = "delete";

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);

                stringResult = await response.Content.ReadAsStringAsync();
            }
            return stringResult;
        }

    }

}