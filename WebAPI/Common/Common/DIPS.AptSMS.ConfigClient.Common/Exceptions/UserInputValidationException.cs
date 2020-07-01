using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Exceptions
{
    public class UserInputValidationException : Exception
    {
        public List<string> EmptyInputs;

        public UserInputValidationException()
        {
        }

        public UserInputValidationException(string message)
            : base(message)
        {
        }

        public UserInputValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public UserInputValidationException(string message, List<string> inputs)
            : base(message)
        {
            EmptyInputs = inputs;
        }

        public UserInputValidationException(string message, Exception inner, List<string> inputs)
            : base(message, inner)
        {
            EmptyInputs = inputs;
        }
    }
}
