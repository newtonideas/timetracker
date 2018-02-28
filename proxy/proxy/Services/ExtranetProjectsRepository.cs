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

        public async Task<IEnumerable<Project>> GetAll(string token)
        {
            using (var client = new HttpClient())
            {


                List<Project> projects = new List<Project>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/a/WORKSPACE/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();                

                //Serialization
                foreach (JObject t in results)
                {
                    Project p = new Project();
                    p.Id = (string)t["project_id"];
                    p.Name = (string)t["project_title"];
                    p.DateCreated = (DateTime)t["creation_date"];                    
                    projects.Add(p);
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
