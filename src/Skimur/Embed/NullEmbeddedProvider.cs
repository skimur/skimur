using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Embed
{
    public class NullEmbeddedProvider : IEmbeddedProvider
    {
        public bool IsEnabled { get { return false; } }

        public IEmbeddedResult Embed(string url)
        {
            return null;
        }
    }
}
