using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface IGroupedTextDataStore
    {
        /// <summary>
        /// Save a New Grouped Text Template.
        /// </summary>       
        [Obsolete]
        Guid InsertGroupedText(GroupedTextDTO template);

        /// <summary>
        /// Update a Grouped Text Template.
        /// </summary>
        [Obsolete]
        Guid UpdateGroupedText(GroupedTextDTO template);

        /// <summary>
        /// Create or update Grouped text template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        Guid CreateUpdateGroupText(GroupedTextDTO template);

        ///<summary>
        ///Query a grouped template by its guid
        /// </summary>
        /// 
        GroupedTextDTO SelectAGroupedTextBy(Guid tempalteGuid);

        ///<summary>
        ///Filter grouped tempaltes
        /// </summary>
        /// 
        List<GroupedTextDTO> SelectGroupedTextsOn(long? departmentID, string searchTerm, bool isActive, bool ishospitalOnly, long hospitalId);
    }
}
