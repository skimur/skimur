using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
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
