using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class TicketAppendingMessageInspector : IClientMessageInspector
    {
        private Guid? m_clientTicket;

        public TicketAppendingMessageInspector(Guid? clientTicket)
        {
            m_clientTicket = clientTicket;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (!m_clientTicket.HasValue)
            {
                return null;
            }

            // Old version with ticket in SOAP header
            request.Headers.Add(new ClientTicketHeader(m_clientTicket.Value));

            // New version with ticket in HTTP header
            HttpRequestMessageProperty httpRequestMessage = GetHttpRequestMessageProperty(request);

            if (string.IsNullOrEmpty(httpRequestMessage.Headers["TicketHeader"]))
            {
                httpRequestMessage.Headers["TicketHeader"] = m_clientTicket.ToString();
            }

            return null;
        }

        private HttpRequestMessageProperty GetHttpRequestMessageProperty(Message request)
        {
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                return httpRequestMessageObject as HttpRequestMessageProperty;
            }

            var httpRequestMessage = new HttpRequestMessageProperty();
            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);

            return httpRequestMessage;
        }
    }
}
