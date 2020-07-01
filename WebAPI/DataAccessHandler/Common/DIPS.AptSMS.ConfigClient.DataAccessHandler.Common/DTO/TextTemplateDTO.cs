using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO
{
    /// <summary>
    /// Table: APSMSTEXT
    /// </summary>
    public class TextTemplateDTO
    {
        public Guid? TemplateGUID { get; set; }

        public Guid? RuleSetGUID { get; set; }

        public Guid? GroupedTextGUID { get; set; }

        public string Name { get; set; }

        public string RuleSetName { get; set; }

        public List<string> ExcludedOrgUnits { get; set; }

        public int SendSMSBeforeDays { get; set; }

        public string SMSText { get; set; }        

        public long HospitalID { get; set; }

        public long? DepartmentID { get; set; }

        public long? TemplateDepartmentID { get; set; }

        public long? OPDID { get; set; }

        public long? LocationID { get; set; }

        public long? SectionID { get; set; }

        public long? WardID { get; set; }

        public List<long> OfficialLevelOfCare { get; set; }

        public List<long> ContactType { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public bool AttachPSSLink { get; set; }

        public bool IsVideoAppoinment { get; set; }

        public bool IsActive { get; set; }

        public bool IsGenerateSMS { get; set; } = true;

        public Guid? ReplacedByGUID { get; set; }
    }
}
