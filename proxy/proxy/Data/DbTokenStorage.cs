using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proxy.Data
{
    public class DbTokenStorage : ITokenStorage
    {
        private readonly ApplicationDbContext _db;

        public DbTokenStorage(ApplicationDbContext db)
        {
            _db = db;
        }

        public void Add(object obj)
        {
            _db.Add(obj);
        }

        public async Task<int> SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
            return 1;
        }

        public async Task<object> SingleOrDefaultAsync(string id)
        {
            return await _db.AccessTokens.SingleOrDefaultAsync(m => m.Token == id);
        }
    }
}
