using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface IDateTimeFormatDataService
    {
        /// <summary>
        /// Add / Edit DateTime format
        /// </summary>
        Guid SaveDateTimeFormat(SmsConfigurationDTO smsConfigurationDto);

        /// <summary>
        /// Get DateTime formats by Hospital id.
        /// </summary>
        List<SmsConfigurationDTO> GetDateTimeFormatByHospital(long hospitalId);

    }
}
