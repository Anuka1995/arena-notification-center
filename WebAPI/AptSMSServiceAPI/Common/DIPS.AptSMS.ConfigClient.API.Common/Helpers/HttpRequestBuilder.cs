using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.Helpers
{
    public class HttpRequestBuilder
    {
        private HttpMethod method = null;
        private string requestUri = "";
        private HttpContent content = null;
        private string bearerToken = "";
        private string basicAuthHeader = "";
        private string authTicket = "";
        private string acceptHeader = "application/json";
        private TimeSpan timeout = new TimeSpan(0, 0, 15);
        private bool allowAutoRedirect = false;
        private string ticketHeader = "";

        private Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        public HttpRequestBuilder()
        {
        }

        public HttpRequestBuilder AddMethod(HttpMethod method)
        {
            this.method = method;
            return this;
        }

        public HttpRequestBuilder AddRequestUri(string requestUri)
        {
            this.requestUri = requestUri;
            return this;
        }

        public HttpRequestBuilder AddQueryParameter(string name, string value)
        {
            queryParameters.Add(name, value);
            return this;
        }

        public HttpRequestBuilder AddContent(HttpContent content)
        {
            this.content = content;
            return this;
        }

        public HttpRequestBuilder AddJsonContent<T>(T content)
        {
            this.content = new JsonContent(content);
            return this;
        }

        public HttpRequestBuilder AddStringContent<T>(T content)
        {
            this.content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return this;
        }

        public HttpRequestBuilder AddBearerToken(string bearerToken)
        {
            this.bearerToken = bearerToken;
            return this;
        }

        public HttpRequestBuilder AddAuthTicket(string ticket)
        {
            this.authTicket = ticket;
            return this;
        }

        public HttpRequestBuilder AddTicketHeader(string ticket)
        {
            this.ticketHeader = ticket;
            return this;
        }

        public HttpRequestBuilder AddBasicAuthentication(string username, string password)
        {
            var basicAuthHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password)));
            this.basicAuthHeader = basicAuthHeader;
            return this;
        }

        public HttpRequestBuilder AddAcceptHeader(string acceptHeader)
        {
            this.acceptHeader = acceptHeader;
            return this;
        }

        public HttpRequestBuilder AddTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public HttpRequestBuilder AddAllowAutoRedirect(bool allowAutoRedirect)
        {
            this.allowAutoRedirect = allowAutoRedirect;
            return this;
        }

        public HttpRequestMessage Build()
        {
            // Check required arguments
            EnsureArguments();

            // Set up request
            var request = new HttpRequestMessage
            {
                Method = this.method,
            };


            if (queryParameters.Count > 0)
            {
                var urlWithQueryParameters = QueryHelpers.AddQueryString(this.requestUri, queryParameters);
                request.RequestUri = new Uri(urlWithQueryParameters, UriKind.Relative);
            }
            else
            {
                request.RequestUri = new Uri(this.requestUri, UriKind.Relative);
            }

            if (this.content != null)
            {
                request.Content = this.content;
            }


            if (!string.IsNullOrEmpty(this.bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.bearerToken);
            }

            if (!string.IsNullOrEmpty(this.basicAuthHeader))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", this.basicAuthHeader);
            }

            if (!string.IsNullOrEmpty(this.ticketHeader))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("TicketHeader", this.authTicket);
                request.Headers.Add("TicketHeader", this.authTicket);
            }

            if (!string.IsNullOrEmpty(this.authTicket))
            {
                //request.Headers.Authorization = new AuthenticationHeaderValue("TicketHeader", this.authTicket);
                request.Headers.Add("TicketHeader", this.authTicket);
                request.Headers.Add("Auth-Ticket", this.ticketHeader);
            }

            request.Headers.Accept.Clear();
            if (!string.IsNullOrEmpty(this.acceptHeader))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(this.acceptHeader));
            }

            return request;

        }

        #region Private-Section

        private void EnsureArguments()
        {
            if (this.method == null)
                throw new ArgumentNullException("Method");

            if (string.IsNullOrEmpty(this.requestUri))
                throw new ArgumentNullException("Request Uri");
        }

        #endregion
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object value)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8,
            "application/json")
        {
        }

        public JsonContent(object value, string mediaType)
            : base(JsonConvert.SerializeObject(value), Encoding.UTF8, mediaType)
        {
        }
    }
}

