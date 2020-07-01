
using System;
using System.Net.Http;
using System.Threading.Tasks;
using DIPS.Infrastructure.Logging;
using DIPS.AccessControl.Authorization;
using DIPS.AccessControl.Authorization.Constants;
using Microsoft.Extensions.Configuration;
using DIPS.AptSMS.ConfigClient.API.Common.Helpers;
using DIPS.AptSMS.ConfigClient.API.Common.ViewModels;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.API.Common.Models;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class AccountProvisionService : IAccountProvisionService
    {
        private static readonly ILog s_log = LogProvider.For<IAccountProvisionService>();

        private readonly HttpClient m_httpClient;
        private readonly IConfiguration m_configs;

        private const string UserRolesApi = "DIPS-WebAPI/DIPS/Authentication/api/userroles";
        private const string AuthenticationApi = "DIPS-WebAPI/DIPS/Authentication/api/session";

        private readonly long FUNCTIONELEMENTID = 6726;
        private const string AuthorizationApi = V1 + PolicyDecisionPoint;
        private const string V1 = "dips-authorization-service/api/v1.0/";
        private const string PolicyDecisionPoint = "authorization/pdp";

        public AccountProvisionService(HttpClient httpClient, IConfiguration configuration)
        {
            m_httpClient = httpClient;
            m_configs = configuration;
        }

        public async Task<bool> LoggedInUserHaveAccessAsync(string ticket)
        {
            bool accessAllowed = false;

            var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri(AuthorizationApi)
                .AddStringContent(BuildAuthorizationRequest())
                .AddTicketHeader(ticket)
                .Build();

            var response = await m_httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return false;

            if (!response.IsSuccessStatusCode)
            {
                s_log.Error($"LoggedInUserHaveAccessAsync - REST call is not successfull! | BaseUrl: {m_httpClient.BaseAddress}-{AuthorizationApi} | {response.ReasonPhrase} | {response.StatusCode}");
                throw new RESTCallException($"Error in checking access! {response.ReasonPhrase}", response.StatusCode);
            }

            var authoResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(await response.Content.ReadAsStringAsync());

            if (authoResponse.Results.First().Decision == Decision.Permit)
                accessAllowed = true;

            return accessAllowed;
        }

        public async Task<RolesResponse> GetUserRolesByDipsSignatureAsync(UserLogOn userInformation)
        {
            var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Get)
                .AddRequestUri(UserRolesApi)
                .AddBasicAuthentication(userInformation.UserName, userInformation.Password)
                .Build();

            var response = await m_httpClient.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                s_log.Error($"GetUserRolesByDipsSignatureAsync - REST call is not successfull! | BaseUrl: {m_httpClient.BaseAddress}-{UserRolesApi} | {response.ReasonPhrase}");
                throw new RESTCallException($"Failed to load user-roles. Invalid credential.", response.StatusCode);
            }

            if (!response.IsSuccessStatusCode)
            {
                s_log.Error($"GetUserRolesByDipsSignatureAsync - REST call is not successfull! | BaseUrl: {m_httpClient.BaseAddress}-{UserRolesApi} | {response.ReasonPhrase}");
                throw new RESTCallException($"Failed to load user-roles.", response.StatusCode);
            }

            return await response.ContentAs<RolesResponse>();
        }

        public void LogOff()
        {
            throw new NotImplementedException("SB Authentication service does not have logout endpoint.");
        }

        public async Task<UserSession> LogOnAndGetTicketAsync(UserLogOn userInformation)
        {
            var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri(AuthenticationApi)
                .AddJsonContent(new SessionRequest { UserRoleId = long.Parse(userInformation.UserRole) })
                .AddBasicAuthentication(userInformation.UserName, userInformation.Password)
                .Build();

            var response = await m_httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new AccessDeniedException("Access Denied!");

            if (!response.IsSuccessStatusCode)
            {
                s_log.Error($"LogOnAndGetTicketAsync - REST call FAILED! | BaseUrl: {m_httpClient.BaseAddress}-{AuthenticationApi} | {response.ReasonPhrase} || {response.StatusCode}");
                throw new RESTCallException($"Create session failed! ({response.ReasonPhrase})", response.StatusCode);

            }

            return await response.ContentAs<UserSession>();
        }

        public async Task<UserSession> PokeUserSessionAsync(Guid securityToken)
        {
            s_log.Info($"Poke session");

            var request = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Put)
                .AddRequestUri(AuthenticationApi)
                .AddJsonContent(new PokeRequest { Token = securityToken.ToString() })
                .Build();

            var response = await m_httpClient.SendAsync(request);

            switch (response.StatusCode)
            {
                // If the session is expired or unknown, the server will respond with an http 404 Not Found message
                case HttpStatusCode.NotFound:
                    throw new AccessDeniedException("Session Expired!");

                case HttpStatusCode.Unauthorized:
                    throw new AccessDeniedException("Access Denied!");
            }

            if (!response.IsSuccessStatusCode)
            {
                s_log.Error($"PokeUserSessionAsync - REST call FAILED! | BaseUrl: {m_httpClient.BaseAddress}-{AuthenticationApi} | {response.ReasonPhrase}");
                throw new RESTCallException($"Poke failed! {response.ReasonPhrase}", response.StatusCode);
            }

            return await response.ContentAs<UserSession>();
        }

        #region Private-Section

        // Note: the order of the results in AuthorizationResponse corresponds to the order of the requests.
        private List<AuthorizationRequest> BuildAuthorizationRequest()
        {
            var authorizationRequest = new AuthorizationRequest();

            authorizationRequest.AddTargetNamespace(FunctionResourceAttributes.Namespace);
            authorizationRequest.AddResource(XacmlAttributes.ResourceId, FUNCTIONELEMENTID);

            var authorizationRequestList = new List<AuthorizationRequest>();
            authorizationRequestList.Add(authorizationRequest);
            return authorizationRequestList;
        }
        #endregion
    }
}
