using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface IConfigPSSLinkDataService
    {
        /// <summary>
        /// Add / Edit PSS Link
        /// </summary>
        Guid CreateUpdatePSSLink(SmsConfigurationDTO smsConfigurationDto);

        /// <summary>
        /// Get PSS link by Hospital id
        /// </summary>
        List<SmsConfigurationDTO> GetPSSLinkByHospital(long hospitalId);
    }
}
