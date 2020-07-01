using System;

namespace DIPS.AptSMS.ConfigClient.API.Common.Helpers
{
    public class UriHelper
    {
        public static string Combine(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return baseUrl;
            }

            return string.Join("/", baseUrl.TrimEnd('/', '#'), path.TrimStart('/'));
        }

        public static Uri Combine(Uri baseUri, string path)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            // NOTE: Combine using string because `new Uri(new Uri(base), path)` will strip the relative path present in base:
            // http://example.com/my/path + more/ -> http://example.com/my/more/
            return new Uri(Combine(baseUri.ToString(), path));
        }
    }
}
