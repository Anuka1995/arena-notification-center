using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System.Collections.Generic;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    [TestClass]
    public class OrgUnitWcfServiceTests : TestBase
    {
        //These tests are for testing the WCF calling to the organizationalUnit Service.

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetDepartmentListByHospitalId_Success()
        {
            var departmentList = m_OrgUnitService.GetDepartmentListByHospitalId(TestHospitalId, m_ticket);
            Assert.IsNotNull(departmentList);
            Assert.AreNotEqual(0, departmentList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetDepartmentListByHospitalId_NotValidHospitalId_ReturnedEmptyList()
        {
            var departmentList = m_OrgUnitService.GetDepartmentListByHospitalId(0, m_ticket);
            Assert.IsNotNull(departmentList);
            Assert.AreEqual(0, departmentList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetSectionsByDepartmentId_ValidDepartmentId_Success()
        {
            var sectionList = m_OrgUnitService.GetSectionListByDepartmentId(TestdepId_2, m_ticket);
            Assert.IsNotNull(sectionList);
            Assert.AreNotEqual(0, sectionList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetSectionsByDepartmentId_InvalidDepartmentId_ReturnedEmptyList()
        {
            var sectionList = m_OrgUnitService.GetSectionListByDepartmentId(0, m_ticket);
            Assert.IsNotNull(sectionList);
            Assert.AreEqual(0, sectionList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetWardByDepartmentId_ValidDepartmentId_Success()
        {
            var wardList = m_OrgUnitService.GetWardListByDepartmentId(TestdepId_2, m_ticket);
            Assert.IsNotNull(wardList);
            Assert.AreNotEqual(0, wardList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetWardsByDepartmentId_InvalidDepartmentId_ReturnedEmptyList()
        {
            var wardList = m_OrgUnitService.GetWardListByDepartmentId(0, m_ticket);
            Assert.IsNotNull(wardList);
            Assert.AreEqual(0, wardList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetLocationListForDepartmentId_ValidDepartmentId_Success()
        {
            var locationList = m_OrgUnitService.GetLocationListByDepartmentId(TestdepId_1, m_ticket);
            Assert.IsNotNull(locationList);
            Assert.AreNotEqual(0, locationList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetLocationListForDepartmentId_InvalidDepartmentId_ReturnedEmptyList()
        {
            var locationList = m_OrgUnitService.GetLocationListByDepartmentId(0, m_ticket);
            Assert.IsNotNull(locationList);
            Assert.AreEqual(0, locationList.Count);          
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOPDsByDepartmentId_ValidDepartmentId_Success()
        {
            var opdList = m_OrgUnitService.GetOPDListByDepartmentId(TestdepId_2, m_ticket);
            Assert.IsNotNull(opdList);
            Assert.AreNotEqual(0, opdList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOPDsByDepartmentId_InvalidDepartmentId_ReturnedEmptyList()
        {
            var opdList = m_OrgUnitService.GetOPDListByDepartmentId(0, m_ticket);
            Assert.IsNotNull(opdList);
            Assert.AreEqual(0, opdList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOrgUnitByDepartmentId_ValidDepartmentId_Success()
        {
            var department = m_OrgUnitService.GetOrgUnitByDepartmentId(TestdepId_2, m_ticket);
            Assert.IsNotNull(department);
            Assert.AreNotEqual(0, department.LocationList.Count);
            Assert.AreNotEqual(0, department.OPDList.Count);
            Assert.AreNotEqual(0, department.SectionList.Count);
            Assert.AreNotEqual(0, department.WardList.Count);
            //Wards by sectionid is not tested,becaz no test data is found for that.         

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetDepartmentByDepartmentId_ValidDepartmentId_Success()
        {
            var department = m_OrgUnitService.GetDepartmentByDepartmentId(TestdepId_1, m_ticket);
            Assert.IsNotNull(department);
            Assert.AreEqual(TestdepId_1, department.DepartmentId);
            Assert.AreEqual("Masteravd", department.DepartmentShortName);
            Assert.AreEqual("Test avdeling", department.DepartmentName);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetDepartmentListByDepartmentIdList_ValidDepartmentIdList_Success()
        {
            var dipList = new List<long?> { TestdepId_1, TestdepId_2 };
            var department = m_OrgUnitService.GetDepartmentListByDepartmentIdList(dipList, m_ticket);
            Assert.AreEqual(2, department.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOPDListByHospitalId_ValidHospitalId_Success()
        {
            var opdList = m_OrgUnitService.GetDepartmentListByHospitalId(TestHospitalId, m_ticket);
            Assert.IsNotNull(opdList);
            Assert.AreNotEqual(0, opdList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOPDListByHospitalId_NotValidHospitalId_ReturnEmpltyList()
        {
            var opdList = m_OrgUnitService.GetDepartmentListByHospitalId(0, m_ticket);
            Assert.IsNotNull(opdList);
            Assert.AreEqual(0, opdList.Count);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GetOrgUnitStructureForDepartment_ReturnReturnTreeNodeList()
        {
            var treeNodeList = m_OrgUnitService.GetOrgUnitStructureForDepartment(TestHospitalId, TestdepId_1, m_ticket);
            Assert.IsNotNull(treeNodeList);
            Assert.AreNotEqual(0, treeNodeList.Count);
        }
    }
}