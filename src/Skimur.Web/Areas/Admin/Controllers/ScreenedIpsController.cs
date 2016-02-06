using Skimur.App.Services;
using Skimur.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Skimur.Web.Areas.Admin.Controllers
{
    public class ScreenedIpsController : BaseAdminController
    {
        public ScreenedIpsController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public ScreenedIpModel GetById(Guid id)
        {
            return null;
        }
    }
}
