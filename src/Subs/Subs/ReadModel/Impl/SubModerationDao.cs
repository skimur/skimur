using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Skimur;
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
