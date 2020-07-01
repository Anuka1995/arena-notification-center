using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class ClientEndpointBehavior : IEndpointBehavior
    {
        private readonly Guid? m_clientTicket;

        public ClientEndpointBehavior(Guid? clientTicket)
        {
            m_clientTicket = clientTicket;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new TicketAppendingMessageInspector(m_clientTicket));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}
