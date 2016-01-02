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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Skimur.Markdown;
using Microsoft.AspNet.StaticFiles;
using System.Security.Claims;
using Skimur.App;
using Registrar = Skimur.App.Registrar;

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
            SkimurContext.Initialize(
                new ServiceCollectionRegistrar(services, 0),
                this,
                new Registrar(),
                new App.Handlers.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Skimur.Scraper.Registrar());



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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseMiddleware<IpBlockerMiddleware>();
            app.UseMiddleware<ForceHttpsMiddleware>();
            app.UseMiddleware<ForceDomainMiddleware>();

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();

            var facebookAppId = Configuration["Skimur:Authentication:Facebook:AppId"];
            var facebookAppSecret = Configuration["Skimur:Authentication:Facebook:AppSecret"];
            if (!string.IsNullOrEmpty(facebookAppId) && !string.IsNullOrEmpty(facebookAppSecret))
            {
                app.UseFacebookAuthentication(options =>
                {
                    options.AppId = facebookAppId;
                    options.AppSecret = facebookAppSecret;
                });
            }

            var googleClientId = Configuration["Skimur:Authentication:Google:ClientId"];
            var googleClientSecret = Configuration["Skimur:Authentication:Google:ClientSecret"];
            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
            {
                app.UseGoogleAuthentication(options =>
                {
                    options.ClientId = googleClientId;
                    options.ClientSecret = googleClientSecret;
                    options.Scope.Add("https://www.googleapis.com/auth/plus.profile.emails.read");
                });
            }
            
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
            serviceCollection.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
            serviceCollection.AddSingleton<IThumbnailCacheService, ThumbnailCacheService>();
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
            serviceCollection.AddScoped<IUserStore<User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IRoleStore<Role>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IUserRoleStore<User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IPasswordHasher<User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<IUserValidator<User>>(provider => provider.GetService<ApplicationUserStore>());
            serviceCollection.AddScoped<ILookupNormalizer, ApplicationLookupNormalizer>();
            serviceCollection.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireNonLetterOrDigit = false;
                options.Password.RequireUppercase = false;
            }).AddDefaultTokenProviders();

            serviceCollection.AddScoped<IViewComponentInvokerFactory, ViewComponentInvokerFactory>();
            serviceCollection.AddMvc(options =>
            {
                options.ModelValidatorProviders.Add(new FluentValidationModelValidatorProvider());
            });

            serviceCollection.AddScoped<IUserContext, UserContext>();
            serviceCollection.AddScoped<IContextService, ContextService>();

            serviceCollection.AddSession();
            serviceCollection.AddCaching(); // adds a default in-memory implementation of IDistributedCache
            serviceCollection.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Admin");
                });
            });
            

            serviceCollection.AddOptions();
            serviceCollection.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.Events = new CustomCookieAuthenticationEvents();
            });
        }

        #endregion
    }
}
