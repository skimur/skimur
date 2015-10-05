using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Subs.Services;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class KarmaDao :
        // temp until we get a proper caching layer.
        KarmaService, IKarmaDao
    {
        public KarmaDao(ISession session)
            :base(session)
        {
            
        }
    }
}
