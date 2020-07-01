using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class DIPSContactTypeService : IDIPSContactTypeService
    {
        private readonly IDIPSContactTypeDataService m_DIPSContactTypeDataService;

        public DIPSContactTypeService(IDIPSContactTypeDataService DIPSContactTypeDataService)
        {
            m_DIPSContactTypeDataService = DIPSContactTypeDataService;
        }

        public List<OfficialLevelOfCare> GetOfficialLevelOfCareInfo()
        {
            var officialLevelOfCareDTOList = m_DIPSContactTypeDataService.GetOfficialLevelOfCareDetails();

            return officialLevelOfCareDTOList.Select(t => DataDTOToModel(t)).ToList();
        }

        public List<ContactType> GetContactTypeInfo()
        {
            var contactTypeDTOList = m_DIPSContactTypeDataService.GetContactTypeDetails();

            return contactTypeDTOList.Select(t => DataDTOToModel(t)).ToList();
        }

        #region Private Methods
       
        private OfficialLevelOfCare DataDTOToModel(OfficialLevelOfCareDTO dataDto)
        {
            return new OfficialLevelOfCare()
            {
                OfficialLevelOfCareId = dataDto.Id,
                OfficialLevelOfCareName = dataDto.Name
            };
        }

        private ContactType DataDTOToModel(ContactTypeDTO dataDto)
        {
            return new ContactType()
            {
                ContactTypeId = dataDto.Id,
                ContactTypeName = dataDto.Name
            };
        }
        #endregion
    }
}
