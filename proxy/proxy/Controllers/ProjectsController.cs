﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using proxy.Models;
using proxy.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proxy.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public IEnumerable<Project> GetAll()
        {
            return this._projectRepository.GetAll();
        }

        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetById(string id)
        {
            return new ObjectResult(this._projectRepository.GetById(id));
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
