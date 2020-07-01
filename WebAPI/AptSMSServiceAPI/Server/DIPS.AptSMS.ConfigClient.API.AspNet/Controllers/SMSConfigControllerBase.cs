using DIPS.AptSMS.ConfigClient.API.Common.Models;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{

    public class SMSConfigControllerBase : ControllerBase
    {
        private readonly IUser m_currentUser;

        public SMSConfigControllerBase(IUser user)
        {
            m_currentUser = user;
        }

        public CurrentUserInfo GetCurrentUserInfo()
        {
            if (m_currentUser == null)
            {
                throw new Exception("Failed to load the currunt user from the TicketHeader.");
            }

            return new CurrentUserInfo(m_currentUser.SecurityToken, m_currentUser.UserRoleId, m_currentUser.HospitalId);
        }
    }
}
