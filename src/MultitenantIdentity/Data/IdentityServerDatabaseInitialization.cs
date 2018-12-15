using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MultitenantIdentity.Data
{
    public static class IdentityServerDatabaseInitialization
    {
        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope())
            {
                PerformMigrations(serviceScope);
                SeedData(serviceScope);
            }
        }

        private static void PerformMigrations(IServiceScope serviceScope)
        {
            serviceScope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>()
                .Database
                .Migrate();

            serviceScope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>()
                .Database
                .Migrate();

            serviceScope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database
                .Migrate();
        }

        private static void SeedData(IServiceScope serviceScope)
        {
            var context = serviceScope
                .ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (!userManager.Users.Any())
            {
                foreach (var testUser in Config.GetUsers())
                {
                    var identityUser = new ApplicationUser(testUser.Username)
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = testUser.Username,
                        Email = testUser.Username + "@mail.com",
                        EmailConfirmed = false,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };

                    userManager.CreateAsync(identityUser, testUser.Password).Wait();
                    userManager.AddClaimsAsync(identityUser, testUser.Claims.ToList()).Wait();
                }

            }
        }
    }
}
