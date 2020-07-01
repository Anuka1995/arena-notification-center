using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    [TestClass]
    public class RuleSetsIntegrationTests : TestBase
    {
        private static readonly string depLevelruleSetName = "DEP_TESTRULESET2020";
        private static readonly string hospitalLevelruleSetName = "HOSPITAL_TESTRULESET2020";
        private static readonly long departmentId = 1;
        private static readonly long hospitalId = 1;
        private static readonly int SendBeforeInDays = 101;



        private readonly long HospitalID = 1;

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTests_GetAllActive_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            var ruleSets = ruleSetService.GetAllActiveRuleSets(HospitalID);

            var ruleSet = ruleSetService.GetRuleSetById(guid);
            Assert.IsNotNull(ruleSets);
            Assert.IsTrue(ruleSets.Any());

            foreach (var r in ruleSets)
            {
                Assert.IsTrue(r.IsActive);
            }
        }

        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_CreateRuleSet_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_DEPID_RULESETGUID_IsActive_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, DeparmentLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(1, guid.ToString(), false, false, hospitalId);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_DEPID_SEARCHTERM_IsActive_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(1, depLevelruleSetName, false, false, hospitalId);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_DEPID_SEARCHTERM_InActive_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            ruleset.IsActive = false;
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(1, depLevelruleSetName, true, false, hospitalId);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Count == 1);

        }
        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_HospitalLevel_SEARCHTERM_IsActive_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleSetHospitalLevel = GetHospitalLevelRuleSet();
            var guid = ruleSetService.SaveRuleSet(ruleSetHospitalLevel);
            Assert.IsNotNull(guid);

            var hospitalLevelRuleSets = ruleSetService.SearchRuleSet(null, hospitalLevelruleSetName, false, true, hospitalId);
            Assert.IsNotNull(hospitalLevelRuleSets);
            foreach (var ruleSet in hospitalLevelRuleSets)
            {
                Assert.IsTrue(ruleSet.DepartmentId == null);
                //Assert.IsTrue(ruleSet.IsActive);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_OnlyReturnRuleSets_ForGivenHospitalId()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleSetHospitalLevel = GetHospitalLevelRuleSet();
            var guid = ruleSetService.SaveRuleSet(ruleSetHospitalLevel);
            Assert.IsNotNull(guid);

            var hospitalLevelRuleSets = ruleSetService.SearchRuleSet(null, hospitalLevelruleSetName, false, true, hospitalId);
            Assert.IsNotNull(hospitalLevelRuleSets);
            foreach (var ruleSet in hospitalLevelRuleSets)
            {
                Assert.IsTrue(ruleSet.DepartmentId == null);
                //Assert.IsTrue(ruleSet.IsActive);
            }
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_OPDListForRuleSetId_DepartmentLevelRuleSet_ReturnFilterdOPDList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, DeparmentLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var opdList = ruleSetService.GetOPDListForRuleSetId(TestdepId_2, hospitalId, guid, m_ticket);
            Assert.IsNotNull(opdList);
            bool isAvailable = false;
            foreach (var _ in opdList.Where(opd => opd.UnitGid == DeparmentLevelOPDUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_OPDListForRuleSetId_HospitalLevelRuleSet_ReturnFilterdOPDList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var opdList = ruleSetService.GetOPDListForRuleSetId(null, hospitalId, guid, m_ticket);
            Assert.IsNotNull(opdList);
            bool isAvailable = false;
            foreach (var _ in opdList.Where(opd => opd.UnitGid == HospitalLevelOPDUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_GetSectionListForRuleSetId_ReturnFilteredSectionList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, SectionUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            var sectionList = ruleSetService.GetSectionListForRuleSetId(TestdepId_2, guid, m_ticket);
            Assert.IsNotNull(sectionList);
            bool isAvailable = false;
            foreach (var _ in sectionList.Where(opd => opd.UnitGid == SectionUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_GetWardListForRuleSetId_ForDepartment_ReturnFilteredWardList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, WardUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            var wardList = ruleSetService.GetWardListForRuleSetId(TestdepId_2, null, guid, m_ticket);
            Assert.IsNotNull(wardList);
            bool isAvailable = false;
            foreach (var _ in wardList.Where(opd => opd.UnitGid == WardUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }

        [Ignore("No enough test data available to test this")]
        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_GetWardListForRuleSetId_ForSection_ReturnFilteredWardList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, WardUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            var wardList = ruleSetService.GetWardListForRuleSetId(null, TestSectionId, guid, m_ticket);
            Assert.IsNotNull(wardList);
            bool isAvailable = false;
            foreach (var _ in wardList.Where(opd => opd.UnitGid == WardUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_GetLocationListForRuleSetId_ForDepartment_ReturnFilteredLocationList()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, LocationUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            var wardList = ruleSetService.GetLocationListForRuleSetId(TestdepId_2, guid, m_ticket);
            Assert.IsNotNull(wardList);
            bool isAvailable = false;
            foreach (var _ in wardList.Where(opd => opd.UnitGid == LocationUnitgid).Select(opd => new { }))
            {
                isAvailable = true;
            }
            Assert.IsFalse(isAvailable);
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_NULL_DEPID_NULL_SEARCHTERM_IsActive_GETALLSUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
           // AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(null, null, false, false, hospitalId);
            Assert.IsNotNull(ruleSetList);
            Assert.IsTrue(ruleSetList.Any(c => c.RulesetId == guid));

            var ruleSetList2 = ruleSetService.SearchRuleSet(null, null, false, false, TestHospitalId2);           
            Assert.IsFalse(ruleSetList2.Any(c => c.RulesetId == guid));
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_NULL_SEARCHTERM_IsActive_GETALLSUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(departmentId, null, false, false, hospitalId);
            Assert.IsTrue(ruleSetList.Count > 0);
            foreach (var ruleSet in ruleSetList)
            {
                Assert.IsTrue(ruleSet.DepartmentId == departmentId);
                // Assert.IsTrue(ruleSet.IsActive);
            }


        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_SearchRuleSet_By_NUll_DEPID_SEARCHTERM_IsActive_GETSUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSetList = ruleSetService.SearchRuleSet(null, "TEST", false, false, hospitalId);
            Assert.IsNotNull(ruleSetList);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_UpdateRuleSet_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var rulesetforeUpdate = ruleSetService.GetRuleSetById(guid);
            rulesetforeUpdate.ExpireInDays = 99;
            var updatedguid = ruleSetService.SaveRuleSet(rulesetforeUpdate);
            var rulesetAfterUpdate = ruleSetService.GetRuleSetById(updatedguid);
            Assert.IsTrue(rulesetAfterUpdate.ExpireInDays == rulesetforeUpdate.ExpireInDays);
            Assert.IsNotNull(updatedguid);

        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void RuleSetsIntegrationTest_GetRuleSet_By_RuleSetID_SUCCESS()
        {
            var ruleSetService = GetInstance<IRuleSetService>();
            var ruleset = GetRuleSet();
            AddExcludedReshId(ruleset, HospitalLevelOPDUnitgid);
            var guid = ruleSetService.SaveRuleSet(ruleset);
            Assert.IsNotNull(guid);

            var ruleSet = ruleSetService.GetRuleSetById(guid);
            Assert.IsNotNull(ruleSet);

        }

        #region Private Methods


        private static void AddExcludedReshId(RuleSet ruleset, string reshid)
        {
            var orgUnit = new OrgUnit
            {
                UnitID = reshid
            };
            ruleset.ExcludeOrgUnits.Add(orgUnit);
        }

        private static RuleSet GetRuleSet()
        {
            var orgUnitList = new List<OrgUnit>();

            return new RuleSet
            {
                RulesetName = depLevelruleSetName,
                DepartmentId = departmentId,
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

        private static RuleSet GetHospitalLevelRuleSet()
        {
            var orgUnitList = new List<OrgUnit>();

            return new RuleSet
            {
                RulesetName = hospitalLevelruleSetName,
                DepartmentId = null,
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

        #endregion
    }
}
