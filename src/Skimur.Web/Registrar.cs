using System;
using System.Web.Mvc;
using Infrastructure;
using Infrastructure.FileSystem;
using Infrastructure.Settings;
using Microsoft.AspNet.Identity;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using Skimur.Web.Avatar;
using Skimur.Web.Identity;
using Skimur.Web.Services;
using Skimur.Web.Services.Impl;

namespace Skimur.Web
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<IUserStore<ApplicationUser, Guid>, ApplicationUserStore>();
            container.RegisterSingleton<IIdentityMessageService, ApplicationIdentityMessageService>();
            container.RegisterSingleton<ApplicationUserManager>();
            container.RegisterSingleton<IAvatarService, AvatarService>();
            container.RegisterSingleton<IThumbnailCacheService, ThumbnailCacheService>();

            container.RegisterPerWebRequest<ApplicationSignInManager>();
            container.RegisterPerWebRequest<IUserContext, UserContext>();
            container.RegisterPerWebRequest<IContextService, ContextService>();

            container.RegisterSingleton<IFileSystem>(() =>
            {
                var webSettings = container.GetInstance<ISettingsProvider<WebSettings>>();
                var dataDirectory = container.GetInstance<IPathResolver>().Resolve(webSettings.Settings.DataDirectory);
                return new LocalFileSystem(dataDirectory);
            });

            container.RegisterMvcIntegratedFilterProvider();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        public int Order { get { return 0; } }
    }
}
