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
            catch (UnauthorizedAccessException)
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
            catch (UnauthorizedAccessException)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> updateTest([FromRoute] string project_id)
        {
            string token = "cgqJ48yzIFtW7G8";
            var id = "8cb3d17f-553a-41da-a902-49e5601cb57d";
            Timelog t = new Timelog();
            t.Id = "8cb3d17f-553a-41da-a902-49e5601cb57d";
            t.Project_id = "87b47424-1c46-473b-81c1-a1b52123b7ce";
            t.Task_id = "49d2dc7f - e9df - 4b10 - ac82 - 5c8cc0220ee1";
            t.User_id = "e981a503-1536-4a48-920d-6c464f596cbc";
            t.Title = "eee rock";
            DateTime startDate = new DateTime(2018, 3, 3, 10, 30, 16);
            DateTime endDate = new DateTime(2018, 3, 3, 10, 45, 16);
            t.Start_on = startDate;
            t.Finish_on = endDate;
            return new ObjectResult(await Update(token, id, t, project_id));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string token, string id, [FromBody]Timelog timelog, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._timelogRepository.Update(token, id, timelog, project_id));
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromHeader]string token, string id, [FromRoute]string project_id)
        {
            try
            {
                if (token == null) return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
                return new ObjectResult(await this._timelogRepository.Delete(token, id));
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToRoute(new { controller = "Users", action = "AccessToken" });
            }
        }
    }
}
