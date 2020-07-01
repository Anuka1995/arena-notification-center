using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO
{
    public class SmsConfigurationDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public long? HospitalId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
