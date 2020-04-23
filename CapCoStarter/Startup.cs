using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using CapCoStarter.Data;
using CapCoStarter.HttpHandlers;

namespace CapCoStarter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            //register 

            services.AddServerSideBlazor();                 

            services.AddHttpContextAccessor();
            services.AddTransient<BearerTokenHandler>();//add the bearer token handler

            //add api client with additional bearer token added
            //services.AddHttpClient("APIClient", client =>
            // {
            //     client.BaseAddress = new Uri("https://localhost:44228/");//this is the client address
            //     client.DefaultRequestHeaders.Clear();
            //     client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            // }).AddHttpMessageHandler<BearerTokenHandler>();
            //var pslmUri = new Uri(Configuration.GetValue<string>("ApiUri")),

            var pslmUri = new Uri("https://localhost:44228/");
            void RegisterTypedClient<TClient, TImplementation>(Uri apiBaseUrl)
                where TClient : class where TImplementation : class, TClient
            {
                services.AddHttpClient<TClient, TImplementation>(client =>
                {
                    client.BaseAddress = apiBaseUrl;
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
                }).AddHttpMessageHandler<BearerTokenHandler>();
            }
            RegisterTypedClient<ITestDataService, TestDataService>(pslmUri);


            //add a client to access the openid information
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44338/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, 
                options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44338";
                    options.ClientId = "CapCoStarterclient";
                    options.ResponseType = "code id_token";
                    options.ClientSecret = "secret";
                    options.Scope.Add("CapCoStarterapi");
                    //options.UsePkce = false;
                    //options.CallbackPath = new Microsoft.AspNetCore.Http.PathString("..."); comment out to use default.
                    //options.SignedOutCallbackPath = new Microsoft.AspNetCore.Http.PathString("..."); comment out to use default.
                    options.Scope.Add("openid");//this is default
                    options.Scope.Add("profile");//this is default
                    options.Scope.Add("email");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    //also make sure that the roles are mapped, otherwise it wont show up.
                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    //make sure address not in claims identity
                    //options.ClaimActions.DeleteClaim("address"); not needed as it does not map in claims identity
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //add this before the  routing, so auth is triggered before any routing.
            app.UseAuthentication(); //1
            app.UseAuthorization(); // 2
            //now continue to routing

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
