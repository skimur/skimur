using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public class SubWrapped
    {
        public SubWrapped(Sub sub)
        {
            Sub = sub;
        }

        public Sub Sub { get; private set; }

        public bool IsSubscribed { get; set; }
    }
}
