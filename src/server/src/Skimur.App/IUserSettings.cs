using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App
{
    public interface IUserSettings
    {
        bool ShowNsfw { get; set; }
    }
}
