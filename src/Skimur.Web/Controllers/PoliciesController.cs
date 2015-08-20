using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web.Controllers
{
    public class PoliciesController : BaseController
    {
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult UserAgreement()
        {
            return View();
        }

        public ActionResult ContentPolicy()
        {
            return View();
        }
    }
}
