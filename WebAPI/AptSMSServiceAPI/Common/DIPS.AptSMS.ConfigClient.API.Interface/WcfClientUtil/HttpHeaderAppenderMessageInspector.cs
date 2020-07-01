using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class HttpHeaderAppenderMessageInspector : IClientMessageInspector
    {
        IDictionary<string, string> m_headers;

        public HttpHeaderAppenderMessageInspector(IDictionary<string, string> headers)
        {
            m_headers = headers ?? new Dictionary<string, string>();
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                AppendHeaders(httpRequestMessage, m_headers);
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                AppendHeaders(httpRequestMessage, m_headers);

                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }

        private void AppendHeaders(HttpRequestMessageProperty httpRequestMessage, IDictionary<string, string> m_headers)
        {
            foreach (var pair in m_headers)
            {
                httpRequestMessage.Headers[pair.Key] = pair.Value;
            }
        }
    }
}
