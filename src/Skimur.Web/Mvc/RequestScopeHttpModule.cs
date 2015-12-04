using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Mvc
{
    public class RequestScopeHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items["Scope"] = SkimurContext.Resolve<IServiceScopeFactory>().CreateScope();
        }

        private void EndRequest(object sender, EventArgs e)
        {
            IServiceScope scope = HttpContext.Current.Items["Scope"] as IServiceScope;
            if (scope != null)
            {
                scope.Dispose();
            }
        }

        public void Dispose()
        {

        }
    }
}
