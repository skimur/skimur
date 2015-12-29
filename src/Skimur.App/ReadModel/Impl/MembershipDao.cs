using Membership.Services;
using Membership.Services.Impl;
using Skimur.Data;

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
