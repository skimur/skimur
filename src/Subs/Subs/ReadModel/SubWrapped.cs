using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public class SubWrapped
    {
        private int? _fuzzedNumberOfSubscribers;

        public SubWrapped(Sub sub)
        {
            Sub = sub;
        }

        public Sub Sub { get; private set; }

        public bool IsSubscribed { get; set; }

        public int NumberOfSubscribers
        {
            get
            {
                if (_fuzzedNumberOfSubscribers.HasValue)
                    return _fuzzedNumberOfSubscribers.Value;
                return Sub.NumberOfSubscribers;
            }
        }

        public void FuzzNumberOfSubscribers(int fuzzed)
        {
            if (_fuzzedNumberOfSubscribers.HasValue)
                return;

            _fuzzedNumberOfSubscribers = fuzzed;
        }
    }
}
