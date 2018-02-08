using Microsoft.AspNetCore.Authorization;
using proxy.Data;
using proxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Validators
{
    public class TokenValidation
    {
        private readonly ApplicationDbContext _context;

        public TokenValidation(ApplicationDbContext context)
        {
            _context = context;
        }

        public AccessToken checkToken(string token)
        {
            return _context.AccessTokens.Find(token);
        }

        
    }
}
