using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.Models
{
   public class CurrentUserInfo
    {
        //public long UserId { get; set; }
        public Guid Ticket { get; set; }
        public long UserRoleId { get; set; }
        public long HospitalId { get; set; }

        public CurrentUserInfo(Guid ticket, long userRoleId, long hospitalId)
        {
            //UserId = userId;
            Ticket = ticket;
            UserRoleId = userRoleId;
            HospitalId = hospitalId;
        }
    }
}
