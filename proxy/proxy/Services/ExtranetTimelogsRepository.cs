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

        public void Create(Timelog timelog)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Timelog>> GetAll(string token) {
            using (var client = new HttpClient()) {


                List<Timelog> timelogs = new List<Timelog>();

                Dictionary<string, string> authCookies = await _authService.getAuthCredentials(token);

                //Retrieving a JSON-Object
                var response = RequestGenerator.generateRequest("/api/ApiAlpha.ashx/w/TTI/a/TIMELOG/tickets/list?&listOfFields=ALL&withTechnicalData=true", authCookies, _config).Result;

                var json = JObject.Parse(response);
                var results = json["data"].Children().ToList();

                //Serialization
                foreach (JToken t in results) {
                    Timelog timelog = t.ToObject<Timelog>();
                    timelogs.Add(timelog);
                }
                    
                return timelogs;
            }
        }

        public async Task<Timelog> GetById(string id, string token)
        {
            var allTimelogs = await GetAll(token);
            foreach (Timelog t in allTimelogs) {
                if (t.Id == id) {
                    return t;
                }
            }
            return null;
        }

        public void Update(Timelog timelog)
        {
            throw new NotImplementedException();
        }
    }
}
