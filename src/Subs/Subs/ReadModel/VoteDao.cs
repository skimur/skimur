using Infrastructure.Data;
using Subs.Services;

namespace Subs.ReadModel
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
