using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using proxy.AuthServices;
using proxy.Models;
using proxy.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proxy.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthService _authService;

        public ProjectsController(IProjectRepository projectRepository, IAuthService authService)
        {
            _authService = authService;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] string token)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._projectRepository.GetAll(token));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpGet("{id}", Name = "GetProject")]
        public async Task<IActionResult> GetById(string id, [FromHeader] string token)
        {
            Project project = await this._projectRepository.GetById(id, token);
            if (project == null)
            {
                return NotFound();
            }
            return new ObjectResult(project);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Project project)
        {
            return Ok(project);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Project project)
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
