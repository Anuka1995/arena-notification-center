
using DIPS.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public class WcfConfigurationProvider : IWcfConfigurationProvider
    {      
        public const string UseHttpsSetting = "UseHttps";
        private const string WCFContextPath = "OrganizationWcfServer";
        private const string OrganizationServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-IOrganizationService.svc";
        private const string HospitalServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-IHospitalService.svc";
        private const string DepartmentServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-IDepartmentService.svc";
        private const string WardServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-IWardService.svc";
        private const string SectionServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-ISectionService.svc";
        private const string LocationServiceUrlSettingValue = "DIPS-OrganizationalUnits-Interface-ILocationService.svc";
        private const string OrganizationServerUrlSetting = "OrganizationServerUrl";


        private static readonly ILog s_log = LogProvider.GetLogger(typeof(WcfConfigurationProvider));

        private readonly IConfiguration m_configuration;

        private string m_fullDepartmentServiceUrl;

        private string m_fullHospitalServiceUrl;

        private string m_fullLocationServiceUrl;

        private string m_fullOrganizationServiceUrl;

        private string m_fullSectionServiceUrl;

        private string m_fullWardServiceUrl;


        public WcfConfigurationProvider(IConfiguration configuration)
        {
            m_configuration = configuration;
        }

        public string FullOrganizationServiceUrl
        {
            get
            {
                if (m_fullOrganizationServiceUrl == null)
                {
                    var orgnizationServerURL = m_configuration.GetValue<string>("OrganizationServerUrl");
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;

                    var serviceUrl = OrganizationServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for EncounterService from: {OrganizationServiceUrlSettingValue}");

                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullOrganizationServiceUrl = url;

                    s_log.Debug("Url for OrganizationService:" + m_fullOrganizationServiceUrl);
                }

                return m_fullOrganizationServiceUrl;
            }
        }

        public string FullHospitalServiceUrl
        {
            get
            {
                if (m_fullHospitalServiceUrl == null)
                {

                    var orgnizationServerURL = m_configuration.GetValue<string>("OrganizationServerUrl");
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;
                    
                    var serviceUrl = HospitalServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for HospitalService from: {HospitalServiceUrlSettingValue}");

                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullHospitalServiceUrl = url;

                    s_log.Debug("Url for EncounterService:" + m_fullHospitalServiceUrl);
                }

                return m_fullHospitalServiceUrl;
            }
        }

        public string FullDepartmentServiceUrl
        {
            get
            {
                if (m_fullDepartmentServiceUrl == null)
                {
                    var orgnizationServerURL = m_configuration.GetValue<string>(OrganizationServerUrlSetting);
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;

                    var serviceUrl = DepartmentServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for DepartmentService from: {DepartmentServiceUrlSettingValue}");
                
                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullDepartmentServiceUrl = url;

                    s_log.Debug("Url for DepartmentService:" + m_fullDepartmentServiceUrl);
                }

                return m_fullDepartmentServiceUrl;
            }
        }

        public string FullWardServiceUrl
        {
            get
            {
                if (m_fullWardServiceUrl == null)
                {
                    var orgnizationServerURL = m_configuration.GetValue<string>("OrganizationServerUrl");
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;
                 
                    var serviceUrl = WardServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for WardService from: {WardServiceUrlSettingValue}");

                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullWardServiceUrl = url;

                    s_log.Debug("Url for WardService:" + m_fullWardServiceUrl);
                }

                return m_fullWardServiceUrl;
            }
        }

        public string FullSectionServiceUrl
        {
            get
            {
                if (m_fullSectionServiceUrl == null)
                {
                    var orgnizationServerURL = m_configuration.GetValue<string>("OrganizationServerUrl");
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;

                    var serviceUrl = SectionServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for SectionService from: {SectionServiceUrlSettingValue}");

                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullSectionServiceUrl = url;

                    s_log.Debug("Url for SectionService:" + m_fullSectionServiceUrl);
                }

                return m_fullSectionServiceUrl;
            }
        }

        public string FullLocationServiceUrl
        {
            get
            {
                if (m_fullLocationServiceUrl == null)
                {
                    var orgnizationServerURL = m_configuration.GetValue<string>("OrganizationServerUrl");
                    ThrowIfEmpty(orgnizationServerURL, OrganizationServerUrlSetting);
                    var server = orgnizationServerURL.EndsWith("/") || WCFContextPath.StartsWith("/") ? orgnizationServerURL + WCFContextPath : orgnizationServerURL + "/" + WCFContextPath;

                    var serviceUrl = LocationServiceUrlSettingValue;
                    var useHttps = m_configuration.GetValue(UseHttpsSetting, true);

                    s_log.Debug($"Build url for LocationService from: {LocationServiceUrlSettingValue}");

                    string url;
                    if (IsAbsoluteUri(serviceUrl))
                    {
                        url = serviceUrl;
                    }
                    else
                    {
                        // Use full local hostname as default since we bundle the server with FHIR on the same machine.
                        // Using 'localhost' would not work since https certificates are set to the real hostname.
                        if (string.IsNullOrEmpty(server))
                        {
                            server = LocalHostName();
                        }

                        server = PrependHttp(server, useHttps);

                        url = server.EndsWith("/") || serviceUrl.StartsWith("/") ? server + serviceUrl : server + "/" + serviceUrl;
                    }

                    m_fullLocationServiceUrl = url;

                    s_log.Debug("Url for LocationService:" + m_fullLocationServiceUrl);
                }

                return m_fullLocationServiceUrl;
            }
        }

        private static string LocalHostName()
        {
            var domain = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var host = Dns.GetHostName();

            return $"{host}.{domain}";
        }

        private string PrependHttp(string server, bool useHttps)
        {
            if (Regex.IsMatch(server, "^[^:]+://"))
            {
                return server;
            }

            return useHttps ? "https://" + server : "http://" + server;
        }

        private bool IsAbsoluteUri(string encounterServiceUrl)
        {
            var uri = new Uri(new Uri("xxxx://yyyy"), encounterServiceUrl);
            return uri.Host != "yyyy" && uri.Scheme != "xxxx";
        }

        private static void ThrowIfEmpty(string value, string key)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"Failed read configuration: {key} is empty.");
            }
        }
    }
}
