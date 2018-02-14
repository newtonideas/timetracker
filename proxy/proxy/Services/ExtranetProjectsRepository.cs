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
                var response = generateRequest("/api/ApiAlpha.ashx/w/TTI/a/WORKSPACE/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies).Result;

                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JToken t in results)
                {
                    Project project = t.ToObject<Project>();
                    projects.Add(project);
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

        public async Task<string> generateRequest(string URI, Dictionary<string, string> authCookies)
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
