using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string message)
            : base(message)
        {
        }

        public AccessDeniedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
