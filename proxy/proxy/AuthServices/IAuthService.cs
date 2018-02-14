using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.AuthServices
{
    public interface IAuthService
    {
        Task<string> createAuthCredentials(string login, string password);
        Task<Dictionary<string,string>> getAuthCredentials(string token);
    }
}
