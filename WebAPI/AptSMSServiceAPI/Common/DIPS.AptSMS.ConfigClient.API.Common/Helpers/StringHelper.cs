using System;

namespace DIPS.AptSMS.ConfigClient.API.Common.Helpers
{
    public static class StringHelper
    {
        public static bool AreEqual(string input1, string input2)
        {
            return input1.Equals(input2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
