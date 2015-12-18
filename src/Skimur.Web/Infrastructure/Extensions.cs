using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Http.Features;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public static class Extensions
    {
        public static string RemoteAddress(this HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
        }

        public static string RawUrl(this HttpRequest httpRequest)
        {
            return httpRequest.Path + httpRequest.QueryString;
        }

        public static T Get<T>(this ISession session, string key)
        {
            var result = session.GetString(key);

            if (string.IsNullOrEmpty(result))
                return default(T);

            return JsonConvert.DeserializeObject<T>(result);
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                session.Remove(key);
            }
            else
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
        }
    }
}
