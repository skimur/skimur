using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skimur.Web.Services;
using Skimur.Utils;
using Skimur.Web.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Membership;
using Skimur.Web.Services.Impl;

namespace Skimur.Web
{
    public class Startup : IRegistrar
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            SkimurContext.ContainerInitialized += Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(
                new ServiceCollectionRegistrar(services, 0), 
                this,
                new Membership.Registrar());
            return SkimurContext.ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        #region IRegistrar

        public int Order => 0;

        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfiguration>(provider => Configuration);
            //// Add framework services.
            //services.AddEntityFramework()
            //    .AddSqlServer()
            //    .AddDbContext<ApplicationDbContext>(options =>
            //        options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            serviceCollection.AddIdentity<User, Role>().AddDefaultTokenProviders()
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationUserStore>();


            serviceCollection.AddMvc();

            //// Add application services.
            serviceCollection.AddSingleton<IEmailSender, AuthMessageSender>();
            serviceCollection.AddSingleton<ISmsSender, AuthMessageSender>();
        }

        #endregion
    }
}
