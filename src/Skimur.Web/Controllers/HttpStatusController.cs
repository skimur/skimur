using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Controllers
{
    public class HttpStatusController : BaseController
    {
        public ActionResult Error()
        {
            return View("HttpStatus/500");
        }

        public ActionResult UnAuthorized()
        {
            return View("HttpStatus/401");
        }

        public ActionResult NotFound()
        {
            return View("HttpStatus/404");
        }

        public ActionResult Unknown()
        {
            return View("HttpStatus/Unknown");
        }
    }
}
