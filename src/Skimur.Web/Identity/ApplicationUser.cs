using System;
using Membership;
using Microsoft.AspNet.Identity;

namespace Skimur.Web.Identity
{
    public class ApplicationUser : User, IUser<Guid>
    {

    }
}
