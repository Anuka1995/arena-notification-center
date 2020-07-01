using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.Models
{
    public class PSSLinkModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public long? HospitalId { get; set; }
        public string Link { get; set; }
        public bool active { get; set; }
    }
}
