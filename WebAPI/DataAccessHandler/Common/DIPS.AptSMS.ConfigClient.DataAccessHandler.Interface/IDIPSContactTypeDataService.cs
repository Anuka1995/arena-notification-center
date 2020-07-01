using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public interface IDIPSContactTypeDataService
    {
        ///<summary>
        ///Get a Officail Level Of Care Details
        ///</summary>
        List<OfficialLevelOfCareDTO> GetOfficialLevelOfCareDetails();

        ///<summary>
        ///Get contact type Details
        ///</summary>
        List<ContactTypeDTO> GetContactTypeDetails();
    }
}