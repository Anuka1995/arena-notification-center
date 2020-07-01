using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface IGroupedTextDataService
    {
        ///<summary>
        ///Save A GroupedTemplate
        ///</summary>
        Guid SaveGroupedText(GroupedTextDTO tempateDTO);

        ///<summary>
        ///Get a GroupedTemplate by guild
        ///</summary>
        GroupedTextDTO GetGroupedTextBy(Guid templateGuid);

        ///<summary>
        ///Search for GroupedTemplates
        /// </summary>
        List<GroupedTextDTO> SearchGroupedText(long? departmentid, string searchterm, bool isActive, bool ishospitalOnly, long hospitalId);
    }
}
