using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.Common.Models;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IDateTimeFormatService
    {
        /// <summary>
        /// Add / Edit DateTime format
        /// </summary>
        Guid SaveDateTimeFormat(DateTimeFormat dateTimeFormatDto);

        /// <summary>
        /// Get DateTime formats by Hospital id.
        /// </summary>
        List<DateTimeFormat> GetDateTimeFormatByHospital(long hospitalId);
        List<DateTimeFormat> GetActiveDateTimeFormatByHospital(long hospitalId);
    }
}
