using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterBasedOnFormNameAttribute : ActionFilterAttribute
    {
        private readonly string _name;
        private readonly string _actionParameterName;

        public ParameterBasedOnFormNameAttribute(string name, string actionParameterName)
        {
            _name = name;
            _actionParameterName = actionParameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.ActionArguments[_actionParameterName] = context.HttpContext.Request.Form.ContainsKey(_name);
            base.OnActionExecuting(context);
        }
    }
}
