using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using proxy.Services;
using proxy.Models;
using proxy.AuthServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proxy.Controllers
{
    [Route("api/[controller]")]
    public class TimelogsController : Controller
    {
        private readonly ITimelogRepository _timelogRepository;
        private readonly IAuthService _authService;

        public TimelogsController(ITimelogRepository timelogRepository, IAuthService authService)
        {
            _authService = authService;
            _timelogRepository = timelogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader] string token)
        {
            try
            {
                return new ObjectResult (await this._timelogRepository.GetAll(token));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToAction(nameof(Authenticate));
            }
        }

        [HttpGet("{id}", Name = "GetTimelog")]
        public async Task<IActionResult> GetById(string id, [FromHeader] string token)
        {
            Timelog timelog = await this._timelogRepository.GetById(id, token);
            if (timelog == null)
            {
                return NotFound();
            }
            return new ObjectResult(timelog);
        }

        [HttpPost]
        public IActionResult Create([FromBody]Timelog timelog)
        {
            return Ok(timelog);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Timelog timelog)
        {
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return new NoContentResult();
        }

        [HttpPost("[action]")]
        public async Task<string> Authenticate(string login, string password)
        {
            return await _authService.createAuthCredentials(login, password);
        }
    }
}
