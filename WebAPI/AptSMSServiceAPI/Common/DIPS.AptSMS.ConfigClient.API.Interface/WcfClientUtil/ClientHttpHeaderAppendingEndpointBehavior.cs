using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class ClientHttpHeaderAppendingEndpointBehavior : IEndpointBehavior
    {
        IDictionary<string, string> m_headers;

        public ClientHttpHeaderAppendingEndpointBehavior(IDictionary<string, string> headers)
        {
            m_headers = headers;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new HttpHeaderAppenderMessageInspector(m_headers));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}
