using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAll(string token, string name=null);
        Task<Project> GetById(string id, string token);
        void Create(Project project);
        void Update(Project project);
        void Delete(string id);
    }
}
