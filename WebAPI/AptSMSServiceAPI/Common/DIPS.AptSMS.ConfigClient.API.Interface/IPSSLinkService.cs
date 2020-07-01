using System;
using System.Collections.Generic;
using System.Text;
using DIPS.AptSMS.ConfigClient.Common.Models;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
     public interface IPSSLinkService
    {
        
        Guid CreateUpdatePSSLink(PSSLinkModel PSSLinkDto);

        List<PSSLinkModel> GetPSSLinkByHospital(long hospitalId);
    }
}
