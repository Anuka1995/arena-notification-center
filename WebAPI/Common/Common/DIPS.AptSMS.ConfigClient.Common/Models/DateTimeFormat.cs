using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    public class DateTimeFormat
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public long? HospitalId { get; set; }
        public string format { get; set; }
        public string displaySample { get; set; }
        public bool active { get; set; }
    }
}
