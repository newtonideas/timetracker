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

    }

}