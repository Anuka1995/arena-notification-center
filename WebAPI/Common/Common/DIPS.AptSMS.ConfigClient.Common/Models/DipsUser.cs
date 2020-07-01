using System;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Dips Login user information 
    /// </summary>
    public class DipsUser
    {
        public long UserId { get; set; }
        public long UserRole { get; set; }
        public long OrganizationId { get; set; }
    }
}
