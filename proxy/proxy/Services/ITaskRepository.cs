using System;
using System.Collections.Generic;
using System.Linq;
using proxy.Models;

namespace proxy.Services
{
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> GetAll(string token);
        System.Threading.Tasks.Task<Task> GetById(string id, string token);
        void Create(Task task);
        void Update(Task task);
        void Delete(string id);
    }
}