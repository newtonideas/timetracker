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
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/projects/!BLA", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var workspaceUsers = json["workspace_users"].Children().ToList();

                //Serialization
                foreach (var u in workspaceUsers)
                {
                    var user = new User();
                    user.Id = (string)u["id"];
                    user.Name = (string)u["name"];
                    users.Add(user);
                }
                return users;
            }
        }

        public async Task<IEnumerable<User>> GetAllByProject(string token, string project_id)
        {
            using (var client = new HttpClient())
            {

                List<User> users = new List<User>();
                var project_alias = (await (new ExtranetProjectsRepository(_authService, _config)).GetById(project_id, token)).Alias;

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/projects/" + project_alias + "!BLA", authCookies, _config).Result;
                var json = JObject.Parse(response);
                var workspaceUsers = json["workspace_users"].Children().ToList();
                var company = (string)json["workspace_details"]["company_title"];

                //Serialization
                foreach (var u in workspaceUsers) {
                    var user = new User();
                    user.Id = (string)u["id"];
                    user.Name = (string)u["name"];
                    //user.Email = "";// (string)u["email"];
                    //user.Password = "";
                    user.Company = company;
                    //user.Phone = "";
                    users.Add(user);
                }
                return users;
            }
        }

        public async Task<User> GetById(string id, string token)
        {
            var allUsers = await GetAll(token);
            foreach (var u in allUsers)
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