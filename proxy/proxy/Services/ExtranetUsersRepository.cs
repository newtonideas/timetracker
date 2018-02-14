using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using proxy.Models;
using System.Xml;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace proxy.Services
{
    public class ExtranetUsersRepository : IUserRepository
    {

        public void Create(User task)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Update(User task)
        {
            throw new NotImplementedException();
        }
    }
}