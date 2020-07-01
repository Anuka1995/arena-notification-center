namespace DIPS.AptSMS.ConfigClient.Common.Server.Extensions
{
    public static class StringExtensions
    {
        public static bool IsWhiteSpace(this string input)
        {
            return !string.IsNullOrEmpty(input) && string.IsNullOrWhiteSpace(input);
        }
    }
}
