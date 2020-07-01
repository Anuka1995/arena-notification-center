using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Common.ViewModels
{
    public class SendingRulesListItem
    {
        public Guid RulesetGuid { get; set; }
        public string RulesetName { get; set; }
        public long DepartmentId { get; set; }
        public long HospitalId { get; set; }

    }
}
