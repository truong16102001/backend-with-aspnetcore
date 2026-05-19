using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.DataAccess.DBContext
{
    public class MyDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { 
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Room>? Rooms { get; set; }
        public DbSet<Hotel>? Hotels { get; set; }
    }
}
