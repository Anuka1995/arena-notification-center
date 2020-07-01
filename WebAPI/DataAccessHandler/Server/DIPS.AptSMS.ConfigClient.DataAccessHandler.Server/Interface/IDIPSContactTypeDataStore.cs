using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface IDIPSContactTypeDataStore
    {
        /// <summary>
        /// Get offical level of care details.
        /// </summary>
        List<OfficialLevelOfCareDTO>  SelectOfficialLevelOfCare();

        /// <summary>
        /// Get contact type details.
        /// </summary>
        List<ContactTypeDTO> SelectContactType();
    }
}
