using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class DIPSContactTypeDataService : IDIPSContactTypeDataService
    {
        private readonly IDIPSContactTypeDataStore m_DIPSContactTypeDataStore;

        public DIPSContactTypeDataService(IDIPSContactTypeDataStore DIPSContactTypeDataStore)
        {
            m_DIPSContactTypeDataStore = DIPSContactTypeDataStore;
        }

        public List<OfficialLevelOfCareDTO> GetOfficialLevelOfCareDetails()
        {
            return m_DIPSContactTypeDataStore.SelectOfficialLevelOfCare();
        }

        public List<ContactTypeDTO> GetContactTypeDetails()
        {
            return m_DIPSContactTypeDataStore.SelectContactType();
        }
    }
}
