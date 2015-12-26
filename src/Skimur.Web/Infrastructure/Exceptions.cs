using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Infrastructure
{
    public class BaseHttpException : Exception
    {
        public BaseHttpException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }

    public class NotFoundException : BaseHttpException
    {
        public NotFoundException()
            : base(HttpStatusCode.NotFound, string.Empty)
        {

        }
    }

    public class UnauthorizedException : BaseHttpException
    {
        public UnauthorizedException()
            : base(HttpStatusCode.Unauthorized, string.Empty)
        {

        }
    }
}
