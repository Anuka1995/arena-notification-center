using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.Models
{
    public class SessionRequest
    {
        public long UserRoleId { get; set; }
        public string TerminalName { get; set; }
    }

    public class PokeRequest
    {
        public string Token { get; set; }
    }
}
