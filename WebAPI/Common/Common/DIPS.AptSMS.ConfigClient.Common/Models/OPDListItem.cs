using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// List item of OPD List (poly Clinic)
    /// </summary>
    public class OPDListItem
    {
        public long OPDID { get; set; }
        public string OPDDisplayName { get; set; }
        public string UnitGid { get; set; }
        public List<LocationListItem> LocationList { get; set; }
    }
}

