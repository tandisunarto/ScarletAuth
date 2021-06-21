// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScarletAuth.Controllers;
using ScarletAuth.Data;
using ScarletAuth.ViewModels;
using Serilog;

namespace ScarletAuth
{
    public class Startup
    {
        private readonly ILogger log;

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            var identityServerConnection = Configuration.GetConnectionString("IdentityServerConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            Log.Information($"My Connection String is {identityServerConnection}");

            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseNpgsql(identityServerConnection, options => options.MigrationsAssembly(migrationAssembly));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                // .AddTestUsers(TestUsers.Users);
                .AddAspNetIdentity<ApplicationUser>();

            // in-memory, code config
            // builder.AddInMemoryIdentityResources(Config.IdentityResources);
            // builder.AddInMemoryApiResources(Config.ApiResources);
            // builder.AddInMemoryApiScopes(Config.ApiScopes);
            // builder.AddInMemoryClients(Config.Clients);
            builder.AddConfigurationStore(options => {
                options.ConfigureDbContext = builder => 
                    builder.UseNpgsql(identityServerConnection,
                        option => option.MigrationsAssembly(migrationAssembly));
            });
            builder.AddOperationalStore(options => {
                options.ConfigureDbContext = builder => 
                    builder.UseNpgsql(identityServerConnection,
                        option => option.MigrationsAssembly(migrationAssembly));
            });

            // not recommended for production - you need to store your key material somewhere secure
            // if (Environment.IsDevelopment())
                builder.AddDeveloperSigningCredential();
            // else
                // SetupSigningCredential(builder);

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:5001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });
        }

        private void SetupSigningCredential(IIdentityServerBuilder builder)
        {
            var keyFilePath = Configuration["KeyFilePath"];
            var keyFilePassword = Configuration["KeyFilePassword"];

            if (File.Exists(keyFilePath))
                builder.AddSigningCredential(new X509Certificate2(keyFilePath, keyFilePassword));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}