using System;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Skimur.FileSystem;
using Skimur.Settings;
using Skimur.Web.Avatar;
using Skimur.Web.Identity;
using Skimur.Web.Services;
using Skimur.Web.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Skimur.Web
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUserStore<ApplicationUser, Guid>, ApplicationUserStore>();
            serviceCollection.AddSingleton<IIdentityMessageService, ApplicationIdentityMessageService>();
            serviceCollection.AddSingleton<ApplicationUserManager>();
            serviceCollection.AddSingleton<IAvatarService, AvatarService>();
            serviceCollection.AddSingleton<IThumbnailCacheService, ThumbnailCacheService>();

            serviceCollection.AddSingleton<ApplicationSignInManager>();
            serviceCollection.AddSingleton<IUserContext, UserContext>();
            serviceCollection.AddSingleton<IContextService, ContextService>();

            serviceCollection.AddSingleton<IFileSystem>(provider =>
            {
                var webSettings = provider.GetService<ISettingsProvider<WebSettings>>();
                var dataDirectory = provider.GetService<IPathResolver>().Resolve(webSettings.Settings.DataDirectory);
                return new LocalFileSystem(dataDirectory);
            });

            // container.RegisterMvcIntegratedFilterProvider();
        }

        public int Order { get { return 0; } }
    }
}
