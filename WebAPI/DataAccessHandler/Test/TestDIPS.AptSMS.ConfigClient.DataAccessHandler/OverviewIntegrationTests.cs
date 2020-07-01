using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class OverviewIntegrationTests : TestBase
    {
        private readonly int sendBeforeDays = 110;

        [TestMethod, TestCategory("IntegrationTest")]
        public void OverviewIntegrationTests_Overview_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();

            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var overviewOfTemplates = textTemplateDatatService.GetTemplatesOverviewBy(true,TestHospitalId1);

            Assert.IsTrue(overviewOfTemplates.Any());
        }

        #region private
        private TextTemplateDTO GetTextTemplate(Guid? templateId, Guid? ruleSetId,
            Guid? groupTextTemplate, long hospitalID, long? depId, long? locId,
            long? secId, long? wardId, long? opdId, List<long> offLevelCare,
            List<long> contactType, bool isVideo = false, string name = "Test Textemplate",
            string smsText = "Test SMS Text",
            bool isActive = true)
        {

            var template = new TextTemplateDTO
            {
                HospitalID = hospitalID,
                IsActive = isActive,
                Name = name,
                SMSText = smsText,
                AttachPSSLink = true,
                OPDID = opdId,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(2),
                ContactType = contactType,
                OfficialLevelOfCare = offLevelCare,
                IsVideoAppoinment = isVideo
            };
            if (depId != null)
                template.DepartmentID = (long)depId;
            if (locId != null)
                template.LocationID = (long)locId;
            if (secId != null)
                template.SectionID = (long)secId;
            if (wardId != null)
                template.WardID = (long)wardId;
            if (templateId != null)
                template.TemplateGUID = (Guid)templateId;
            if (ruleSetId != null)
                template.RuleSetGUID = (Guid)ruleSetId;
            if (groupTextTemplate != null)
                template.GroupedTextGUID = (Guid)groupTextTemplate;

            return template;
        }

        private List<long> GetOfficialLevelOfCareList()
        {
            var DIPSContactTypeDataService = GetInstance<IDIPSContactTypeDataService>();
            var offCareList = DIPSContactTypeDataService.GetOfficialLevelOfCareDetails();
            return offCareList.Select(l => l.Id).ToList();
        }
        private List<long> GetContactTypeList()
        {
            var DIPSContactTypeDataService = GetInstance<IDIPSContactTypeDataService>();
            var contactTypeList = DIPSContactTypeDataService.GetContactTypeDetails();
            return contactTypeList.Select(l => l.Id).ToList();
        }

        private GroupedTextDTO GetGroupedText(string smsText = null)
        {
            return new GroupedTextDTO()
            {
                HospitalID = TestHospitalId1,
                DepartmentID = TestDepartmentId2,
                Name = "GroupedText",
                Text = smsText
            };
        }

        private RuleSetDTO GetRuleSet(int daysCount, long hospitalId, long? departmentId, string name = "My Test RuleSet")
        {
            return new RuleSetDTO
            {
                Name = name,
                HospitalID = hospitalId,
                DepartmentID = departmentId,
                IsActive = true,
                RuleSetGUID = null,
                IgnoreSMStoAdmittedPatient = true,
                ExcludingOrgUnitIDs = new List<string>() { "aabaea+0000000000011", "aabaea+0000000000010" },
                isValidateAptTime = true,
                AptValidate_From = "10:00",
                AptValidate_To = "11:00",
                DaysForRetryExpiry = 1,
                SendingTimeWindowFrom = "09:00",
                SendingTimeWindowTo = "11:00",
                SendSMSBeforeDays = daysCount,
                SendSMSBeforeInMins = 30,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(1)
            };
        }

        private RuleSetDTO GetRuleSetWithExcludedOrgUnits(List<string> orgIDs, string name = "My Test RuleSet")
        {
            // orgIDs.Add("0");            
            return new RuleSetDTO
            {
                Name = name,
                HospitalID = TestHospitalId1,
                DepartmentID = TestDepartmentId2,
                IsActive = true,
                RuleSetGUID = null,
                IgnoreSMStoAdmittedPatient = true,
                ExcludingOrgUnitIDs = orgIDs,
                isValidateAptTime = true,
                AptValidate_From = "10:00",
                AptValidate_To = "11:00",
                DaysForRetryExpiry = 1,
                SendingTimeWindowFrom = "09:00",
                SendingTimeWindowTo = "11:00",
                SendSMSBeforeDays = 1,
                SendSMSBeforeInMins = 30,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now
            };
        }
        #endregion
    }
}
