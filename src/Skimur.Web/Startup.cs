using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skimur.Web.Services;
using Skimur.Utils;
using Skimur.Web.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Skimur.Web.Services.Impl;
using Skimur.FileSystem;
using Skimur.Settings;
using Skimur.Web.Infrastructure;
using Microsoft.AspNet.Mvc.ViewComponents;

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
                new Membership.Registrar(),
                new Emails.Handlers.Registrar(),
                new Subs.Registrar());

            // this will start the command listeners for subs, email, etc.
            SkimurContext.ServiceProvider.GetService<Messaging.IBusLifetime>();

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

            app.UseSession();
            
            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["Skimur:Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Skimur:Authentication:Facebook:AppSecret"];
            });
            
            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = Configuration["Skimur:Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Skimur:Authentication:Google:ClientSecret"];
                options.Scope.Add("https://www.googleapis.com/auth/plus.profile.emails.read");
            });

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseMvc(routes =>
            {
                Routes.Register(routes);
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        #region IRegistrar

        public int Order => 0;

        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfiguration>(provider => Configuration);
            serviceCollection.AddSingleton<IEmailSender, AuthMessageSender>();
            serviceCollection.AddSingleton<ISmsSender, AuthMessageSender>();
            serviceCollection.AddSingleton<IAvatarService, AvatarService>();
            serviceCollection.AddSingleton<IFileSystem>(provider =>
            {
                var webSettings = provider.GetService<ISettingsProvider<WebSettings>>();
                var dataDirectory = provider.GetService<IPathResolver>().Resolve(webSettings.Settings.DataDirectory);
                return new LocalFileSystem(dataDirectory);
            });
            
            serviceCollection.AddScoped<ApplicationUserStore>();
            serviceCollection.AddScoped<IUserStore<Membership.User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IRoleStore<Membership.Role>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IPasswordHasher<Membership.User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IUserValidator<Membership.User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<ILookupNormalizer, ApplicationLookupNormalizer>();
            serviceCollection.AddIdentity<Membership.User, Membership.Role>(options => 
            {
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
            }).AddDefaultTokenProviders();

            serviceCollection.AddScoped<IViewComponentInvokerFactory, ViewComponentInvokerFactory>();
            serviceCollection.AddMvc();

            serviceCollection.AddScoped<IUserContext, UserContext>();
            serviceCollection.AddScoped<IContextService, ContextService>();

            serviceCollection.AddSession();
            serviceCollection.AddCaching(); // adds a default in-memory implementation of IDistributedCache
            serviceCollection.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        #endregion
    }
}
