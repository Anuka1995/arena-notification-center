using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.Models
{
    public class RolesResponse
    {
        public int UserLoginStatus { get; set; }
        public User User { get; set; }
        public List<Role> UserRoles { get; set; }
        public DateTime UserLastLogOnTime { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Role
    {
        public long UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
    }
}
