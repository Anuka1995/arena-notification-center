using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class TextTemplateDataService : ITextTemplateDataService
    {
        private readonly ITextTemplateDataStore m_textTemplateDataStore;

        public TextTemplateDataService(ITextTemplateDataStore textTemplateDataStore)
        {
            m_textTemplateDataStore = textTemplateDataStore;
        }

        public List<TextTemplateDTO> GetTextTemplatByOPD(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplatByOPD(scheduleId, depId, opdId, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetTextTemplatByWard(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplatByWard(scheduleId, depId, wardId, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public TextTemplateDTO GetTextTemplateById(Guid templateGuid)
        {
            return m_textTemplateDataStore.SelectTextTemplateById(templateGuid);
        }

        public Guid SaveTextTemplate(TextTemplateDTO templateDTO)
        {
            return m_textTemplateDataStore.InsertOrUpdateTextTemplate(templateDTO);
        }

        public List<TextTemplateDTO> SearchTextTemplate(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId)
        {
            return m_textTemplateDataStore.SelectTextTemplateOn(departmetnID, opdID, sectionID, wardID, searchTerm, isActive, isHospitalOnly, hospitalId);
        }

        public List<TextTemplateDTO> GetTextTemplatByWard_BySections(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplatByWard_BySections(scheduleId, depId, wardId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetTextTemplateBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplateBySection(scheduleId, depId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetTextTemplatByDepartment(Guid scheduleId, long departmentID, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplatByDepartment(scheduleId, departmentID, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetTextTemplatByHospitalLevel(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetTextTemplatByHospital(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetGetTextTemplateByHospitalLevel_OPD(Guid scheduleId, long hospitalId, long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            return m_textTemplateDataStore.GetGetTextTemplateByHospitalLevel_OPD(scheduleId, hospitalId, opdId, locationId, contactTypes, officialLevelofcare);
        }

        public List<TextTemplateDTO> GetTemplatesOverviewBy(bool isActive,long hospitalId)
        {
            return m_textTemplateDataStore.SelectFullTextTemplatesOn(isActive,hospitalId);
        }
    }
}
