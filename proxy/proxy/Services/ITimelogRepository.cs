using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface ITimelogRepository
    {
        Task<IEnumerable<Timelog>> GetAll(string token, string project_id);
        Task<Timelog> GetById(string id, string token, string project_id);
        Task<Timelog> Create(string token, Timelog timelog, string project_id);
        Task<Timelog> Update(string token, string id, Timelog timelog, string project_id);
        Task<string> Delete(string token, string id);
    }
}
