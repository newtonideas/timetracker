﻿using System;
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
    [Route("api/projects/{project_id}/[controller]")]
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
        public async Task<IActionResult> GetAll([FromHeader] string token, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult (await this._timelogRepository.GetAll(token, project_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpGet("{id}", Name = "GetTimelog")]
        public async Task<IActionResult> GetById(string id, [FromHeader] string token, [FromRoute]string project_id)
        {
            Timelog timelog = await this._timelogRepository.GetById(id, token, project_id);
            if (timelog == null)
            {
                return NotFound();
            }
            return new ObjectResult(timelog);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromHeader]string token, [FromBody]Timelog timelog, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._timelogRepository.Create(token, timelog, project_id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody]Timelog timelog)
        {
            return new NoContentResult();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader]string token, string id, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._timelogRepository.Delete(token, id));
            }
            catch (UnauthorizedAccessException e)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }
    }
}
