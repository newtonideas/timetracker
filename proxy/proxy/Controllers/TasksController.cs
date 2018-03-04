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

        [Route("~/api/tasks")]
        public async System.Threading.Tasks.Task<IActionResult> GetAll([FromHeader] string token) {
            try {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._taskRepository.GetAll(token));
            }
            catch (UnauthorizedAccessException e) {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetAllByProject([FromHeader] string token, [FromRoute] string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new {controller = "Users", action="AccessToken"});
                return new ObjectResult (await this._taskRepository.GetAllByProject(token, project_id));
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
        public async System.Threading.Tasks.Task<IActionResult> Create([FromHeader]string token, [FromBody]Task task, [FromRoute]string project_id, string created_by_id, string responsible_user_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._taskRepository.Create(token, task, project_id, created_by_id, responsible_user_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }


        [HttpGet("[action]")]
        public async System.Threading.Tasks.Task<IActionResult> updateTest([FromRoute] string project_id)
        {
            string token = "cgqJ48yzIFtW7G8";
            Task t = new Task();
            t.Id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            t.Name = "Edited! It works!!!!!!!!!!!!";
            t.Description = "hop hey";
            t.Priority = "normal";
            t.Status = "submitted";
            var id = "49d2dc7f-e9df-4b10-ac82-5c8cc0220ee1";
            return new ObjectResult(await Update(token, id, t, project_id, "e981a503-1536-4a48-920d-6c464f596cbc", "e981a503-1536-4a48-920d-6c464f596cbc"));
        }

        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> Update([FromHeader] string token, string id, [FromBody]Task task, [FromRoute]string project_id, string created_by_id, string responsible_user_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._taskRepository.Update(token, id, task, project_id, created_by_id, responsible_user_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
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