using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface ITextTemplateDataService
    {
        ///<summary>
        ///Save A TextTemplate
        ///</summary>
        Guid SaveTextTemplate(TextTemplateDTO textTemplateDTO);

        ///<summary>
        ///Get a TextTemplate by guild
        ///</summary>
        TextTemplateDTO GetTextTemplateById(Guid textTemplateGuid);

        ///<summary>
        ///Search for TextTemplate
        /// </summary>
        List<TextTemplateDTO> SearchTextTemplate(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId);
        List<TextTemplateDTO> GetTextTemplatByWard(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByOPD(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByWard_BySections(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);      
        List<TextTemplateDTO> GetTextTemplateBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByDepartment(Guid scheduleId, long departmentID, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByHospitalLevel(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetGetTextTemplateByHospitalLevel_OPD(Guid scheduleId, long hospitalId, long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);

        ///<summary>
        ///Get the overview of text templates
        /// </summary>
        List<TextTemplateDTO> GetTemplatesOverviewBy(bool isActive,long hospitalId);
    }
}
