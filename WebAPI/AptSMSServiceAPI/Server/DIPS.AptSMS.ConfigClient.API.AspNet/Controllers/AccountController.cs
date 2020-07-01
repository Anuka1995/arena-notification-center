using DIPS.AptSMS.ConfigClient.API.Common.ViewModels;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using DIPS.Infrastructure.Security;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    /// <summary>
    /// Web API functions for Account information and authorization  
    /// </summary>
    [Route("auth/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static readonly ILog  m_log = LogProvider.GetLogger(typeof(AccountController));

        public IConfiguration Configuration { get; }
        private readonly IAccountProvisionService m_accountProvisionService;
        private readonly IUser m_user;

        public AccountController(IAccountProvisionService accountService, IConfiguration configuration, IUser user)
        {
            Configuration = configuration;
            m_accountProvisionService = accountService;
            m_user = user;
        }

        [AllowAnonymous]
        [HttpGet, Route("userroles")]
        public async Task<ActionResult> GetUserRolesAsync(string username, string password)
        {
            m_log.Info("obtaining user roles for username ");
            m_log.DebugFormat("obtaining user roles for username <{0}> ", username);
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                var errorMsg = $"UserName/Password is not provided correctly for: {username}";
                m_log.Info(errorMsg);
                throw new UserInputValidationException(errorMsg);
            }
            try
            {
                var rolesContent = await m_accountProvisionService.GetUserRolesByDipsSignatureAsync(new UserLogOn { UserName = username, Password = password });
                if (rolesContent == null || rolesContent.UserRoles == null)
                {
                    var errorMsg = $"Empty roles returned for: {username}";
                    m_log.Info(errorMsg);
                    throw new Exception(errorMsg);
                }
                var userInfo = new UserInfo()
                {
                    FirstName = rolesContent.User.FirstName,
                    LastName = rolesContent.User.LastName,
                    Username = rolesContent.User.Username,
                    UserRoles = (rolesContent.UserRoles.Select(r => new UserRole 
                    { 
                        UserRoleId = r.UserRoleId,
                        RoleName = r.RoleName,
                        HospitalId = r.HospitalId,
                        HospitalName = r.HospitalName})).ToList()
                };
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error in getting user-roles for the user: {username}";
                m_log.Info(errorMsg);
                m_log.ErrorException(errorMsg, ex);
                throw ex;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("getticket")]
        public async Task<ActionResult<UserSession>> AuthenticateUserAsync(UserLogOn userInformation)
        {
            if (userInformation == null || string.IsNullOrEmpty(userInformation.UserName) || string.IsNullOrEmpty(userInformation.Password) || string.IsNullOrEmpty(userInformation.UserRole))
            {
                var errorMsg   = "User Information is not available to perform authentication for the userName "+ userInformation.UserName;
                m_log.Error(errorMsg);
                throw new UserInputValidationException("User Information is not available to perform authentication!");
            }
            try
            {
                var authEnabled = Configuration.GetValue<bool>("CheckAuthorization");

                var userSession = await m_accountProvisionService.LogOnAndGetTicketAsync(userInformation);

                if (authEnabled)
                {
                    bool isAuthorized = await m_accountProvisionService.LoggedInUserHaveAccessAsync(userSession.Token);

                    if (isAuthorized)
                    {
                        m_log.Trace("auth/getticket SUCCESS");
                        return Ok(userSession);
                    }
                    return Unauthorized();
                }

                return Ok(userSession);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error in create session for the user {userInformation.UserName}";
                m_log.ErrorException(errorMsg, ex);
                throw ex;
            }
        }

        [HttpGet]
        [Route("logOff")]
        public ActionResult LogOff()
        {
            try
            {
                m_accountProvisionService.LogOff();
                m_log.Info("Log Off - SUCCESS");
                return Ok();
            }
            catch (Exception ex)
            {
                m_log.Info("Error occured in logoff user");
                throw new Exception($"Error occured in logoff user. {ex.Message}", ex);
            }
        }

        [HttpPut]
        [Route("poke")]
        public async Task<ActionResult> PokeSession()
        {
            try
            {
                var token = m_user.SecurityToken;
                var updatedSession = await m_accountProvisionService.PokeUserSessionAsync(token);
                m_log.Info($"Poke Session for {m_user.UserName} | {m_user.UserRoleName} - SUCCESS");

                return Ok(updatedSession);
            }
            catch (Exception ex)
            {
                m_log.Info("Error occured in PokeUserSessionAsync user");
                throw ex;
            }
        }
    }

}
