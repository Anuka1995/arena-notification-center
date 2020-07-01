using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class DateTimeFormatDataService : IDateTimeFormatDataService
    {
        private IDateTimeFormatDataStore m_DateTimeFormatDataStore;

        public DateTimeFormatDataService(IDateTimeFormatDataStore dateTimeFormatDataStore)
        {
            m_DateTimeFormatDataStore = dateTimeFormatDataStore;
        }
        public Guid SaveDateTimeFormat(SmsConfigurationDTO smsConfigurationDto)
        {
            return m_DateTimeFormatDataStore.CreateUpdateDateTimeFormat(smsConfigurationDto);
        }

        public List<SmsConfigurationDTO> GetDateTimeFormatByHospital(long hospitalId)
        {
            return m_DateTimeFormatDataStore.GetDateTimeFormatByHospital(hospitalId);
        }
    }
}
