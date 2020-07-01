using System;
using System.Net;

namespace DIPS.AptSMS.ConfigClient.Common.Exceptions
{
    public class RESTCallException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public RESTCallException()
        {
        }

        public RESTCallException(string message, HttpStatusCode errorCode)
            : base(message)
        {
            HttpStatusCode = errorCode;
        }

        public RESTCallException(string message, HttpStatusCode errorCode, Exception inner)
            : base(message, inner)
        {
            HttpStatusCode = errorCode;
        }
    }
}
