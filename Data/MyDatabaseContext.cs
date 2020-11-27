using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreSqlDb.Models
{
    public class MyDatabaseContext : IdentityDbContext
    {
        public MyDatabaseContext (DbContextOptions<MyDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<DotNetCoreSqlDb.Models.Todo> Todo { get; set; }
        public DbSet<DotNetCoreSqlDb.Models.Inventory> Inventory { get; set; }

        public DbSet<DotNetCoreSqlDb.Models.Order> Order { get; set; }
    }
}
