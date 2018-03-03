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
        Task<IEnumerable<User>> GetAll(string token);
        Task<IEnumerable<User>> GetAllByProject(string token, string project_id);
        Task<User> GetById(string id, string token);
        void Create(User user);
        void Update(User user);
        void Delete(long id);
    }
}