﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Dotnettency;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MultitenantIdentity
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
                            tenantServices.AddSingleton(_environment); // See https://github.com/aspnet/Mvc/issues/8340
                            tenantServices.AddWebEncoders();

                            tenantServices.AddMvc()
                            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                            tenantServices.AddMvc();

                            if (tenant.Name == "Moogle")
                            {
                                // configure identity server with in-memory stores, keys, clients and resources
                                tenantServices.AddIdentityServer()
                                    .AddDeveloperSigningCredential()
                                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                                    .AddInMemoryApiResources(Config.GetApiResources())
                                    .AddInMemoryPersistedGrants()
                                    .AddInMemoryClients(Config.GetClients())
                                    .AddTestUsers(Config.GetUsers());

                                tenantServices.AddAuthentication()
                                        .AddGoogle("Google", opt =>
                                        {
                                            opt.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                                            opt.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                                            opt.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                                        });

                            }
                            else
                            {
                                    // configure identity server with in-memory stores, keys, clients and resources
                                    tenantServices.AddIdentityServer()
                                        .AddDeveloperSigningCredential()
                                        .AddInMemoryIdentityResources(Config2.GetIdentityResources())
                                        .AddInMemoryApiResources(Config2.GetApiResources())
                                        .AddInMemoryPersistedGrants()
                                        .AddInMemoryClients(Config2.GetClients())
                                        .AddTestUsers(Config2.GetUsers());

                                    tenantServices.AddAuthentication()
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

                        })
                        .AddPerRequestContainerMiddlewareServices() // services needed for per tenant container middleware.
                        .AddPerTenantMiddlewarePipelineServices(); // services needed for per tenant middleware pipeline.
                    })
                    .ConfigureTenantMiddleware((a) =>
                    {
                        a.OnInitialiseTenantPipeline((b, c) =>
                        {
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
