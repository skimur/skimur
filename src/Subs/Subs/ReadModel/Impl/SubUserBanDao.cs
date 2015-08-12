using Infrastructure.Data;
using Subs.Services;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
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
