using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Skimur.Web.Mvc
{
    /// <summary>
    /// This attribute is used to indicate that this MVC action is an ajax/JSON request.
    /// </summary>
    public class AjaxAttribute : FilterAttribute
    {
    }
}
