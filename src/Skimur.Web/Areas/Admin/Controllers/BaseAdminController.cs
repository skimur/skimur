using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Skimur.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class BaseAdminController : BaseController
    {
    }
}
