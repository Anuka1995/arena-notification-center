using System;
using System.Net;
using System.Net.Http;

namespace DIPS.AptSMS.ConfigClient.API.Common.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static void EnsureSuccessStatusCodeAndRethrow(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.Content != null)
                response.Content.Dispose();

            throw new HttpResponseException(response.StatusCode, content);
        }
    }

    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}
