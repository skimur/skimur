using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Membership.Services;
using Membership.Services.Impl;

namespace Membership.ReadModel.Impl
{
    public class MembershipDao : 
        MembershipService,
        IMembershipDao
    {
        public MembershipDao(IDbConnectionProvider connectionProvider, IPasswordManager passwordManager)
            :base(connectionProvider, passwordManager)
        {
            
        }
    }
}
