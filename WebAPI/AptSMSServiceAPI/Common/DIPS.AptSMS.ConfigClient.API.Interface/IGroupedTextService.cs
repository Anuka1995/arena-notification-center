using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IGroupedTextService
    {
        Guid SaveGroupedTemplate(SMSTextTemplate texttemplate);

        SMSTextTemplate GetTextTemplateBy(Guid templateGuid);

        List<SMSTextTemplate> FilterTextTemplatesBy(long departmentID);

        List<SMSTextTemplate> FilterTextTemplatesBy(string searchTerm);

        List<SMSTextTemplate> FilterTextTemplatesBy(long? departmentID, string searchTerm, Guid SecurityToken, bool getInactive, bool ishospitalOnly, long hospitalId);
    }
}
