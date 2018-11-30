using System;
using System.IdentityModel.Tokens.Jwt;
using Dotnettency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultitenantClient
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // services.AddRouting();
            services.AddMiddlewareAnalysis();
            //  services.AddMvc();
            services.AddWebEncoders(); // Not sure why this is necessary. See https://github.com/aspnet/Mvc/issues/8340 may not be necessary in 2.1.0

            _loggerFactory.AddConsole();
            ILogger<Startup> logger = _loggerFactory.CreateLogger<Startup>();


            IServiceProvider serviceProvider = services.AddAspNetCoreMultiTenancy<Tenant>((options) =>
            {
                options
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        containerBuilder.WithAutofac((tenant, tenantServices) =>
                        {
                            //tenantServices.Configure<CookiePolicyOptions>(opt =>
                            //{
                            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                            //    opt.CheckConsentNeeded = context => true;
                            //    opt.MinimumSameSitePolicy = SameSiteMode.None;
                            //});

                            tenantServices.AddSingleton(_environment); // See https://github.com/aspnet/Mvc/issues/8340
                            tenantServices.AddWebEncoders();

                            tenantServices.AddMvc()
                            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                            if (tenant.Name == "Moogle")
                            {
                                tenantServices.AddAuthentication(opt =>
                                {
                                    opt.DefaultScheme = "Cookies";
                                    opt.DefaultChallengeScheme = "oidc";
                                })
                                 .AddCookie("Cookies", opt =>
                                 {

                                     opt.Cookie.Name = "Cookie";


                                 })
                                .AddOpenIdConnect("oidc", opt =>
                                {
                                    opt.SignInScheme = "Cookies";

                                    opt.Authority = "http://localhost:5000";
                                    opt.RequireHttpsMetadata = false;

                                    opt.ClientId = "mvc";
                                    opt.SaveTokens = true;
                                });
                            }
                            else
                            {
                                tenantServices.AddAuthentication(opt =>
                                {
                                    opt.DefaultScheme = "Cookies";
                                    opt.DefaultChallengeScheme = "oidc";
                                })
                              .AddCookie("Cookies", opt =>
                              {

                                  opt.Cookie.Name = "Cookie2";


                              })
                              .AddOpenIdConnect("oidc", opt =>
                              {
                                  opt.SignInScheme = "Cookies";

                                  opt.Authority = "http://localhost:5002";
                                  opt.RequireHttpsMetadata = false;

                                  opt.ClientId = "mvc2";
                                  opt.SaveTokens = true;
                              });

                            }
                        })
                        .AddPerRequestContainerMiddlewareServices() // services needed for per tenant container middleware.
                        .AddPerTenantMiddlewarePipelineServices(); // services needed for per tenant middleware pipeline.
                    })
                    .ConfigureTenantMiddleware((a) =>
                    {
                        a.OnInitialiseTenantPipeline((b, c) =>
                        {
                          // c.UseHttpsRedirection();
                            c.UseStaticFiles();
                            c.UseCookiePolicy();

                            c.UseAuthentication();

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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app = app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantContainers();
                options.UsePerTenantMiddlewarePipeline();
            });
        }
    }
}
