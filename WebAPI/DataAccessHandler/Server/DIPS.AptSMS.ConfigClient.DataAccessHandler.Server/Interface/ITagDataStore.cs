using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface ITagDataStore
    {
        /// <summary>
        /// Save a new Tag.
        /// </summary>
        Guid InsertATag(TagDTO tag);

        /// <summary>
        /// Update a Tag.
        /// </summary>
        Guid UpdateATag(TagDTO tag);

        /// <summary>
        /// Insert/Update a Tag.
        /// </summary>
        Guid InsertOrUpdateTag(TagDTO tag);

        /// <summary>
        /// Get a tag by guid
        /// </summary>
        TagDTO SelectTagBy(Guid tagId);

        ///<summary>
        ///</summary>
        List<TagDTO> SelectTagsOn(long? departmentId, string keyword,long hospitalId);

        ///<summary>
        ///</summary>
        List<TagDTO> SelectTagsOn(long? departmentId, string keyword, bool active, bool hospitalLevel, long hospitalId);
    }
}
