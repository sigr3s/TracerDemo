using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TracerDemo.Data;
using TracerDemo.Helpers;
using TracerDemo.Model;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TracerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration.Get<ApplicationSettings>();
            services.AddSingleton(settings);

            //When an access token is sent to the server, use these rules to validate the token.
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.SigningKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = new TimeSpan(1000, 0, 0, 0);
                options.Cookie = new CookieBuilder()
                {
                    Name = "access_token",
                    HttpOnly = true,
                    SameSite = SameSiteMode.None
                };
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.LoginPath = PathString.Empty;
                options.TicketDataFormat = new CustomJwtDataFormat(SecurityAlgorithms.HmacSha256, tokenValidationParameters);
            });

            // Add framework services.
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("no-cache", new CacheProfile()
                {
                    NoStore = true,
                });
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });



            services.AddTransient<SqliteContext>();
            services.AddTransient<HasherHelper>();
            services.AddTransient<ValidationHelper>();
            services.AddTransient<TokenHelper>();
            services.AddTransient<UserHelper>();
            services.AddTransient<SummonerHelper>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationSettings settings)
        {
            app.UseAuthentication();

            app.UseCors("AllowAll");
            app.UseMvcWithDefaultRoute();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SqliteContext>();
                context.Database.Migrate();
            }
        }

    }


}
