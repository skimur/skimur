using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur
{
    public class GuidUtil
    {
        public static Guid NewSequentialId()
        {
            return RT.CombSqlOrder.Comb.Create();
        }
    }
}
