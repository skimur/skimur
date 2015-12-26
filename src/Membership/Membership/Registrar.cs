using Skimur;
using Membership.ReadModel;
using Membership.ReadModel.Impl;
using Membership.Services;
using Membership.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Membership
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMembershipService, MembershipService>();
            serviceCollection.AddSingleton<IMembershipDao, MembershipDao>();
            serviceCollection.AddSingleton<IPasswordManager, PasswordManager>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
