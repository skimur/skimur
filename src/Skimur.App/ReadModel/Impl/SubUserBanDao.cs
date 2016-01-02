using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
{
    public class SubUserBanDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : SubUserBanService, ISubUserBanDao
    {
        public SubUserBanDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
