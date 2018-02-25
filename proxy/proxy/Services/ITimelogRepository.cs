using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface ITimelogRepository
    {
        Task<IEnumerable<Timelog>> GetAll(string token);
        Task<Timelog> GetById(string id, string token);
        Task<Timelog> Create(string token, Timelog timelog);
        void Update(Timelog timelog);
        void Delete(string id);
    }
}
