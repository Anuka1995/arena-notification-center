using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface ITagDataService
    {
        ///<summary>
        ///Save A Tag
        ///</summary>
        Guid SaveTag(TagDTO tag);

        ///<summary>
        ///Get a tag by guild
        ///</summary>
        TagDTO GetTagBy(Guid tagGuid);

        ///<summary>
        ///Search for Tags
        /// </summary>
        List<TagDTO> SearchTags(long? departmentid, string searchterm, long hospitalId);

        ///<summary>
        ///Search for Tags
        /// </summary>
        List<TagDTO> SearchTags(long? departmentid, string searchterm, bool active, bool hospitalLevel, long hospitalId);
    }
}
