using System;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// List item of Location information
    /// </summary>
    public class LocationListItem
    {
        public long LocationId { get; set; }
        public string LocationDisplayName { get; set; }
        public string UnitGid { get; set; }
    }
}
