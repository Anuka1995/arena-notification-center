using DIPS.Infrastructure.Profiling;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class ClientProfilingEndpointBehavior : IEndpointBehavior
    {
        private readonly IProfilerStorage m_profilerStorage;

        public ClientProfilingEndpointBehavior(IProfilerStorage profilerStorage)
        {
            m_profilerStorage = profilerStorage;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new ClientProfilingMessageInspector(m_profilerStorage));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }
}
