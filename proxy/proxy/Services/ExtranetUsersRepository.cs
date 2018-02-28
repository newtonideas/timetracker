using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using proxy.Models;
using System.Xml;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using proxy.AuthServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace proxy.Services
{
    public class ExtranetUsersRepository : IUserRepository
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public ExtranetUsersRepository(IAuthService authService, IConfiguration config)
        {
            _config = config;
            _authService = authService;
        }

        public void Create(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll(string token)
        {
            using (var client = new HttpClient())
            {

                List<User> users = new List<User>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/a/WORKSPACE/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JObject t in results)
                {
                    var people_list = t["__people_lists"]["c_participants"].Children().ToList();
                    foreach (JObject u in people_list)
                    {
                        bool userAlreadyAdded = users.Any(item => item.Id == (string)u["id"]);
                        if (!userAlreadyAdded)
                        {
                            User user = new User();
                            user.Id = (string)u["id"];
                            user.Email = (string)u["email"];
                            user.Name = (string)u["first_name"];
                            user.Name += " " + (string)u["last_name"];
                            users.Add(user);
                        }
                    }
                }
                return users;
            }
        }

        public async Task<IEnumerable<User>> GetAllFromProject(string token, string project_id)
        {
            using (var client = new HttpClient())
            {

                List<User> users = new List<User>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/a/WORKSPACE/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JObject t in results)
                {
                    if ((string)t["project_id"] == project_id)
                    {

                        var people_list = t["__people_lists"]["c_participants"].Children().ToList();

                        foreach (JObject u in people_list)
                        {
                            User user = new User();
                            user.Id = (string)u["id"];
                            user.Email = (string)u["email"];
                            user.Name = (string)u["first_name"];
                            user.Name += " " + (string)u["last_name"];
                            users.Add(user);
                        }
                    }
                }
                return users;
            }
        }

        public async Task<User> GetById(string id, string token)
        {
            var allUsers = await GetAll(token);
            foreach (User u in allUsers)
            {
                if (u.Id == id)
                {
                    return u;
                }
            }
            return null;
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}