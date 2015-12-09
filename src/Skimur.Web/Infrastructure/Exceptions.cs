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
        public BaseHttpException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }

    public class NotFoundException : BaseHttpException
    {
        public NotFoundException()
            : base(string.Empty, HttpStatusCode.NotFound)
        {

        }
    }

    public class UnauthorizedException : BaseHttpException
    {
        public UnauthorizedException()
            : base(string.Empty, HttpStatusCode.Unauthorized)
        {

        }
    }
}
