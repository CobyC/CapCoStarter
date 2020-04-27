// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CapCoStarter.IDP.Areas.Identity.Data;
using CapCoStarter.IDP.Data;
using IdentityModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Security.Claims;

namespace CapCoStarter.IDP
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                var host = CreateHostBuilder(args).Build();

                //seeed the database
                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<UserDbContext>();

                        //ensure DB is migrated before seeding
                        context.Database.Migrate();

                        //use the user manager to create test users
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                        var bob = userManager.FindByNameAsync("bob").Result;
                        if (bob == null)
                        {
                            bob = new ApplicationUser
                            {
                                UserName = "Bob",
                                EmailConfirmed = true
                            };

                            var result = userManager.CreateAsync(bob, "P@ssword1").Result;
                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);

                            result = userManager.AddClaimsAsync(bob, new Claim[] {
                                new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Bob"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.Email, "Bob@Smith.com"),
                                new Claim(JwtClaimTypes.Role, "Admin")
                            }).Result;

                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);
                        }

                        var alice = userManager.FindByNameAsync("alice").Result;
                        if (alice == null)
                        {
                            alice = new ApplicationUser
                            {
                                UserName = "Alice",
                                EmailConfirmed = true
                            };

                            var result = userManager.CreateAsync(alice, "P@ssword1").Result;
                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);

                            result = userManager.AddClaimsAsync(alice, new Claim[] {
                                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Alice"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.Email, "Alice@Smith.com"),
                                new Claim(JwtClaimTypes.Role, "User")
                            }).Result;

                            if (!result.Succeeded)
                                throw new Exception(result.Errors.First().Description);
                        }
                    }
                    catch (Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Seeding error");
                    }
                }
                //now run the host app
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}