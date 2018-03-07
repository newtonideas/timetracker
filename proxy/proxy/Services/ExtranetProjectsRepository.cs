using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using proxy.Models;
using System.Web;
using proxy.AuthServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace proxy.Services
{
    public class ExtranetProjectsRepository : IProjectRepository
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public ExtranetProjectsRepository(IAuthService authService, IConfiguration config)
        {
            _config = config;
            _authService = authService;
        }

        public async Task<IEnumerable<Project>> GetAll(string token, string name = null)
        {
            using (var client = new HttpClient())
            {

                var projects = new List<Project>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/a/WORKSPACE/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();                

                //Serialization
                foreach (JObject t in results)
                {
                    Project project = new Project();
                    project.Id = (string)t["project_id"];
                    project.Name = (string)t["project_title"];
                    project.TimeCreated = (DateTime)t["creation_date"];
                    project.Alias = (string)t["project_alias"];
                    projects.Add(project);
                }
                if (!(string.IsNullOrEmpty(name))) {
                    var filtered = new List<Project>();

                    foreach (var p in projects) {
                        if (p.Name.Contains(name) || p.Alias.Contains(name)) {
                            filtered.Add(p);
                        }
                    }

                    projects = filtered;
                }

                return projects;
            }
        }
        public async Task<Project> GetById(string id, string token)
        {
            var allProjects = await GetAll(token);
            foreach (Project t in allProjects)
            {
                if (t.Id == id)
                {
                    return t;
                }
            }
            return null;
        }
        public void Create(Project project)
        {
            throw new NotImplementedException();
        }
        public void Update(Project project)
        {
            throw new NotImplementedException();
        }
        public void Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
