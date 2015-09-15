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
    public class SubModerationDao :
        SubModerationService, // TOOD: only implement read-only methods
        ISubModerationDao 
    {
        public SubModerationDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
