using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CapCoStarter.Api.Data
{
    public class AppDataDbContext : DbContext
    {
        public AppDataDbContext (DbContextOptions<AppDataDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestDataEntity> TestDataEntity { get; set; }
    }
}
