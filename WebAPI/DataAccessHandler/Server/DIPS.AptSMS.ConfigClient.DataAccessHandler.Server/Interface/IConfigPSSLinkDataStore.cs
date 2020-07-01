using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface IConfigPSSLinkDataStore
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
