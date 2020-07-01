using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface ITextTemplateDataStore
    {
        /// <summary>
        /// Add / Edit new Text Template.
        /// </summary>
        Guid InsertOrUpdateTextTemplate(TextTemplateDTO templateDTO);

        /// <summary>
        /// Get a Text Template by ID.
        /// </summary>
        TextTemplateDTO SelectTextTemplateById(Guid templateGuid);

        /// <summary>
        /// Filter a Text Template.
        /// </summary>
        List<TextTemplateDTO> SelectTextTemplateOn(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId);
        List<TextTemplateDTO> GetTextTemplatByWard(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByWard_BySections(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);       
        List<TextTemplateDTO> GetTextTemplatByOPD(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplateBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByDepartment(Guid scheduleId, long departmentID, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetTextTemplatByHospital(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<TextTemplateDTO> GetGetTextTemplateByHospitalLevel_OPD(Guid scheduleId, long hospitalId, long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);

        /// <summary>
        /// Query for all active text tempalte for a hospital
        /// </summary>
        List<TextTemplateDTO> SelectFullTextTemplatesOn(bool isActive, long hospitalId);


    }
}
