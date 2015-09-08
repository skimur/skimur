using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BundleTransformer.Core.Transformers;
using Infrastructure.Messaging;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using SimpleInjector;
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

            // todo: factor out the command/event bus handling to separate exe via a build script
            _busLifetime = SkimurContext.Resolve<IBusLifetime>();
        }

        public void ConfigureContainer()
        {
            SkimurContext.ContainerInitialized += Infrastructure.Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Infrastructure.Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(new Infrastructure.Registrar(),
                new Infrastructure.Settings.Registrar(),
                new Infrastructure.Caching.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
                new Infrastructure.Cassandra.Registrar(),
                new Infrastructure.Postgres.Registrar(),
                new Infrastructure.Logging.Registrar(),
                new Markdown.Registrar(),
                new Subs.Registrar(),
                new Subs.Worker.Registrar(),
                new Membership.Registrar(),
                this);
        }
        
        public void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public void RegisterBundles(BundleCollection bundles)
        {
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
                "~/Scripts/markdown.js",
                "~/Scripts/to-markdown.js",
                "~/Scripts/bootstrap-markdown.js",
                "~/Scripts/app/api.js",
                "~/Scripts/app/ui.js",
                "~/Scripts/app/login.js",
                "~/Scripts/app/misc.js",
                "~/Scripts/app/comments.js",
                "~/Scripts/app/posts.js",
                "~/Scripts/app/subs.js",
                "~/Scripts/app/messages.js");
            
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
