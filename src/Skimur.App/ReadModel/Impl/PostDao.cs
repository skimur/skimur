using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
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
