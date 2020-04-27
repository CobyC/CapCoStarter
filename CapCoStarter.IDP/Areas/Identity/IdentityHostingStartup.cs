using System;
using CapCoStarter.IDP.Areas.Identity.Data;
using CapCoStarter.IDP.Data;
using CapCoStarter.IDP.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CapCoStarter.IDP.Areas.Identity.IdentityHostingStartup))]
namespace CapCoStarter.IDP.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("UserDbContextConnection")));

                //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<UserDbContext>();

                //add oidc
                services.AddIdentity<ApplicationUser, IdentityRole>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();
                //.AddDefaultUI() // if default UI needs to be used.

                services.AddTransient<IEmailSender, EmailSender>();
            });
        }
    }
}