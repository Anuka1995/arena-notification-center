using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Lkist item Information for SMS test template
    /// </summary>
    public class SMSTextTemplateListItem
    {
        public Guid TextTemplateId { get; set; }
        public String TextTemplateName { get; set; }
        public long OrganizationID { get; set; }
        public long HospitalId { get; set; }
        public String SMSText { get; set; }
        public Guid RuleSetId { get; set; }
    }
}
