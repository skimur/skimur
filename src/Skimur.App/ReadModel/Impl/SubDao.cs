using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
{
    public class SubDao 
        // this class temporarily implements the service, until we implement the proper read-only layer
        : SubService, ISubDao
    {
        public SubDao(IDbConnectionProvider conn, IMapper mapper)
            :base(conn, mapper)
        {
            
        }
    }
}
