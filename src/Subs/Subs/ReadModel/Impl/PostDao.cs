using Infrastructure.Data;
using Subs.Services;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class PostDao
         // this class temporarily implements the service, until we implement the proper read-only layer
        : PostService, IPostDao
    {
        public PostDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
