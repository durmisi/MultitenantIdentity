using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantsWeb.Domain;

namespace TenantsWeb.Data
{
    public class TenantsDbContext : DbContext
    {
        public DbSet<Tenant> Tenants { get; set; }

        private static bool _migrated;

        public TenantsDbContext(DbContextOptions<TenantsDbContext> options)
            : base(options)
        {
            if (_migrated) return;
            Database.Migrate();
            _migrated = true;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Tenant>().HasKey(p => p.Id);

            base.OnModelCreating(builder);
        }
    }
}
