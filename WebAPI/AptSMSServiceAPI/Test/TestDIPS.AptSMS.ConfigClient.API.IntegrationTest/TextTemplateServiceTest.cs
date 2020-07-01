using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.API.Server;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    [TestClass]
    public class TextTemplateServiceTest : TestBase
    {
        private readonly long m_hospitalId = 1;
        private readonly long OrganizationId = 1;
        private static readonly int SendBeforeInDays = 101;


        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateServiceSuccess_GetTagWithOutFunction_Success()
        {
            var strKey = "FOEDSELSNRTYPENAVN";
            var templateService = (TextTemplateService)GetInstance<ITextTemplateService>();
            var result = templateService.GetTagWithFunction(strKey);
            Assert.AreEqual(new KeyValuePair<string, string>(strKey, ""), result);
        }
        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateServiceSuccess_ApplyFunction_Success()
        {
            var strKey = "FOEDSELSNRTYPENAVN";
            var templateService = (TextTemplateService)GetInstance<ITextTemplateService>();
            templateService.ApplyFunction("2020-02-17T09:14:19", "TX|MM/dd/yyyy HH:mm:ss");
            var result = templateService.GetTagWithFunction(strKey);
            Assert.AreEqual(new KeyValuePair<string, string>(strKey, ""), result);
        }



        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateServiceSuccess_GetTagWithFunction_Success()
        {
            var strKey = "FOEDSELSNRTYPENAVN:D";
            var templateService = (TextTemplateService)GetInstance<ITextTemplateService>();
            var result = templateService.GetTagWithFunction(strKey);
            Assert.AreEqual(new KeyValuePair<string, string>("FOEDSELSNRTYPENAVN", "D"), result);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateServiceSuccess_GetTagsList_CountSuccess()
        {
            var strKey = @"you need to be visit hospitl[$$FAGOMRAADE$$]and[$$OPPMOTETID$$] or this[$$LOKNAVN$$] and also[$$ADRESSE$$] bevare[$$AVDNAVN$$] or you might[$$ADRESSE: L$$] and the[$$ORGANISASJONSNR$$] then[$$POSTSTED$$][$$FOEDSELSNRTYPENAVN$$][$$MOTTATTFRA: U$$] then[$$HENVISNINGID$$]";
            var templateService = (TextTemplateService)GetInstance<ITextTemplateService>();
            var result = templateService.GetTagsList(strKey);
            Assert.AreEqual(11, result.Count);
            var strKey2 = @"you need to be visit hospitl[$$FAGOMRAADE$$]and[$$OPPMOTETID$$] or this[$$LOKNAVN$$] and also[$$ADRESSE$$] bevare[$$AVDNAVN$$] or you might[$$ADRESSE$$] and the[$$ORGANISASJONSNR$$] then[$$POSTSTED$$][$$FOEDSELSNRTYPENAVN$$][$$MOTTATTFRA: U$$] then[$$HENVISNINGID$$]";
            var result2 = templateService.GetTagsList(strKey2);
            Assert.AreEqual(11, result2.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_SaveTemplateForHospitalLevel_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, null, null, null, null, null, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_SaveTemplateForHospitalLevel_WithGroupTemplateId_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var groupTemplateService = GetInstance<IGroupedTextService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var guid = groupTemplateService.SaveGroupedTemplate(GetGroupTempalteForSave(TestdepId_1, m_hospitalId, true, DateTime.Now, DateTime.Now.AddDays(1)));
            Assert.IsNotNull(guid);
            var smsText = GetSmsTextTemplate(m_hospitalId, null, null, null, null, null, true, ruleSetGuid, null, guid, null);
            var guid1 = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid1);
        }

        //[TestMethod, TestCategory("IntegrationTest")]
        //public void TextTemplateService_SaveTemplateForHospitalLevel__Success()
        //{
        //    var templateService = GetInstance<ITextTemplateService>();
        //    var groupTemplateService = GetInstance<IGroupedTextService>();
        //    var guid = groupTemplateService.SaveGroupedTemplate(GetGroupTempalteForSave(TestdepId_1, m_hospitalId, true, DateTime.Now, DateTime.Now.AddDays(1)));
        //    Assert.IsNotNull(guid);
        //    var smsText = GetSmsTextTemplate(m_hospitalId,null, null, null, null, null, true, null, guid, null);
        //    var guid1 = templateService.SaveTextTemplate(smsText);
        //    Assert.IsNotNull(guid1);
        //}

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_SaveTemplateForDepartmentLevel__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_1, null, null, null, null, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_SaveTemplateWithLocation_Ward_Section__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByDepartmentId__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(TestdepId_2, null, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByDepartmentId__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(TestdepId_2, null, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByOpdId__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(null, TestOpdId, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_BySection__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(null, null, TestSectionId, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByWardId__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(null, null, null, TestWardId, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_BySearchTerm__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(null, null, null, null, "testTemplate", true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_NoSearchParam__ReturnTreeNodeList_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();

            //hospital level ruleset
            var ruleset1 = GetRuleSet("test-ruleset-hospitalLevel", null, m_hospitalId);
            var ruleSetGuid1 = ruleSetService.SaveRuleSet(ruleset1);

            //departmemt level ruleset
            var ruleset2 = GetRuleSet("test-ruleset-depLevel", TestdepId_2, m_hospitalId);
            var ruleSetGuid2 = ruleSetService.SaveRuleSet(ruleset2);

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid1, null, null, null, false, "testTemplate 1 Text", "testTemplate 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid2, null, null, null, false, "testTemplate 2 Text", "testTemplate 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var smsText2 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, null, null, null, null, true, ruleSetGuid1, null, null, null, false, "testTemplate 3 Text", "testTemplate 3");
            var guid2 = templateService.SaveSMSTextTemplate(smsText2);
            Assert.IsNotNull(guid2);

            var resultSmsText = templateService.GetSMSTextTemplateTreeNodes(null, null, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            var isRuleSet1Available = resultSmsText.Any(c => c.Id == ruleSetGuid1.ToString());
            Assert.IsTrue(isRuleSet1Available);
            var isRuleSet2Available = resultSmsText.Any(c => c.Id == ruleSetGuid2.ToString());
            Assert.IsTrue(isRuleSet2Available);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByOpdId__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, TestOpdId, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_BySection__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, TestSectionId, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_ByWardId__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, TestWardId, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_BySearchTerm__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null, false, "Testing searchTerm param");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, "Testing searchTerm", true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_BySearchTerm_EmptyText_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, 
                TestSectionId, TestWardId, true, ruleSetGuid, null, null, 
                null, false, smsText: null, textTemplateName: "abcd template");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, "abcd", true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_NoSearchParam_ReturnAllTextTemplates__Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null, false, "template 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, null, null, null, null, true, ruleSetGuid, null, null, null, false, "template 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid1).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_GetSMSTextTemplate_OnlyForGivenHospitalId_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            //AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null, false, "template 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsText1 = GetSmsTextTemplate(m_hospitalId, TestdepId_2, null, null, null, null, true, ruleSetGuid, null, null, null, false, "template 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);

            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, null, true, false, m_hospitalId);
            Assert.IsTrue(resultSmsText.Count >= 2);
            Assert.IsTrue(resultSmsText.Any(c => c.TextTemplateId == guid));
            Assert.IsTrue(resultSmsText.Any(c => c.TextTemplateId == guid1));

            var resultSmsText1 = templateService.SearchSMSTextTemplate(null, null, null, null, null, true, false, TestHospitalId2);
            Assert.IsFalse(resultSmsText1.Any(c => c.TextTemplateId == guid));
            Assert.IsFalse(resultSmsText1.Any(c => c.TextTemplateId == guid1));
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_GetHospitalLevelOnly_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", null, m_hospitalId);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null, false, "template 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var smsText1 = GetSmsTextTemplate(m_hospitalId, null, null, null, null, null, true, ruleSetGuid, null, null, null, false, "template 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, null, true, true, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 0);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid1).ToList().Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void SearchSmsTextTemplate_GetInActiveTemplates_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", null, m_hospitalId);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, false, ruleSetGuid, null, null, null, false, "template 1");
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var smsText1 = GetSmsTextTemplate(m_hospitalId, null, null, null, null, null, false, ruleSetGuid, null, null, null, false, "template 2");
            var guid1 = templateService.SaveSMSTextTemplate(smsText1);
            Assert.IsNotNull(guid1);
            var resultSmsText = templateService.SearchSMSTextTemplate(null, null, null, null, null, false, true, m_hospitalId);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid).ToList().Count == 0);
            Assert.IsTrue(resultSmsText.Where(c => c.TextTemplateId == guid1).ToList().Count == 1);

            var resultSmsText1 = templateService.SearchSMSTextTemplate(TestdepId_2, null, null, null, null, false, false, m_hospitalId);
            Assert.IsTrue(resultSmsText1.Where(c => c.TextTemplateId == guid).ToList().Count == 1);
            Assert.IsTrue(resultSmsText1.Where(c => c.TextTemplateId == guid1).ToList().Count == 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_GetSMSTextTemplateById_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            //AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, null, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);
            var smsTextTemplate = templateService.GetSMSTextTemplateById(guid);
            Assert.IsNotNull(smsTextTemplate);
            Assert.AreEqual(guid, smsTextTemplate.TextTemplateId);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_GetSMSTextTemplateById_WithGroupText_Success()
        {
            var templateService = GetInstance<ITextTemplateService>();
            var ruleSetService = GetInstance<IRuleSetService>();
            var grpTextService = GetInstance<IGroupedTextService>();
            var groupTextString = "Test Group template for sms text template";

            var ruleset = GetRuleSet("test-ruleset", TestdepId_2, m_hospitalId);
            var ruleSetGuid = ruleSetService.SaveRuleSet(ruleset);
            var groupTextId = grpTextService.SaveGroupedTemplate(GetTempalteForSave(TestdepId_2, OrganizationId, true, null, null, "Test Group template", groupTextString));

            var smsText = GetSmsTextTemplate(m_hospitalId, TestdepId_2, TestOpdId, TestLocationId, TestSectionId, TestWardId, true, ruleSetGuid, null, groupTextId, null);
            var guid = templateService.SaveSMSTextTemplate(smsText);
            Assert.IsNotNull(guid);

            var smsTextTemplate = templateService.GetSMSTextTemplateById(guid);
            Assert.IsNotNull(smsTextTemplate);
            Assert.AreEqual(guid, smsTextTemplate.TextTemplateId);
            Assert.AreEqual(groupTextId, smsTextTemplate.GroupTemplateId);
            Assert.AreEqual(groupTextString, smsTextTemplate.SMSTextTemplate);
        }

        [ExpectedException(typeof(UserInputValidationException))]
        [TestMethod, TestCategory("IntegrationTest")]
        public void TextTemplateService_GetSMSTextTemplateById_UsingInvalidId_ReturnException()
        {
            var templateService = GetInstance<ITextTemplateService>();
            templateService.GetSMSTextTemplateById(Guid.NewGuid());
        }


        #region private methods

        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        private SMSTextTemplate GetGroupTempalteForSave(long? department, long? organization, bool isActive, DateTime? fromDate, DateTime? todate, string name = "My Grouped Template", string text = "Reuseable string")
        {
            return new SMSTextTemplate()
            {
                TextTemplateTextId = null,
                HospitalID = m_hospitalId,
                DepartmentID = department,
                OrganizationID = organization,
                TextTemplateName = name,
                TextTemplateString = text,
                IsActive = isActive,
                ValidFrom = fromDate,
                ValidTo = todate
            };
        }

        private SMSText GetSmsTextTemplate(long hospitalid, long? depId, long? opd, 
            long? locId, long? secId, long? wardId, bool isActive, Guid? ruleId, 
            Guid? textTemplateId, Guid? groupTempId, Guid? ruleSetId, 
            bool isVideo = false, string smsText = "Test text", 
            string textTemplateName = "template name")
        {
            var smsTextModel = new SMSText
            {
                isActive = isActive,
                SMSTextTemplate = smsText,
                TextTemplateName = textTemplateName,
                HospitalId = hospitalid,
                TextTemplateId = textTemplateId,
                GroupTemplateId = groupTempId,
                RulesetId = ruleId,
                IsVideoAppoinment = isVideo
            };

            if (depId != null)
                smsTextModel.DepartmentId = (long)depId;
            if (locId != null)
                smsTextModel.LocationId = (long)locId;
            if (secId != null)
                smsTextModel.SectionId = (long)secId;
            if (wardId != null)
                smsTextModel.WardId = (long)wardId;
            if (opd != null)
                smsTextModel.OPDId = (long)opd;

            return smsTextModel;
        }
        private static void AddExcludedReshId(RuleSet ruleset, string reshid)
        {

            var orgUnit = new OrgUnit
            {
                UnitID = reshid
            };
            ruleset.ExcludeOrgUnits.Add(orgUnit);
        }

        private static RuleSet GetRuleSet(string rulesetName, long? depId, long hospitalId)
        {
            var orgUnitList = new List<OrgUnit>();

            return new RuleSet
            {
                RulesetName = rulesetName,
                DepartmentId = depId,
                HospitalId = hospitalId,
                AppointmentFrom = "8:30",
                AppointmentTo = "10:30",
                ExcludeOrgUnits = orgUnitList,
                IsActive = true,
                ExpireInDays = 2,
                IgnoreSMStoAdmittedPatient = true,
                ScheduleValidityPeriodFrom = DateTime.Now.Date.AddMonths(-1),
                ScheduleValidityPeriodTo = DateTime.Now.Date.AddMonths(2),
                SendBeforeInDays = SendBeforeInDays,
                SendingTimeIntervalFrom = "10:45",
                SendingTimeIntervalTo = "11:23",

            };
        }
        private SMSTextTemplate GetTempalteForSave(long? department, long? organization, bool isActive, DateTime? fromDate, DateTime? todate, string name = "My Grouped Template", string text = "Reuseable string")
        {
            return new SMSTextTemplate()
            {
                TextTemplateTextId = null,
                HospitalID = m_hospitalId,
                DepartmentID = department,
                OrganizationID = organization,
                TextTemplateName = name,
                TextTemplateString = text,
                IsActive = isActive,
                ValidFrom = fromDate,
                ValidTo = todate
            };
        }
        #endregion
    }
}
