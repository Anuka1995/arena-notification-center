using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    /// <summary>
    /// Ruleset object
    /// </summary>
    public class RuleSet
    {
        public Guid? RulesetId { get; set; }
        public long? DepartmentId { get; set; }
        public string RulesetName { get; set; }
        public long HospitalId { get; set; }
        public int SendBeforeInDays { get; set; }
        public int? MinimumTimeFromMinutes { get; set; }
        public int? ExpireInDays { get; set; }
        public bool IsAppointmentTimeValidate { get; set; }
        public DateTime? ScheduleValidityPeriodTo { get; set; }
        public DateTime? ScheduleValidityPeriodFrom { get; set; }
        //TimeStamp convertion is not availabel in react so get as String .Can convert to timeStamp by using TimeStamp.Parse("")
        public String AppointmentTo { get; set; }
        //TimeStamp convertion is not availabel in react so get as String .Can convert to timeStamp by using TimeStamp.Parse("")
        public String AppointmentFrom { get; set; }
        //TimeStamp convertion is not availabel in react so get as String .Can convert to timeStamp by using TimeStamp.Parse("")
        public String SendingTimeIntervalFrom { get; set; }
        //TimeStamp convertion is not availabel in react so get as String .Can convert to timeStamp by using TimeStamp.Parse("")
        public String SendingTimeIntervalTo { get; set; }
        public bool IgnoreSMStoAdmittedPatient { get; set; }
        public List<OrgUnit> ExcludeOrgUnits { get; set; }
        public bool IsLatest { get; set; }
        public bool IsActive { get; set; }
		public string AvoidOrgUnitIds {get; set;}

    }
}
