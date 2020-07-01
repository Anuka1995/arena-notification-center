using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IDIPSContactTypeService
    {
        List<OfficialLevelOfCare> GetOfficialLevelOfCareInfo();
        List<ContactType> GetContactTypeInfo();
    }
}
