using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class RuleSetIntegrationTests : TestBase
    {
        private readonly long DepartmentId = 88;

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateNew_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(getARuleSet(4, TestHospitalId1));

            Assert.IsNotNull(newguid);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateNew_Null_ExpireDays_null_SendBeforeDays_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleset = getARuleSet(4, TestHospitalId1);
            ruleset.SendSMSBeforeInMins = null;
            ruleset.SendSMSBeforeDays = 0;
            ruleset.DaysForRetryExpiry = null;
            var newguid = rulesetDatatService.SaveRuleSet(ruleset);
            Assert.IsNotNull(newguid);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateNew_EmptyExcludeOrgs_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(new List<string>()));

            Assert.IsNotNull(newguid);
        }

        [Ignore]
        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateNew_NullExcludeOrgs_FAIL()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            try
            {
                rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(null));
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
                Assert.IsTrue(e is DBOperationException);
            }
        }

        public void RuleSetIntegrationTests_CreateNew_WithMissingRequired_FAIL()
        {

        }

        public void RuleSetIntegrationTests_CreateNew_Duplicate_FAIL()
        {

        }

        public void RuleSetIntegrationTests_CreateNew_WithMissingOptional_SUCCESS()
        {

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_GetRuleSet_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(getARuleSet(2, TestHospitalId1));

            var ruleSet = rulesetDatatService.GetRuleSetBy(newguid);

            Assert.IsNotNull(ruleSet);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_Update_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(getARuleSet(2, TestHospitalId1));

            var ruleSet = rulesetDatatService.GetRuleSetBy(newguid);
            ruleSet.Name = "New ruleset name";

            var savedRulesetGuid = rulesetDatatService.SaveRuleSet(ruleSet);

            Assert.IsNotNull(savedRulesetGuid);

            Assert.AreNotEqual(savedRulesetGuid, newguid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_Department_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var savedGUID = rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1));

            var ruleSets = rulesetDatatService.SearchRuleSets(null, DepartmentId, null, true, false, TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_SearchHospitalLeveRuleSet_SearchByName_Success()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetDto = GetHospitalLevelRuleSet();
            var guid = rulesetDatatService.SaveRuleSet(ruleSetDto);
            var ruleSetList = rulesetDatatService.SearchRuleSets(null, null, "TEST_HOSPITL_LEVEL_RULE_999", true, true, TestHospitalId1);
            Assert.IsNotNull(guid);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_SearchHospitalLeveRuleSet_ByGuid_Success()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetDto = GetHospitalLevelRuleSet();
            var guid = rulesetDatatService.SaveRuleSet(ruleSetDto);
            var ruleSetList = rulesetDatatService.SearchRuleSets(guid, null, null, true, true, TestHospitalId1);
            Assert.IsNotNull(guid);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_SearchHospitalLeveRuleSet_ByName_InactiveRules_Success()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var ruleSetDto = GetHospitalLevelRuleSet();
            ruleSetDto.IsActive = false;
            var guid = rulesetDatatService.SaveRuleSet(ruleSetDto);
            var ruleSetList = rulesetDatatService.SearchRuleSets(null, null, "TEST_HOSPITL_LEVEL_RULE_999", false, true, TestHospitalId1);
            Assert.IsNotNull(guid);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_SearchTerm_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1, name: "Test Rule One"));
            rulesetDatatService.SaveRuleSet(getARuleSet(102, TestHospitalId1, name: "Test Rule Two"));

            var ruleSets = rulesetDatatService.SearchRuleSets(null, null, "Test", true, false, TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_GUID_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var savedGUID = rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1));

            var ruleSets = rulesetDatatService.SearchRuleSets(savedGUID, null, null, true, false, TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Count() == 1);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_ALL_Active_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1));
            rulesetDatatService.SaveRuleSet(getARuleSet(102, TestHospitalId1));
            rulesetDatatService.SaveRuleSet(getARuleSet(103, TestHospitalId1));
            rulesetDatatService.SaveRuleSet(getARuleSet(104, TestHospitalId1));

            var ruleSets = rulesetDatatService.SearchRuleSets(null, null, null, true, false, TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_ALL_InActive_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1, isActive: false));
            rulesetDatatService.SaveRuleSet(getARuleSet(102, TestHospitalId1, isActive: false));
            rulesetDatatService.SaveRuleSet(getARuleSet(103, TestHospitalId1, isActive: false));
            rulesetDatatService.SaveRuleSet(getARuleSet(104, TestHospitalId1, isActive: false));

            var ruleSets = rulesetDatatService.SearchRuleSets(null, null, null, false, false, TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_FilterRulesets_ReturnRuleSet_OnlyForGivenHospital_Success()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var guid1 = rulesetDatatService.SaveRuleSet(getARuleSet(101, TestHospitalId1));
            Assert.IsNotNull(guid1);
            var guid2 = rulesetDatatService.SaveRuleSet(getARuleSet(102, TestHospitalId2));
            Assert.IsNotNull(guid2);

            var ruleSet1 = rulesetDatatService.SearchRuleSets(null, null, null, true, false, TestHospitalId1);
            Assert.IsNotNull(ruleSet1);
            var isGuid1Exist = ruleSet1.Any(c => c.RuleSetGUID == guid1);
            Assert.IsTrue(isGuid1Exist);

            var ruleSet2 = rulesetDatatService.SearchRuleSets(null, null, null, true, false, TestHospitalId2);
            Assert.IsNotNull(ruleSet2);
            var isGuid2Exist = ruleSet2.Any(c => c.RuleSetGUID == guid2);
            Assert.IsTrue(isGuid1Exist);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_GetAllActive_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            rulesetDatatService.SaveRuleSet(getARuleSet(2, TestHospitalId1, name: "Rule Set One"));

            var ruleSets = rulesetDatatService.GetAllActiveRuleSets(TestHospitalId1);

            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());
            foreach (var r in ruleSets)
            {
                Assert.IsTrue(r.IsActive);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateRuleSet_WithEmptyExcludeOrgs_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(new List<string>()));

            Assert.IsNotNull(newguid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateRuleSet_WithNULLExcludeOrgs_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var newguid = rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(null));

            Assert.IsNotNull(newguid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateRuleSet_WithExcludedOrgs_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var orgIds = new List<string>() { "10", "345", "5678" };
            var newguid = rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(orgIds));

            Assert.IsNotNull(newguid);

            var ruleSet = rulesetDatatService.GetRuleSetBy(newguid);
            Assert.IsTrue(ruleSet.ExcludingOrgUnitIDs.Any());
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetIntegrationTests_CreateRuleSet_WithExcludedOrgs_UPDATE_SUCCESS()
        {
            var rulesetDatatService = GetInstance<IRuleSetDataService>();
            var orgIds = new List<string>() { "10", "345", "5678" };
            var newguid = rulesetDatatService.SaveRuleSet(GetARuleSetWithExcludedOrgUnits(orgIds));

            Assert.IsNotNull(newguid);

            var ruleSet = rulesetDatatService.GetRuleSetBy(newguid);

            ruleSet.ExcludingOrgUnitIDs = new List<string>() { "400" };

            Assert.IsTrue(ruleSet.ExcludingOrgUnitIDs.Any());

        }

        #region Private Functions
        private RuleSetDTO getARuleSet(int daysCount, long hospitalId, string name = "My Test RuleSet", bool isActive = true)
        {
            return new RuleSetDTO
            {
                Name = name,
                HospitalID = hospitalId,
                DepartmentID = DepartmentId,
                IsActive = isActive,
                RuleSetGUID = null,
                IgnoreSMStoAdmittedPatient = true,
                ExcludingOrgUnitIDs = new List<string>() { "20000", "29999", "9000" },
                isValidateAptTime = true,
                AptValidate_From = "10:00",
                AptValidate_To = "11:00",
                DaysForRetryExpiry = 12,
                SendingTimeWindowFrom = "09:00",
                SendingTimeWindowTo = "11:00",
                SendSMSBeforeDays = daysCount,
                SendSMSBeforeInMins = 12,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(1)
            };
        }

        private RuleSetDTO GetHospitalLevelRuleSet()
        {
            return new RuleSetDTO
            {
                Name = "TEST_HOSPITL_LEVEL_RULE_999",
                HospitalID = TestHospitalId1,
                DepartmentID = null,
                IsActive = true,
                RuleSetGUID = null,
                IgnoreSMStoAdmittedPatient = true,
                ExcludingOrgUnitIDs = new List<string>() { "20000", "29999", "9000" },
                isValidateAptTime = true,
                AptValidate_From = "10:00",
                AptValidate_To = "11:00",
                DaysForRetryExpiry = 12,
                SendingTimeWindowFrom = "09:00",
                SendingTimeWindowTo = "11:00",
                SendSMSBeforeDays = 333,
                SendSMSBeforeInMins = 12,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(1)
            };
        }
        private RuleSetDTO GetARuleSetWithExcludedOrgUnits(List<string> orgIDs, string name = "My Test RuleSet")
        {
            // orgIDs.Add("0");            
            return new RuleSetDTO
            {
                Name = name,
                HospitalID = TestHospitalId1,
                DepartmentID = DepartmentId,
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
                ValidTo = DateTime.Now.AddDays(1)
            };
        }
        #endregion
    }
}
