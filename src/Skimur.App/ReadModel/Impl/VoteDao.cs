using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
{
    public class VoteDao
         // this class temporarily implements the service, until we implement the proper read-only layer
        : VoteService, IVoteDao
    {
        public VoteDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
