using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using proxy.Services;
using proxy.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using proxy.AuthServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proxy.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UsersController(IUserRepository userRepository, IAuthService authService)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpGet("[action]")]
        public async System.Threading.Tasks.Task<string> AccessToken(string login, string password)
        {
            return await AccessTokenPost(login, password);
        }

        [HttpPost("[action]")]
        [ActionName("AccessToken")]
        public async System.Threading.Tasks.Task<string> AccessTokenPost(string login, string password)
        {
            return await _authService.createAuthCredentials(login, password);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] string token)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._userRepository.GetAll(token));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
            
        }

        
        [HttpGet("projects/{project_id}/users")]
        public async Task<IActionResult> GetAllFromProject([FromHeader] string token, string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._userRepository.GetAllFromProject(token, project_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }

        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetById(string id, [FromHeader]string token)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._userRepository.GetById(id, token));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]User user)
        {
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]User user)
        {
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new NoContentResult();
        }
    }
}