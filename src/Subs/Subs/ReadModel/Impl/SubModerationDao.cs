using Skimur;
using Skimur.Data;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class ModerationDao :
        ModerationService, // TOOD: only implement read-only methods
        IModerationDao 
    {
        public ModerationDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
