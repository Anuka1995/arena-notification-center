using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class DateTimeFormatService : IDateTimeFormatService
    {
        private readonly ILog s_log = LogProvider.GetLogger(typeof(DateTimeFormatService));
        private readonly IDateTimeFormatDataService m_DateTimeFormatDataService;

        public DateTimeFormatService(IDateTimeFormatDataService dateTimeFormatDataService)
        {
            m_DateTimeFormatDataService = dateTimeFormatDataService;
        }
        public Guid SaveDateTimeFormat(DateTimeFormat dateTimeFormatDto)
        {
            return m_DateTimeFormatDataService.SaveDateTimeFormat(ModelToDataDTO(dateTimeFormatDto));
        }

        public List<DateTimeFormat> GetDateTimeFormatByHospital(long hospitalId)
        {
            var dateTimeFormatDTOs = m_DateTimeFormatDataService.GetDateTimeFormatByHospital(hospitalId);

            var list = dateTimeFormatDTOs.Select(DataDTOToModel).ToList();
            return list;
        }

        private SmsConfigurationDTO ModelToDataDTO(DateTimeFormat dateTimeFormat)
        {
            return new SmsConfigurationDTO()
            {
                Id = dateTimeFormat.Id,
                HospitalId = dateTimeFormat.HospitalId,
                Name = dateTimeFormat.Name,
                Value = dateTimeFormat.format,
                IsActive = dateTimeFormat.active
            };
        }

        private DateTimeFormat DataDTOToModel(SmsConfigurationDTO smsConfigurationDto)
        {
            var date = DateTime.Now;
            return new DateTimeFormat()
            {
                Id = smsConfigurationDto.Id,
                HospitalId = smsConfigurationDto.HospitalId,
                Name = smsConfigurationDto.Name,
                displaySample = date.ToString(smsConfigurationDto.Value),
                format = smsConfigurationDto.Value,
                active = smsConfigurationDto.IsActive
            };
        }

        public List<DateTimeFormat> GetActiveDateTimeFormatByHospital(long hospitalId)
        {
            var dateTimeFormatDTOs = m_DateTimeFormatDataService.GetDateTimeFormatByHospital(hospitalId);
            var activeDateTimeFormatDTOs = dateTimeFormatDTOs.Where(t => (t.IsActive == true)).ToList();
            var list = activeDateTimeFormatDTOs.Select(DataDTOToModel).ToList();
            return list;
        }
    }
}