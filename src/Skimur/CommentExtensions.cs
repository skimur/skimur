using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur
{
    public static class CommentExtensions
    {
        public static T As<T>(this object o)
        {
            return (T) o;
        }
    }
}
