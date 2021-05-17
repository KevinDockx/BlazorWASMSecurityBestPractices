using Duende.Bff;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Marvin.BlazorBFF.Server
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
            services.AddBff()
              .AddServerSideSessions();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "localAuthCookie";
                options.DefaultChallengeScheme = "oidc";
                options.DefaultSignOutScheme = "oidc";
            })
              .AddCookie("localAuthCookie", options =>
              {
                  options.Cookie.Name = "__Host-blazor";
                  options.Cookie.SameSite = SameSiteMode.Strict;
              })
              .AddOpenIdConnect("oidc", options =>
              {
                  options.Authority = "https://localhost:44318";

                  // code flow + PKCE with secret (client is confidential)
                  options.ClientId = "wasmbffcode";
                  options.ClientSecret = "secret";
                  options.ResponseType = "code";
                 // options.ResponseMode = "query";

                  options.MapInboundClaims = false;
                  options.GetClaimsFromUserInfoEndpoint = true;
                  options.SaveTokens = true;
                   
                  options.Scope.Clear();
                  options.Scope.Add("openid");
                  options.Scope.Add("profile");
                  options.Scope.Add("myapi");
                  options.Scope.Add("offline_access");
              });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBffManagementEndpoints();

                endpoints.MapRemoteBffApiEndpoint("/downstream", "https://localhost:44378/")
                    .RequireAccessToken(TokenType.User);

                endpoints.MapRazorPages();

                endpoints.MapControllers()
                    .RequireAuthorization()
                    .AsLocalBffApiEndpoint();

                endpoints.MapFallbackToFile("index.html"); 
            });
        }
    }
}
