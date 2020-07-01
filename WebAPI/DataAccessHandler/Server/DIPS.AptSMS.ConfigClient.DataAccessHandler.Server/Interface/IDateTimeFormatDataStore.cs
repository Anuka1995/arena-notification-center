using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface IDateTimeFormatDataStore
    {
        /// <summary>
        /// Add / Edit DateTime format
        /// </summary>
        Guid CreateUpdateDateTimeFormat(SmsConfigurationDTO smsConfigurationDto);

        /// <summary>
        /// Get DateTime formats by Hospital id.
        /// </summary>
        List<SmsConfigurationDTO> GetDateTimeFormatByHospital(long hospitalId);

    }
}
