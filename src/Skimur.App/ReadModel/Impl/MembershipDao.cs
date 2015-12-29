using Skimur.App.Services;
using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
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
