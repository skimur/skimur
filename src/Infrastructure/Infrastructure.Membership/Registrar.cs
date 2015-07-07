using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Membership
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingle<IMembershipService, MembershipService>();
            container.RegisterSingle<IPasswordManager, PasswordManager>();
        }

        public int Order { get { return 0; } }
    }
}
