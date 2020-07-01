using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class GroupedTemplateDataService : IGroupedTextDataService
    {
        private readonly IGroupedTextDataStore m_groupedTemplateDataStore;

        public GroupedTemplateDataService(IGroupedTextDataStore groupedTemplateDataStore)
        {
            m_groupedTemplateDataStore = groupedTemplateDataStore;
        }

        public GroupedTextDTO GetGroupedTextBy(Guid templateGuid)
        {
            return m_groupedTemplateDataStore.SelectAGroupedTextBy(templateGuid);
        }

        public Guid SaveGroupedText(GroupedTextDTO tempateDTO)
        {           
            return m_groupedTemplateDataStore.CreateUpdateGroupText(tempateDTO);
        }

        public List<GroupedTextDTO> SearchGroupedText(long? departmentid, string searchterm, bool isActive, bool ishospitalOnly, long hospitalId)
        {
            return m_groupedTemplateDataStore.SelectGroupedTextsOn(departmentid, searchterm, isActive, ishospitalOnly, hospitalId);
        }
    }
}
