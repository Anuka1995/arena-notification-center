using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.ViewModels
{
    public class UserSession
    {
        public long? UserRoleId { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public long HospitalId { get; set; }
        public string HospitalName { get; set; }
    }
}
