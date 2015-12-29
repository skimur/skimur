using Cassandra;
using Skimur.App.Services.Impl;

namespace Skimur.App.ReadModel.Impl
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
