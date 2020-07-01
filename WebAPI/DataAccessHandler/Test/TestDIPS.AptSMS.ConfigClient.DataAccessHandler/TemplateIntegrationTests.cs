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
    public class TemplateIntegrationTests : TestBase
    {
        private readonly int sendBeforeDays = 110;

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, 
                TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList());
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_NullOfficialLEvelOfCare_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, 
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, 
                null, GetContactTypeList());
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_EmptyOfficialLEvelOfCare_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                new List<long>(), GetContactTypeList());
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_NullContactType_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                GetOfficialLevelOfCareList(), null);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_EmptyContactType_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                GetOfficialLevelOfCareList(), new List<long>());
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_UpdateTempalte_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, GetOfficialLevelOfCareList(), GetContactTypeList());
            var guid1 = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid1);
            var textTemplate1 = GetTextTemplate(guid1, ruleSetGuid, null, 1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 2 for test update");
            var guid2 = textTemplateDatatService.SaveTextTemplate(textTemplate1);
            Assert.IsNotNull(guid2);
        }



        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_ByDepartmentId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate1 = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid1 = textTemplateDatatService.SaveTextTemplate(textTemplate1);
            Assert.IsNotNull(guid1);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(TestDepartmentId2, null, null, null, null, true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid1).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_BySectionId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, 41, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, 41, null, null, true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_ByOPDId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, TestOpdId, null, null, null, true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_BywardId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, TestWardId, null, true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_BywardId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            //Save Text Template for Department-->ward level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, null, null, TestWardId, null, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.GetTextTemplatByWard(ruleSetGuid, TestDepartmentId2, TestWardId, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_ByOPDId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            //Save Text Template for Department-->OPD level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, null, null, null, TestOpdId, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.GetTextTemplatByOPD(ruleSetGuid, TestDepartmentId2, TestOpdId, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }
        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_BySectionId_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            //Save Text Template for Department-->Section level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, null, TestSectionId, null, null, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
            var textTempaleList = textTemplateDatatService.GetTextTemplateBySection(ruleSetGuid, TestDepartmentId2, TestSectionId, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }
        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_ByDepartment_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            //Save Text Template for Department level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, null, null, null, null, null, null, smsText: "Test Template 1");
            
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            
            Assert.IsNotNull(guid);
            var textTempaleList = textTemplateDatatService.GetTextTemplatByDepartment(ruleSetGuid, TestDepartmentId2, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_ByHospital_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template"));
            //Save Text Template for Department level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
            var textTempaleList = textTemplateDatatService.GetTextTemplatByHospitalLevel(ruleSetGuid, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_ByHospital_and_OPD_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template"));
            //Save Text Template for Department level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, TestOpdId, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
            var textTempaleList = textTemplateDatatService.GetGetTextTemplateByHospitalLevel_OPD(ruleSetGuid, TestHospitalId1, TestOpdId, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_AdvancedFilterTexts_BywardId_AND_SectionBy_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            //Save Text Template for Department-->ward level & section level
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, null, TestSectionId, TestWardId, null, null, null, smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.GetTextTemplatByWard_BySections(ruleSetGuid, TestDepartmentId2, TestWardId, TestSectionId, TestHospitalId1, null, null, null);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_BySearchTerm_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, "Test Template 1", true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_GetInActiveTemplates_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1", isActive: false);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, false, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);

            var textTempaleList1 = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, true, false, TestHospitalId1);
            var value1 = textTempaleList1.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value1.Count == 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_GetHospitalLevelOnly_InActiveTemplate_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1", isActive: false);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, false, true, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_ForGivenHospitalOnly_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template 1"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, true, false, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Any(c => c.TemplateGUID == guid);
            Assert.IsTrue(value);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_ForGivenHospitalOnly_Fail()
        {
            //Use Different hospital ids to save and search the sms Text templates.filter will not work as expected.
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template 1"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1");
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, true, false, TestHospitalId2);
            Assert.IsFalse(textTempaleList.Any(c => c.TemplateGUID == guid));
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_FilterTemplates_GetHospitalLevelOnly_ActiveTemplate_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, null, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, null, null, null, null, null, GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: "Test Template 1", isActive: true);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);

            var textTempaleList = textTemplateDatatService.SearchTextTemplate(null, null, null, null, null, true, true, TestHospitalId1);
            Assert.AreNotEqual(0, textTempaleList.Count);
            var value = textTempaleList.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value.Count == 1);

            //hospital level=false ==>for this department shpuld also mentioned.thats how the hospital level gets false in SP level.
            var textTempaleList1 = textTemplateDatatService.SearchTextTemplate(TestDepartmentId2, null, null, null, null, true, false, TestHospitalId1);
            var value1 = textTempaleList1.Where(c => c.TemplateGUID == guid).ToList();
            Assert.IsTrue(value1.Count == 0);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_GetTextTemplateById__SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList());
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);
            Assert.IsNotNull(guid);
            var smsTemplate = textTemplateDatatService.GetTextTemplateById(guid);
            Assert.IsNotNull(smsTemplate);
            Assert.AreEqual(guid, smsTemplate.TemplateGUID);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_GetTextTemplateById_InvalidId_NullReturned()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var smsTemplate = textTemplateDatatService.GetTextTemplateById(Guid.NewGuid());
            Assert.IsNull(smsTemplate);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_IsVideo_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), isVideo: true);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.GetTextTemplateById(guid);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(savedTemplate.IsVideoAppoinment);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_IsNotVideo_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, GetOfficialLevelOfCareList(), GetContactTypeList(), isVideo: false);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.GetTextTemplateById(guid);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(!savedTemplate.IsVideoAppoinment);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_EmptyText_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, TestDepartmentId2, "Ruleset to test Text template"));
          
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1, 
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId, 
                GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: null, isVideo: false);
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.GetTextTemplateById(guid);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(!savedTemplate.IsGenerateSMS);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_EmptyGroupedText_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var groupedTextDatatService = GetInstance<IGroupedTextDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();

            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1, 
                TestDepartmentId2, "Ruleset to test Text template"));
            var groupedTextGuid = groupedTextDatatService.SaveGroupedText(GetGroupedText());
            var textTemplate = GetTextTemplate(null, ruleSetGuid, groupedTextGuid, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: null, isVideo: false);
            
            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.GetTextTemplateById(guid);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(!savedTemplate.IsGenerateSMS);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_CreateTemplate_IsGenerateSMS_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var groupedTextDatatService = GetInstance<IGroupedTextDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();

            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1,
                TestDepartmentId2, "Ruleset to test Text template"));
            var groupedTextGuid = groupedTextDatatService.SaveGroupedText(GetGroupedText(smsText: "I am a sms text"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, groupedTextGuid, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                GetOfficialLevelOfCareList(), GetContactTypeList(), smsText: null, isVideo: false);

            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.GetTextTemplateById(guid);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(savedTemplate.IsGenerateSMS);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TemplateIntegrationTests_SearchTemplate_WithEmptyContent_SUCCESS()
        {
            var textTemplateDatatService = GetInstance<ITextTemplateDataService>();
            var groupedTextDatatService = GetInstance<IGroupedTextDataService>();
            var rulesetDatatService = GetInstance<IRuleSetDataService>();

            var ruleSetGuid = rulesetDatatService.SaveRuleSet(GetRuleSet(sendBeforeDays, TestHospitalId1,
                TestDepartmentId2, "Ruleset to test Text template"));
            var textTemplate = GetTextTemplate(null, ruleSetGuid, null, TestHospitalId1,
                TestDepartmentId2, TestLocationId, TestSectionId, TestWardId, TestOpdId,
                GetOfficialLevelOfCareList(), GetContactTypeList(), name: "abcd template", smsText: "", isVideo: false);

            var guid = textTemplateDatatService.SaveTextTemplate(textTemplate);

            var savedTemplate = textTemplateDatatService.SearchTextTemplate(null, null, null, null,
                "abcd", true, false, TestHospitalId1);

            Assert.IsNotNull(savedTemplate);
            Assert.IsTrue(savedTemplate.Any());
            var firstItem = savedTemplate.FirstOrDefault();
            Assert.IsTrue(!firstItem.IsGenerateSMS);
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
            return new GroupedTextDTO() {
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
