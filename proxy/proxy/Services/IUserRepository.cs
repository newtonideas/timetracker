using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll(string token, string name = null);
        Task<IEnumerable<User>> GetAllByProject(string token, string project_id, string name = null);
        Task<User> GetById(string id, string token);
        void Create(User user);
        void Update(User user);
        void Delete(long id);
    }
}