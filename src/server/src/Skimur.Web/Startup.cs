using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JavaScriptViewEngine;
using Microsoft.Extensions.Configuration;
using System.IO;
using Skimur.App;
using Skimur.Web.Services;
using System;
using System.Collections.Generic;
using Skimur.Email;
using Skimur.Sms;
using Skimur.Utils;

namespace Skimur.Web
{
    public class Startup : IRegistrar
    {
        IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true);

            if(hostingEnvironment.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            SkimurContext.Initialize(
                new ServiceCollectionRegistrar(services, 0),
                new Skimur.App.Registrar(),
                this);
            
            return SkimurContext.ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole((category, loglevel) => loglevel >= LogLevel.Warning);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/Status/Status/{0}");

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();

            app.UseJsEngine();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public int Order => 0;

        public void Register(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddJsEngine(builder =>
            {
                builder.UseNodeRenderEngine(options =>
                {
                    options.GetModuleName += (path, model, viewBag, routeValues, area, viewType) => "server.js";
                    options.ProjectDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "App");
                });
                builder.UseSingletonEngineFactory();
            });
            services.AddMvc();

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddUserStore<MembershipStore>()
                .AddRoleStore<MembershipStore>()
                .AddDefaultTokenProviders();

            services.AddSession();
        }
    }
}
