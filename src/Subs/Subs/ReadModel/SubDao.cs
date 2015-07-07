using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Data;
using ServiceStack.OrmLite;

namespace Subs.ReadModel
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
