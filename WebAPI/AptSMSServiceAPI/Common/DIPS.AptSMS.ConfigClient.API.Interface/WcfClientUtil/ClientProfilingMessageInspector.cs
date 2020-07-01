using DIPS.Infrastructure.Profiling;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class ClientProfilingMessageInspector : IClientMessageInspector
    {
        private IProfilerStorage m_profilerStorage;

        public ClientProfilingMessageInspector(IProfilerStorage profilerStorage)
        {
            m_profilerStorage = profilerStorage;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (m_profilerStorage != null)
            {
                HttpRequestMessageProperty httpRequestMessage = GetHttpRequestMessageProperty(request);

                //if (string.IsNullOrEmpty(httpRequestMessage.Headers["X-Profiling"]))
                //{
                //    httpRequestMessage.Headers["X-Profiling"] = m_profilerStorage?.Peek()?.ToString();
                //}
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
