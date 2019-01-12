using System;
using System.Reflection;
using CorrelationId;
using Dotnettency;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MultitenantIdentity.Data;
using Tenants.Web.Client;
using Tenants.Web.Client.Options;
using Tenants.Web.Client.Framework;

namespace MultitenantIdentity
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        
        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration
            )
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // services.AddRouting();
            services.AddMiddlewareAnalysis();
            //  services.AddMvc();
            services.AddWebEncoders(); // Not sure why this is necessary. See https://github.com/aspnet/Mvc/issues/8340 may not be necessary in 2.1.0

            _loggerFactory.AddConsole();
            ILogger<Startup> logger = _loggerFactory.CreateLogger<Startup>();

            services.AddCorrelationId(); // Add Correlation ID support to ASP.NET Core

            services
                .AddPolicies(this._configuration) // Setup Polly policies.
                .AddHttpClient<ITenantsClient, TenantsClient, TenantsClientOptions>(this._configuration, "TenantsClient");

            //var serviceProvider1 = services.BuildServiceProvider();
            //var tenantsClient = serviceProvider1.GetRequiredService<ITenantsClient>();
            //var tenants = tenantsClient.GetTenants().Result;

            IServiceProvider serviceProvider = services.AddAspNetCoreMultiTenancy<Tenant>((options) =>
            {
                options
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        containerBuilder.WithAutofac((tenant, tenantServices) =>
                        {
                          

                            tenantServices.AddSingleton(_environment); // See https://github.com/aspnet/Mvc/issues/8340
                            tenantServices.AddWebEncoders();

                            var connectionString = _configuration.GetConnectionString("DefaultConnection");
                            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                            tenantServices.AddDbContext<ApplicationDbContext>(opt =>
                                opt.UseSqlServer(connectionString));

                            tenantServices.AddIdentity<ApplicationUser, ApplicationRole>()
                                .AddEntityFrameworkStores<ApplicationDbContext>()
                                .AddDefaultTokenProviders();

                            // Add application services.
                            //tenantServices.AddTransient<IEmailSender, EmailSender>();

                            tenantServices.AddMvc()
                                          .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


                            if (tenant.Name == "Tenant1") {

                                // configure identity server with in-memory stores, keys, clients and resources
                                tenantServices.AddIdentityServer()
                                    .AddDeveloperSigningCredential()
                                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                                    .AddInMemoryApiResources(Config.GetApiResources())
                                    .AddInMemoryPersistedGrants()
                                    .AddInMemoryClients(Config.GetClients())
                                    .AddTestUsers(Config.GetUsers())
                                    //.AddConfigurationStore(opt =>
                                    //{
                                    //    opt.ConfigureDbContext = builder =>
                                    //        builder.UseSqlServer(connectionString,
                                    //            sql => sql.MigrationsAssembly(migrationsAssembly));
                                    //})
                                    //// this adds the operational data from DB (codes, tokens, consents)
                                    //.AddOperationalStore(opt =>
                                    //{
                                    //    opt.ConfigureDbContext = builder =>
                                    //        builder.UseSqlServer(connectionString,
                                    //            sql => sql.MigrationsAssembly(migrationsAssembly));

                                    //// this enables automatic token cleanup. this is optional.
                                    //opt.EnableTokenCleanup = true;
                                    //    opt.TokenCleanupInterval = 30;
                                    //})
                                   // .AddAspNetIdentity<ApplicationUser>()
                                    ;



                                tenantServices.AddAuthentication()
                                    .AddCookie("Cookies", opt =>
                                    {

                                        opt.Cookie.Name = "Cookie";


                                    })
                                        .AddGoogle("Google", opt =>
                                        {
                                            opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                                            opt.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                                            opt.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                                        })
                                        .AddOpenIdConnect("oidc", "OpenID Connect", opt =>
                                        {
                                            opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                            opt.SignOutScheme = IdentityServerConstants.SignoutScheme;

                                            opt.Authority = "https://demo.identityserver.io/";
                                            opt.ClientId = "implicit";

                                            opt.TokenValidationParameters = new TokenValidationParameters
                                            {
                                                NameClaimType = "name",
                                                RoleClaimType = "role"
                                            };
                                        });

                            }
                            else
                            {
                                //Tenant2

                                // configure identity server with in-memory stores, keys, clients and resources
                                tenantServices.AddIdentityServer()
                                    .AddDeveloperSigningCredential()
                                    .AddInMemoryIdentityResources(Config2.GetIdentityResources())
                                    .AddInMemoryApiResources(Config2.GetApiResources())
                                    .AddInMemoryPersistedGrants()
                                    .AddInMemoryClients(Config2.GetClients())
                                    .AddTestUsers(Config2.GetUsers());

                                tenantServices.AddAuthentication()
                                    .AddCookie("Cookies", opt => { opt.Cookie.Name = "Cookie2"; });

                                //.AddConfigurationStore(opt =>
                                //{
                                //    opt.ConfigureDbContext = builder =>
                                //        builder.UseSqlServer(connectionString,
                                //            sql => sql.MigrationsAssembly(migrationsAssembly));
                                //})
                                //// this adds the operational data from DB (codes, tokens, consents)
                                //.AddOperationalStore(opt =>
                                //{
                                //    opt.ConfigureDbContext = builder =>
                                //        builder.UseSqlServer(connectionString,
                                //            sql => sql.MigrationsAssembly(migrationsAssembly));

                                //// this enables automatic token cleanup. this is optional.
                                //opt.EnableTokenCleanup = true;
                                //    opt.TokenCleanupInterval = 30;
                                //})
                                //.AddAspNetIdentity<ApplicationUser>()
                                ;

                                //tenantServices.AddAuthentication()
                                //        .AddGoogle("Google", opt =>
                                //        {
                                //            opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                                //            opt.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                                //            opt.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                                //        })
                                //        .AddOpenIdConnect("oidc", "OpenID Connect", opt =>
                                //        {
                                //            opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                //            opt.SignOutScheme = IdentityServerConstants.SignoutScheme;

                                //            opt.Authority = "https://demo.identityserver.io/";
                                //            opt.ClientId = "implicit";

                                //            opt.TokenValidationParameters = new TokenValidationParameters
                                //            {
                                //                NameClaimType = "name",
                                //                RoleClaimType = "role"
                                //            };
                                //        });


                            }

                     

                        })
                        .AddPerRequestContainerMiddlewareServices() // services needed for per tenant container middleware.
                        .AddPerTenantMiddlewarePipelineServices(); // services needed for per tenant middleware pipeline.
                    })
                    .ConfigureTenantMiddleware((a) =>
                    {
                        a.OnInitialiseTenantPipeline((b, c) =>
                        {

                            // IdentityServerDatabaseInitialization.InitializeDatabase(c);

                            //c.UseDeveloperExceptionPage();
                            c.UseStaticFiles();

                            c.UseIdentityServer();

                            c.UseMvc(routes =>
                            {
                                routes.MapRoute(
                                    name: "default",
                                    template: "{controller=Home}/{action=Index}/{id?}");
                            });


                        });
                    });

            });

             // When using tenant containers, must return IServiceProvider.
             return serviceProvider;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCorrelationId();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app = app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantContainers();
                options.UsePerTenantMiddlewarePipeline();
            });
        }

    }
}
