using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web.Controllers
{
    public class PagesController : BaseController
    {
        public ActionResult About()
        {
            return View();
        }
    }
}
