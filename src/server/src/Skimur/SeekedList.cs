using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur
{
    public class SeekedList<T> : List<T>
    {
        public SeekedList(IEnumerable<T> source, int skipped, int? taken, long totalCount)
        {
            Skipped = skipped;
            Taken = taken;
            TotalCount = totalCount;
            AddRange(source);
        }

        public SeekedList() { }

        public int Skipped { get; }

        public int? Taken { get; }

        public long TotalCount { get; }

        public bool HasMore => TotalCount > (Skipped + Taken);

        public bool HasPrevious => Skipped > 0;
    }
}
