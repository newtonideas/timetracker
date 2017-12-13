﻿using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAll();
        Project GetById(string id);
        void Create(Project project);
        void Update(Project project);
        void Delete(string id);
    }
}
