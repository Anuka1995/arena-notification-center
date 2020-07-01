using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// List item of RuleSet
    /// </summary>
    public class RuleSetListItem
    {
        public Guid RulesetId { get; set; }
        public string RulesetName { get; set; }
        public long? DepartmentId { get; set; }
        public long HospitalId { get; set; }
        public bool IsActive { get; set; }
    }
}
