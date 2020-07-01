using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// SMS test inefomation 
    /// </summary>
    public class SMSText
    {
        public Guid? TextTemplateId { get; set; }
        public String TextTemplateName { get; set; }
        public Guid? RulesetId { get; set; }
        public String RuleSetName { get; set; }
        public List<string> EcludedOrgIds { get; set; }
        public int SendSMSBeforeDays { get; set; }
        public string SMSTextTemplate { get; set; }
        public long HospitalId { get; set; }
        public long? DepartmentId { get; set; }
        public long? TemplateDepartmentId { get; set; }
        public long? OPDId { get; set; }
        public long? SectionId { get; set; }
        public long? WardId { get; set; }
        public long? LocationId { get; set; }
        public bool IsVideoAppoinment { get; set; }
        public bool IsGenerateSMS { get; set; }
        public bool isPSSLinkInclude { get; set; }
        public bool isActive { get; set; }
        public bool isLatest { get; set; }
        public List<long> ContactType { get; set; }
        public List<long> OfficialLevelOfCare { get; set; }
        public Guid? GroupTemplateId { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? ValidFrom { get; set; }

    }
}
