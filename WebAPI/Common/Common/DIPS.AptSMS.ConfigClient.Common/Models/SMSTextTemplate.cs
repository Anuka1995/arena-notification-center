using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Information for SMS test template
    /// </summary>
    public class SMSTextTemplate
    {
        public Guid? TextTemplateTextId { get; set; }
        public long? OrganizationID { get; set; }
        public long HospitalID { get; set; }
        public long? DepartmentID { get; set; }
        public String TextTemplateName { get; set; }
        public String TextTemplateString { get; set; }
        public string DepartmentShortName { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? ValidFrom { get; set; }
        public bool IsActive { get; set; }

    }
}
