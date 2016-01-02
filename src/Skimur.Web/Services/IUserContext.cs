using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App;

namespace Skimur.Web.Services
{
    public interface IUserContext
    {
        User CurrentUser { get; }

        bool? CurrentNsfw { get; }
    }
}
