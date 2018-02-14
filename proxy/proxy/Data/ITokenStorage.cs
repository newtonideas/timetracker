using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Data
{
    public interface ITokenStorage
    {
        void Add(object obj);
    
        Task<int> SaveChangesAsync();

        Task<object> SingleOrDefaultAsync(string id);
    }
}
