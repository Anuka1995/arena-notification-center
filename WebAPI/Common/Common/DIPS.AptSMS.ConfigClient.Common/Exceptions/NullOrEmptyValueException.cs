using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Exceptions
{
    public class NullOrEmptyValueException : Exception
    {
        public NullOrEmptyValueException()
        {
        }

        public NullOrEmptyValueException(string message)
            : base(message)
        {
        }

        public NullOrEmptyValueException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
