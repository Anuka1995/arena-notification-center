using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class PSSLinkService: IPSSLinkService
    {
        private readonly ILog s_log = LogProvider.GetLogger(typeof(PSSLinkService));
        private readonly IConfigPSSLinkDataService m_PSSLinkDataService;

        public PSSLinkService(IConfigPSSLinkDataService PSSLinkDataService)
        {
            m_PSSLinkDataService = PSSLinkDataService;
        }

        public Guid CreateUpdatePSSLink(PSSLinkModel PSSLinkDto)
        {
            return m_PSSLinkDataService.CreateUpdatePSSLink(ModelToDataDTO(PSSLinkDto));
        }

        public List<PSSLinkModel> GetPSSLinkByHospital(long hospitalId)
        {
            var PSSLinkDTOs = m_PSSLinkDataService.GetPSSLinkByHospital(hospitalId);

            var pssLinkModels = PSSLinkDTOs.Select(DataDTOToModel).ToList();
            return pssLinkModels;
        }

        private SmsConfigurationDTO ModelToDataDTO(PSSLinkModel PSSLink)
        {
            return new SmsConfigurationDTO()
            {
                Id = PSSLink.Id,
                HospitalId = PSSLink.HospitalId,
                Name = PSSLink.Name,
                Value = PSSLink.Link,
                IsActive = PSSLink.active
            };
        }

        private PSSLinkModel DataDTOToModel(SmsConfigurationDTO smsConfigurationDto)
        {
            return new PSSLinkModel()
            {
                Id = smsConfigurationDto.Id,
                Name = smsConfigurationDto.Name,
                HospitalId = smsConfigurationDto.HospitalId,
                Link = smsConfigurationDto.Value,
                active = smsConfigurationDto.IsActive
            };
        }
    }
}
