using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Profiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;


namespace DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil
{
    public class WcfClient : IWcfClient
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(WcfClient));
        private static readonly Binding s_defaultBinding = new BasicHttpBinding(BasicHttpSecurityMode.None) { MaxBufferPoolSize = int.MaxValue, MaxBufferSize = int.MaxValue, MaxReceivedMessageSize = int.MaxValue };
        private static readonly Binding s_defaultHttpsBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxBufferPoolSize = int.MaxValue, MaxBufferSize = int.MaxValue, MaxReceivedMessageSize = int.MaxValue };

        private readonly IProfilerStorage m_profilerStorage;

        public WcfClient()
        { }

        public WcfClient(IProfilerStorage profilerStorage)
        {
            m_profilerStorage = profilerStorage;
        }

        public void CallService<TService>(Action<TService> serviceAction, string serviceUrl, Guid? ticket = null, Binding binding = null)
        {
            CallService(serviceAction, serviceUrl, null, ticket, binding);
        }

        public void CallService<TService>(Action<TService> serviceAction,
            string serviceUrl,
            IDictionary<string, string> extraHeaders,
            Guid? ticket = null,
            Binding binding = null,
            IEnumerable<IEndpointBehavior> endpointBehaviors = null)
        {
            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentException("Invalid service URL");
            }

            s_log.Trace($"One way CallService on service type {typeof(TService)}");
            s_log.Trace($"One way Service URL {serviceUrl}");

            var selectedBinding = SelectBinding(binding, serviceUrl);
            var endpoint = new EndpointAddress(serviceUrl);
            var factory = new ChannelFactory<TService>(selectedBinding, endpoint);
            var address = new EndpointAddress(serviceUrl);

            s_log.Trace($"Using ticket {ticket}");
            factory.Endpoint.EndpointBehaviors.Add(new ClientEndpointBehavior(ticket));
            factory.Endpoint.EndpointBehaviors.Add(new ClientHttpHeaderAppendingEndpointBehavior(extraHeaders));
            factory.Endpoint.EndpointBehaviors.Add(new ClientProfilingEndpointBehavior(m_profilerStorage));

            if (endpointBehaviors != null)
            {
                var behaviors = endpointBehaviors.Where(beh => beh != null).ToList();
                s_log.Trace(() => $"Adding behaviors: {string.Join("|", behaviors.Select(beh => beh.GetType().ToString()))}");

                foreach (var behavior in behaviors)
                {
                    factory.Endpoint.EndpointBehaviors.Add(behavior);
                }
            }

            var stopwatch = new Stopwatch();

            try
            {
                s_log.Trace("factory.Open");
                factory.Open();

                s_log.Trace("CreateChannel");
                var channel = factory.CreateChannel(address);

                s_log.Trace("Making call on channel");
                stopwatch.Start();
                serviceAction(channel);
                stopwatch.Stop();
                s_log.Trace($"Channel call complete, elapsed time: {stopwatch.Elapsed}");

            }
            catch (FaultException exception)
            {
                stopwatch.Stop();
                s_log.Trace($"Channel call raised FaultException, elapsed time: {stopwatch.Elapsed}", exception);
                throw;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                s_log.Error($"Error executing service action, elapsed time: {stopwatch.Elapsed}", exception);
                throw;
            }
            finally
            {
                try
                {
                    if (factory.State == CommunicationState.Opened)
                    {
                        s_log.Trace("factory.Close()");
                        factory.Close();
                        s_log.Trace("Factory Closed");
                    }
                }
                catch (Exception e)
                {
                    s_log.Debug("Factory Close() failed", e);
                }
            }

        }

        public TReturn CallServiceAndReturnResult<TService, TReturn>(Func<TService, TReturn> action, string serviceUrl, Guid? ticket = null, Binding binding = null)
        {
            return CallServiceAndReturnResult(action, serviceUrl, null, ticket, binding);
        }

        public TReturn CallServiceAndReturnResult<TService, TReturn>(
            Func<TService, TReturn> action,
            string serviceUrl,
            IDictionary<string, string> extraHeaders,
            Guid? ticket = null,
            Binding binding = null,
            IEnumerable<IEndpointBehavior> endpointBehaviors = null)
        {
            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentException("Invalid service URL");
            }

            s_log.Trace($"CallServiceAndReturnResult {typeof(TService)}, return type {typeof(TReturn)}");
            s_log.Trace($"Service URL {serviceUrl}");

            var selectedBinding = SelectBinding(binding, serviceUrl);
            var endpoint = new EndpointAddress(serviceUrl);
            var factory = new ChannelFactory<TService>(selectedBinding, endpoint);
            var address = new EndpointAddress(serviceUrl);

            s_log.Trace($"Using ticket {ticket}");
            factory.Endpoint.EndpointBehaviors.Add(new ClientEndpointBehavior(ticket));
            factory.Endpoint.EndpointBehaviors.Add(new ClientHttpHeaderAppendingEndpointBehavior(extraHeaders));
            factory.Endpoint.EndpointBehaviors.Add(new ClientProfilingEndpointBehavior(m_profilerStorage));

            if (endpointBehaviors != null)
            {
                var behaviors = endpointBehaviors.Where(beh => beh != null).ToList();
                s_log.Trace(() => $"Adding behaviors: {string.Join("|", behaviors.Select(beh => beh.GetType().ToString()))}");

                foreach (var behavior in behaviors)
                {
                    factory.Endpoint.EndpointBehaviors.Add(behavior);
                }
            }

            var stopwatch = new Stopwatch();

            try
            {
                s_log.Trace("factory.Open");
                factory.Open();

                s_log.Trace("CreateChannel");
                var channel = factory.CreateChannel(address);

                s_log.Debug(string.Format("Calling service: " + serviceUrl));
                stopwatch.Start();
                var ret = action(channel);
                stopwatch.Stop();
                s_log.Debug($"Service call complete, elapsed time: {stopwatch.Elapsed}");

                return ret;
            }
            catch (FaultException exception)
            {
                stopwatch.Stop();
                s_log.Debug($"Service call raised FaultException, elapsed time: {stopwatch.Elapsed}", exception);
                throw;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                s_log.Error($"Error executing call on channel, elapsed time: {stopwatch.Elapsed}", exception);
                throw;
            }
            finally
            {
                try
                {
                    if (factory.State == CommunicationState.Opened)
                    {
                        s_log.Trace("factory.Close()");
                        factory.Close();
                        s_log.Trace("Factory Closed");
                    }
                }
                catch (Exception e)
                {
                    s_log.Debug("Factory Close() failed", e);
                }
            }
        }

        private static Binding SelectBinding(Binding binding, string serviceUrl)
        {
            if (binding != null)
            {
                s_log.Trace("Binding to use: " + binding.GetType().Name);
                return binding;
            }
            if (serviceUrl != null && serviceUrl.StartsWith("https:"))
            {
                s_log.Trace("Binding to use: Default https binding");
                return s_defaultHttpsBinding;
            }
            s_log.Trace("Binding to use: Default binding");
            return s_defaultBinding;
        }
    }
}












