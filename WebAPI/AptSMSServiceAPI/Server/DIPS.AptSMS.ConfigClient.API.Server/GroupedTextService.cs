using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class GroupedTextService : IGroupedTextService
    {
        private readonly IGroupedTextDataService m_groupedTextDataService;
        private readonly IOrgUnitsService m_orgService;

        public GroupedTextService(IGroupedTextDataService textTemplateDataService, IOrgUnitsService orgService)
        {
            m_groupedTextDataService = textTemplateDataService;
            m_orgService = orgService;
        }

        public List<SMSTextTemplate> FilterTextTemplatesBy(long departmentID)
        {
            throw new NotImplementedException();
        }

        public List<SMSTextTemplate> FilterTextTemplatesBy(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public List<SMSTextTemplate> FilterTextTemplatesBy(long? departmentID, string searchTerm, Guid SecurityToken, bool getInactive, bool ishospitalOnly, long hospitalId)
        {
            bool isActive = !getInactive;
            var dataDTOList = m_groupedTextDataService.SearchGroupedText(departmentID, searchTerm, isActive, ishospitalOnly, hospitalId);
            var SmsTextTemplateList = dataDTOList.Select(t => DataDTOToModel(t)).ToList();
            if (departmentID != null)
            {
                var department = m_orgService.GetDepartmentByDepartmentId((long)departmentID, SecurityToken);
                SmsTextTemplateList.ForEach(c => c.DepartmentShortName = department.DepartmentShortName);
                return SmsTextTemplateList;
            }
            var departmentidList = SmsTextTemplateList.Select(t => t.DepartmentID).Distinct().ToList();
            var DeparmentList = m_orgService.GetDepartmentListByDepartmentIdList(departmentidList, SecurityToken);

            foreach (var textTemplate in SmsTextTemplateList)
            {
                if (textTemplate.DepartmentID == null)
                    continue;

                var selectedDep = DeparmentList.Where(t => t.DepartmentId == textTemplate.DepartmentID).ToList().FirstOrDefault();
                if (selectedDep != null)
                    textTemplate.DepartmentShortName = selectedDep.DepartmentShortName;
            }
            return SmsTextTemplateList;
        }

        public SMSTextTemplate GetTextTemplateBy(Guid templateGuid)
        {
            var textTemplateDTo = m_groupedTextDataService.GetGroupedTextBy(templateGuid);

            return DataDTOToModel(textTemplateDTo);
        }

        public Guid SaveGroupedTemplate(SMSTextTemplate texttemplate)
        {
            var templateDTO = ModelToDataDTO(texttemplate);
            return m_groupedTextDataService.SaveGroupedText(templateDTO);
        }

        #region Private Methods
        private GroupedTextDTO ModelToDataDTO(SMSTextTemplate texttemplate)
        {
            return new GroupedTextDTO()
            {
                GroupedTempateGUID = (texttemplate.TextTemplateTextId == null || texttemplate.TextTemplateTextId == Guid.Empty)? null : texttemplate.TextTemplateTextId,
                Name = texttemplate.TextTemplateName,
                Text = texttemplate.TextTemplateString,
                DepartmentID = texttemplate.DepartmentID,
                OrganizationID = texttemplate.OrganizationID,
                HospitalID = texttemplate.HospitalID,
                IsActive = texttemplate.IsActive,
                ValidFrom = texttemplate.ValidFrom,
                ValidTo = texttemplate.ValidTo
            };
        }

        private SMSTextTemplate DataDTOToModel(GroupedTextDTO dataDto)
        {
            return new SMSTextTemplate()
            {
                TextTemplateTextId = dataDto.GroupedTempateGUID,
                OrganizationID = dataDto.OrganizationID,
                HospitalID = dataDto.HospitalID,
                DepartmentID = dataDto.DepartmentID,
                TextTemplateName = dataDto.Name,
                TextTemplateString = dataDto.Text,
                IsActive = dataDto.IsActive,
                ValidFrom = dataDto.ValidFrom,
                ValidTo = dataDto.ValidTo
            };
        }
        #endregion
    }
}
