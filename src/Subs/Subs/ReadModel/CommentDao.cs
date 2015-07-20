using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Subs.Services;

namespace Subs.ReadModel
{
    public class CommentDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : CommentService, ICommentDao
    {
        public CommentDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
