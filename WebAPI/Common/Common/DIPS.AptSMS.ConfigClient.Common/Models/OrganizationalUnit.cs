using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    public class OrganizationalUnit
    {
        public long HospitalId { get; set; }
        public List<Department> Department { get; set; }
        public List<OPDListItem> OPDList { get; set; }
        public bool IsHospitalLevel { get; set; }
    }

    public enum OrganizationalUnitType
    {
        Department,
        Section,
        Ward,
        OPD,
        Location
    }
}
