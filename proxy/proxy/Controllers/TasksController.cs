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
    [Route("api/[controller]")]
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
        public async System.Threading.Tasks.Task<IActionResult> GetAll([FromHeader] string token)
        {
            try
            {
                if(token == null) return RedirectToRoute(new {controller = "Users", action="AccessToken"});
                return new ObjectResult (await this._taskRepository.GetAll(token));
            }
            catch (UnauthorizedAccessException e){
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpGet("{id}", Name = "GetTask")]
        public async System.Threading.Tasks.Task<IActionResult> GetById(string id, [FromHeader] string token)
        {
            Task task = await this._taskRepository.GetById(id, token);
            if(task == null)
            {
                return NotFound();
            }
            return new ObjectResult(task);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Task task)
        {
            return Ok(task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Task task)
        {
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return new NoContentResult();
        }

    }
}