using DIPS.AptSMS.ConfigClient.API.Common.Models;
using DIPS.AptSMS.ConfigClient.API.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IAccountProvisionService
    {
        Task<RolesResponse> GetUserRolesByDipsSignatureAsync(UserLogOn userInformation);

        Task<UserSession> LogOnAndGetTicketAsync(UserLogOn userInformation);

        Task<bool> LoggedInUserHaveAccessAsync(string ticket);

        void LogOff();

        Task<UserSession> PokeUserSessionAsync(Guid securityToken);
    }
}
