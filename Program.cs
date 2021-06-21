// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;

namespace ScarletAuth
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                .WriteTo.File(
                   @"identityserver.log",
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                   fileSizeLimitBytes: 1_000_000,
                   rollOnFileSizeLimit: true,
                   shared: true,
                   flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            var seedIDP = args.Any(x => x == "/seedIDP");
            if (seedIDP) args = args.Except(new[] { "/seedIDP" }).ToArray();
            var seedUsers = args.Any(x => x == "/seedUsers");
            if (seedUsers) args = args.Except(new[] { "/seedUsers" }).ToArray();

            try
            {
                Log.Information("Starting host...");
                var host = CreateHostBuilder(args).Build();

                if (seedIDP)
                {
                    Log.Information("Seeding IdentityServer4 Database");
                    SeedData.InitializeIDPDatabase(host.Services);
                    Log.Information("Finished IdentityServer4 Database");
                    return 0;
                }

                if (seedUsers)
                {
                    Log.Information("Seeding .NET Identity Database");
                    SeedData.InitializeUsersDatabase(host.Services);
                    Log.Information("Finsihed .NET Identity Database");
                    return 0;
                }  

                Log.Information("Starting Host");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}