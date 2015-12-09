using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure.Identity
{
    public class ApplicationLookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            // we don't need to normalize
            return key;
        }
    }
}
