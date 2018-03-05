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

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAll(string token) {
            using (var client = new HttpClient()) {

                List<Task> tasks = new List<Task>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving an array of JSON-objects
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx//pid/default-transaction-process-template/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (var t in results) {
                    Task task = new Task();
                    task.Id = (string)t["id"];
                    task.ProjectId = (string)t["project_id"];
                    task.Name = (string)t["title"];
                    task.Status = (string)t["state"];
                    task.Priority = (string)t["priority"];
                    task.TimeCreated = (DateTime)t["creation_date"];
                    //var descrAddress = "/api/ApiAlpha.ashx/w/" + (string)t["project_alias"] + "/a/TASK/tickets/list?expandRecord=true&layout_media=ui-form&listOfFields=ALL&rlx=id%3D%22" + task.Id + "%22&skipAncestors=true"; //var descrResponse = RequestGenerator.generateRequest(descrAddress, authCookies, _config).Result;
                    task.Description = ""; // (string)((JArray.Parse(descrResponse))[0]["description"]);
                    tasks.Add(task);
                }

                return tasks;
            }
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetAllByProject(string token, string project_id)
        {
            using (var client = new HttpClient())
            {
                List<Task> tasks = new List<Task>();
                var project_alias = (await (new ExtranetProjectsRepository(_authService, _config)).GetById(project_id, token)).Alias;

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving an array of JSON-objects
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/w/" + project_alias + "/a/TASK/tickets/list?expandRecord=true&layout_media=ui-form&listOfFields=ALL", authCookies, _config).Result;
                var json = JArray.Parse(response);

                //Serialization
                foreach (var t in json) {
                    Task task = new Task();
                    task.Id = (string)t["id"];
                    task.ProjectId = (string)t["project_id"];
                    task.Name = (string)t["title"];
                    task.Description = (string)t["description"];
                    task.Status = (string)t["state"];
                    task.Priority = (string)t["priority"];
                    task.TimeCreated = (DateTime)t["creation_date"];
                    tasks.Add(task);
                }

                /*
                var allTasks = await GetAll(token);
                foreach (Task t in allTasks) {
                    if (t.ProjectId == project_id) {
                        tasks.Add(t);
                    }
                }*/

                return tasks;
            }
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllByPeriod(string token, string from, string till) {
            using (var client = new HttpClient()) {
                var allTasks = await GetAll(token);
                return RefineByPeriod(allTasks, from, till);
            }
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetAllByProjectByPeriod(string token, string project_id, string from, string till) {
            using (var client = new HttpClient()) {
                var allTasks = await GetAllByProject(token, project_id);
                return RefineByPeriod(allTasks, from, till);
            }
        }

        private IEnumerable<Models.Task> RefineByPeriod(IEnumerable<Models.Task> allTasks, string from, string till) {
            using (var client = new HttpClient()) {
                List<Task> tasks = new List<Task>();

                int fromYear = 0; int fromMonth = 0; int fromDay = 0;
                int tillYear = 0; int tillMonth = 0; int tillDay = 0;
                Int32.TryParse(from.Substring(0, 4), out fromYear);
                Int32.TryParse(from.Substring(5, 2), out fromMonth);
                Int32.TryParse(from.Substring(8, 2), out fromDay);
                Int32.TryParse(till.Substring(0, 4), out tillYear);
                Int32.TryParse(till.Substring(5, 2), out tillMonth);
                Int32.TryParse(till.Substring(8, 2), out tillDay);

                var fromDate = new DateTime(fromYear, fromMonth, fromDay, 0, 0, 0);
                var tillDate = new DateTime(tillYear, tillMonth, tillDay, 23, 59, 59);

                foreach (var t in allTasks) {
                    if (DateTime.Compare(t.TimeCreated, fromDate) >= 0
                        && DateTime.Compare(t.TimeCreated, tillDate) <= 0) {
                        tasks.Add(t);
                    }
                }

                return tasks;
            }
        }

        public async System.Threading.Tasks.Task<Task> GetById(string id, string token, string project_id) {
            var allTasks = await GetAllByProject(token, project_id);
            foreach (Task t in allTasks) {
                if (t.Id == id) {
                    return t;
                }  
            }
            return null;
        }

        public async System.Threading.Tasks.Task<Task> Create(string token, Task task, string project_id, string created_by_id, string responsible_user_id)
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
                inputs["accountable_account_id"] = created_by_id;
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

        public async System.Threading.Tasks.Task<Task> Update(string token, string id, Task task, string project_id, string created_by_id, string responsible_user_id)
        {
            Task editedTask = task;
            if (editedTask.ProjectId != project_id)
            {
                editedTask.ProjectId = project_id;
            }

            if (editedTask.Id != id)
            {
                editedTask.Id = id;
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

                //adding properties for editing task
                Dictionary<string, string> inputs = new Dictionary<string, string>();
                inputs["id"] = editedTask.Id;
                inputs["accountable_account_id"] = created_by_id;
                inputs["keeper_id"] = responsible_user_id;
                inputs["project_id"] = editedTask.ProjectId;
                inputs["title"] = editedTask.Name;
                inputs["description"] = editedTask.Description;
                inputs["process_template_id"] = "default-transaction-process-template";
                inputs["transition"] = "edit";
                inputs["state"] = editedTask.Status;
                inputs["priority"] = editedTask.Priority;

                var requestJson = "[" + JsonConvert.SerializeObject(inputs) + "]";
                var stringContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // making request
                var response = await client.PostAsync(URI, stringContent);

            }

            return editedTask;
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