using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MultitenantIdentity.Data
{
    public class ApplicationDbContextDesignTimeFactory : DesignTimeDbContextFactoryBase<ApplicationDbContext>
    {
        public ApplicationDbContextDesignTimeFactory()
            : base("DefaultConnection", typeof(Startup).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override ApplicationDbContext CreateNewInstance(DbContextOptions<ApplicationDbContext> options)
        {
            return new ApplicationDbContext(options);
        }
    }
}
