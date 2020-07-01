using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class TagDataService : ITagDataService
    {
        private readonly ITagDataStore m_templateTagDataStore;

        public TagDataService(ITagDataStore templateTagDataStore)
        {
            m_templateTagDataStore = templateTagDataStore;
        }

        public TagDTO GetTagBy(Guid tagGuid)
        {
            return m_templateTagDataStore.SelectTagBy(tagGuid);
        }

        public Guid SaveTag(TagDTO tag)
        {
            return m_templateTagDataStore.InsertOrUpdateTag(tag);
        }

        public List<TagDTO> SearchTags(long? departmentid, string searchterm, long hospitalId)
        {
            return m_templateTagDataStore.SelectTagsOn(departmentid, searchterm, hospitalId);
        }

        public List<TagDTO> SearchTags(long? departmentid, string searchterm, bool active, bool hospitalLevel, long hospitalId)
        {
            return m_templateTagDataStore.SelectTagsOn(departmentid, searchterm, active, hospitalLevel, hospitalId);
        }
    }
}
