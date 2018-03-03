using System;
using System.Collections.Generic;
using System.Linq;
using proxy.Models;

namespace proxy.Services
{
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> GetAll(string token, string project_id);
        System.Threading.Tasks.Task<Task> GetById(string id, string token, string project_id);
        System.Threading.Tasks.Task<Task> Create(string token, Task task, string project_id, string created_by_id, string responsible_user_id);
        System.Threading.Tasks.Task<Task> Update(string token, string id, Task task, string project_id, string created_by_id, string responsible_user_id);
        System.Threading.Tasks.Task<string> Delete(string token, string id);
    }
}