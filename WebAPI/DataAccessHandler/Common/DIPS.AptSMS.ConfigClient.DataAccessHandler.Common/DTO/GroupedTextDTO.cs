using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO
{

    /// <summary>
    /// Table: APSMSTEXTTEMPLATE
    /// </summary>
    public class GroupedTextDTO
    {
        public Guid? GroupedTempateGUID { get; set; }

        public long? OrganizationID { get; set; }

        public long HospitalID { get; set; }

        public long? DepartmentID { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public Guid ReplacedByGUID { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime? ValidFrom { get; set; }

        public bool IsActive { get; set; }
        public bool HospitalOnly { get; set; }
    }
}
