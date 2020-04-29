// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CapCoStarter.IDP.Areas.Identity.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace CapCoStarter.IDP
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();
            services.AddMvc();

            var ipdDataDbConnectionString = "Server=(localdb)\\mssqllocaldb;Database=CapCoStarter;Trusted_Connection=True;MultipleActiveResultSets=true";

            var builder = services.AddIdentityServer(option =>
            {
                option.Events.RaiseErrorEvents = true;
                option.Events.RaiseInformationEvents = true;
                option.Events.RaiseFailureEvents = true;
                option.Events.RaiseSuccessEvents = true;
            })
                //.AddInMemoryIdentityResources(Config.Ids)
                //.AddInMemoryApiResources(Config.Apis)
                //.AddInMemoryClients(Config.Clients)
                //.AddTestUsers(TestUsers.Users);
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            var migrationAssm = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                builder.UseSqlServer(ipdDataDbConnectionString,
                options => options.MigrationsAssembly(migrationAssm));
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                 builder.UseSqlServer(ipdDataDbConnectionString,
                 options => options.MigrationsAssembly(migrationAssm));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitDatabase(app);//this could/should really be a sql script

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

        private void InitDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                    .Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                        context.Clients.Add(client.ToEntity());
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                        context.IdentityResources.Add(resource.ToEntity());
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var apiResource in Config.Apis)
                        context.ApiResources.Add(apiResource.ToEntity());
                    context.SaveChanges();
                }
            }
        }
    }
}
