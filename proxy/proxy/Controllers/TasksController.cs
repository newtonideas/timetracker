using System;
using System.Collections.Generic;
using System.Linq;
using proxy.Models;
using Microsoft.AspNetCore.Mvc;
using proxy.Services;
using proxy.AuthServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proxy.Controllers
{
    [Route("api/projects/{project_id}/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IAuthService _authService;

        public TasksController(ITaskRepository taskRepository, IAuthService authService)
        {
            _authService = authService;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetAll([FromHeader] string token, [FromRoute] string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new {controller = "Users", action="AccessToken"});
                return new ObjectResult (await this._taskRepository.GetAll(token, project_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpGet("{id}", Name = "GetTask")]
        public async System.Threading.Tasks.Task<IActionResult> GetById(string id, [FromHeader] string token, [FromRoute]string project_id)
        {
            Task task = await this._taskRepository.GetById(id, token, project_id);
            if(task == null)
            {
                return NotFound();
            }
            return new ObjectResult(task);
        }

        [HttpGet("[action]")]
        public async System.Threading.Tasks.Task<IActionResult> createTest([FromRoute] string project_id)
        {
            string token = "cgqJ48yzIFtW7G8";
            Task t = new Task();
            t.Id = "null";
            t.Name = "It works!";
            t.Description = "babababa";
            t.Priority = "normal";
            t.Status = "submitted";

            return new ObjectResult(await Create(token, t, project_id, "e981a503-1536-4a48-920d-6c464f596cbc", "dad1e3d4-d72f-4446-a6f5-cbaefd5fe283"));
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create([FromHeader]string token, [FromBody]Task task, [FromRoute]string project_id, string create_by_id, string responsible_user_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._taskRepository.Create(token, task, project_id, create_by_id, responsible_user_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Task task)
        {
            return new NoContentResult();
        }


        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> Delete([FromHeader]string token, string id, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._taskRepository.Delete(token, id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

    }
}