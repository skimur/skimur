using Skimur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.ReadModel;
using Membership.ReadModel.Impl;
using Membership.Services;
using Membership.Services.Impl;
using SimpleInjector;

namespace Membership
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingleton<IMembershipService, MembershipService>();
            container.RegisterSingleton<IMembershipDao, MembershipDao>();
            container.RegisterSingleton<IPasswordManager, PasswordManager>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
