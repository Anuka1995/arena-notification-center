using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO
{
    public class RuleSetDTO
    {
        public Guid? RuleSetGUID { get; set; }

        public string Name { get; set; }

        public long HospitalID { get; set; }

        public long? DepartmentID { get; set; }

        public bool isValidateAptTime { get; set; }
        public string AptValidate_From { get; set; }
        public string AptValidate_To { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public string SendingTimeWindowFrom { get; set; }
        public string SendingTimeWindowTo { get; set; }

        public int? DaysForRetryExpiry { get; set; }

        public int SendSMSBeforeDays { get; set; }

        public int? SendSMSBeforeInMins { get; set; }

        public List<string> ExcludingOrgUnitIDs { get; set; }

        public bool IsActive { get; set; }

        public bool IgnoreSMStoAdmittedPatient { get; set; }

        public Guid? ReplacedByGUID { get; set; }

    }
}