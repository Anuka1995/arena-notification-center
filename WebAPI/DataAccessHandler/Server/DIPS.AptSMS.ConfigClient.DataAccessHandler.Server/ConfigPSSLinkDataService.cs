using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class ConfigPSSLinkDataService : IConfigPSSLinkDataService
    {
        private readonly IConfigPSSLinkDataStore m_configPssLinkDataStore;
        public ConfigPSSLinkDataService(IConfigPSSLinkDataStore configPssLinkDataStore)
        {
            m_configPssLinkDataStore = configPssLinkDataStore;
        }
        public Guid CreateUpdatePSSLink(SmsConfigurationDTO smsConfigurationDto)
        {
            return m_configPssLinkDataStore.CreateUpdatePSSLink(smsConfigurationDto);
        }

        public List<SmsConfigurationDTO> GetPSSLinkByHospital(long hospitalId)
        {
            return m_configPssLinkDataStore.GetPSSLinkByHospital(hospitalId);
        }
    }
}
