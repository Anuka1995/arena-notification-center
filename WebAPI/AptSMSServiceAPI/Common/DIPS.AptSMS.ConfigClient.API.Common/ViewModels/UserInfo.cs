using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.ViewModels
{
    public class UserInfo
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }

    public class UserRole
    {
        public long UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
    }
}
