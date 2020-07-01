using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public interface IWcfClient
    {
        void CallService<TService>(
            Action<TService> serviceAction,
            string serviceUrl,
            IDictionary<string, string> extraHeaders,
            Guid? ticket = null,
            Binding binding = null,
            IEnumerable<IEndpointBehavior> endpointBehaviors = null);

        void CallService<TService>(
            Action<TService> serviceAction,
            string serviceUrl,
            Guid? ticket = null,
            Binding binding = null);

        TReturn CallServiceAndReturnResult<TService, TReturn>(
            Func<TService, TReturn> action,
            string serviceUrl,
            Guid? ticket = null,
            Binding binding = null);

        TReturn CallServiceAndReturnResult<TService, TReturn>(
            Func<TService, TReturn> action,
            string serviceUrl,
            IDictionary<string, string> extraHeaders,
            Guid? ticket = null,
            Binding binding = null,
            IEnumerable<IEndpointBehavior> endpointBehaviors = null);
    }
}
