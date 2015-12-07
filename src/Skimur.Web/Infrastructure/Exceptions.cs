using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Skimur.Web.Infrastructure
{
    public abstract class BaseHttpExceptionException : HttpException
    {
        protected BaseHttpExceptionException(HttpStatusCode httpCode)
            : base((int)httpCode, string.Empty)
        {

        }
    }

    public class NotFoundException : BaseHttpExceptionException
    {
        public NotFoundException()
            : base(HttpStatusCode.NotFound)
        {

        }
    }

    public class UnauthorizedException : BaseHttpExceptionException
    {
        public UnauthorizedException()
            : base(HttpStatusCode.Unauthorized)
        {

        }
    }
}
