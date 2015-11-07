using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BundleTransformer.Core.Transformers;
using FluentValidation.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using SimpleInjector;
using Skimur.Logging;
using Skimur.Messaging;
using Skimur.Settings;
using Skimur.Web.Middleware;
using Skimur.Web.Public;

// ReSharper disable once RedundantNameQualifier

[assembly: OwinStartup(typeof(Startup))]
namespace Skimur.Web.Public
{
    public class Startup : IRegistrar
    {
        IAppBuilder _app;
        // todo: factor out the command/event bus handling to separate exe via a build script
        IBusLifetime _busLifetime;

        public void Configuration(IAppBuilder app)
        {
            _app = app;
            RegisterGlobalFilters(GlobalFilters.Filters);
            Routes.Register(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);
            ConfigureAuth(app);
            ConfigureContainer();

            app.Use<IpBlockerMiddleware>(SkimurContext.Resolve<ISettingsProvider<BlockedIps>>());
            app.Use<RequestLoggingMiddleware>(SkimurContext.Resolve<ISettingsProvider<RequestLogging>>(), SkimurContext.Resolve<ILogger<RequestLoggingMiddleware>>());

            // todo: factor out the command/event bus handling to separate exe via a build script
            _busLifetime = SkimurContext.Resolve<IBusLifetime>();

            FluentValidationModelValidatorProvider.Configure();
        }

        public void ConfigureContainer()
        {
            SkimurContext.ContainerInitialized += Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(new Registrar(),
                new Markdown.Registrar(),
                new Scraper.Registrar(),
                new Subs.Registrar(),
                new Subs.Worker.Registrar(),
                new Emails.Handlers.Registrar(),
                new Membership.Registrar(), 
                this);
        }
        
        public void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Mvc.SkimurHandleErrorAttribute());
            filters.Add(new Mvc.AnnouncementFilterAttribute());
        }

        public void RegisterBundles(BundleCollection bundles)
        {
            // no need to do this if we are using static assets!
            if (AssetUrlBuilder.IsUsingStatisAssets) return;

            var scriptTransformer = new ScriptTransformer();

            var scriptsBundle = new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/jquery.js",
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.bootstrap.js",
                "~/Scripts/modernizr.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-notify.js",
                "~/Scripts/sweet-alert.js",
                "~/Scripts/marked.js",
                "~/Scripts/markedHelper.js",
                "~/Scripts/to-markdown.js",
                "~/Scripts/bootstrap-markdown.js",
                "~/Scripts/app/api.js",
                "~/Scripts/app/ui.js",
                "~/Scripts/app/login.js",
                "~/Scripts/app/misc.js",
                "~/Scripts/app/comments.js",
                "~/Scripts/app/posts.js",
                "~/Scripts/app/subs.js",
                "~/Scripts/app/messages.js",
                "~/Scripts/app/moderators.js");
            
            var stylesBundle = new StyleBundle("~/bundles/styles").Include(
                "~/Content/site.less");

            scriptsBundle.Transforms.Add(scriptTransformer);
            stylesBundle.Transforms.Add(new StyleTransformer());

            bundles.Add(scriptsBundle);
            bundles.Add(stylesBundle);
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/account/login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            var facebookAppId = Environment.GetEnvironmentVariable("SkimurSSOFacebookAppId");
            var facebookSecret = Environment.GetEnvironmentVariable("SkimurSSOFacebookAppSecret");

            if (!string.IsNullOrEmpty(facebookAppId) && !string.IsNullOrEmpty(facebookSecret))
                app.UseFacebookAuthentication(facebookAppId, facebookSecret);

            var googleClientId = Environment.GetEnvironmentVariable("SkimurSSOGoogleClientId");
            var googleSecret = Environment.GetEnvironmentVariable("SkimurSSOGoogleSecret");

            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleSecret))
                app.UseGoogleAuthentication(googleClientId, googleSecret);
        }

        public void Register(Container container)
        {
            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);
            container.RegisterSingleton(_app.GetDataProtectionProvider());
            container.RegisterMvcControllers(typeof(global::Skimur.Web.Registrar).Assembly);
        }

        public int Order { get { return 0; } }
    }
}
