using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Controllers
{
    public class PoliciesController : BaseController
    {
        public ActionResult PrivacyPolicy()
        {
            ViewBag.ManageNavigationKey = "privacypolicy";
            return View();
        }

        public ActionResult UserAgreement()
        {
            ViewBag.ManageNavigationKey = "useragreement";
            return View();
        }

        public ActionResult ContentPolicy()
        {
            ViewBag.ManageNavigationKey = "contentpolicy";
            return View();
        }
    }
}
