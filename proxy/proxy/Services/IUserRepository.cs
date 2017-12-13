﻿using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace proxy.Services
{
    public interface IUserRepository
    {
        Task<Dictionary<string, string>> Authenticate(string login, string password);
        IEnumerable<User> GetAll();
        User GetById(long id);
        void Create(User user);
        void Update(User user);
        void Delete(long id);
    }
}