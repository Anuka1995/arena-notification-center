using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class GroupedTextIntegrationTests : TestBase
    {
        private readonly long HospitalId = 1;
        private readonly long DepartmentId = 1;
        private readonly long OrganizationId = 1;


        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_CreateTempalteWithName_SUCCESS()
        {
            var groupedTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupedTextDataService.SaveGroupedText(groupedTextDTO);
            Assert.IsNotNull(guidId);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_UpdateTempalteWithId_SUCCESS()
        {
            var groupedTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupedTextDataService.SaveGroupedText(groupedTextDTO);

            var savedGroupedText = groupedTextDataService.GetGroupedTextBy(guidId);
            savedGroupedText.Name = "NewTempLateName";
            var newTemplateGuid = groupedTextDataService.SaveGroupedText(savedGroupedText);

            Assert.IsNotNull(guidId);
            Assert.IsNotNull(newTemplateGuid);

        }
        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_SelectAGroupedTextBy_GUID_SUCCESS()
        {
            var groupedTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupedTextDataService.SaveGroupedText(groupedTextDTO);
            Assert.IsNotNull(guidId);
            var templateDto = groupedTextDataService.GetGroupedTextBy(guidId);
            Assert.IsNotNull(templateDto);
            Assert.AreEqual(groupedTextDTO.ValidFrom.ToString(), templateDto.ValidFrom.ToString());
            Assert.AreEqual(groupedTextDTO.ValidTo.ToString(), templateDto.ValidTo.ToString());
            Assert.AreEqual(groupedTextDTO.IsActive, templateDto.IsActive);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_CreateTempalteWithTAGS_SUCCESS()
        {

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_CreateTempalteWithoutDepartment_SUCCESS()
        {
            var groupedTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            groupedTextDTO.DepartmentID = null;
            var guidId = groupedTextDataService.SaveGroupedText(groupedTextDTO);
            Assert.IsNotNull(guidId);
        }



        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_ByNullDepartment_SUCCESS()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            groupedTextDTO.DepartmentID = null;
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var templateList = groupTextDataService.SearchGroupedText(null, groupedTextDTO.Text, true, false, HospitalId);
            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Count == 1);

        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_ByDepartmentSearchTerm_SUCCESS()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var templateList = groupTextDataService.SearchGroupedText(groupedTextDTO.DepartmentID, groupedTextDTO.Text, true, false, HospitalId);
            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Count == 1);
            Assert.AreEqual(groupedTextDTO.ValidFrom.ToString(), templateList[0].ValidFrom.ToString());
            Assert.AreEqual(groupedTextDTO.ValidTo.ToString(), templateList[0].ValidTo.ToString());
            Assert.AreEqual(groupedTextDTO.IsActive, templateList[0].IsActive);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_EmptyResults_SUCCESS()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var departmentIdNotExist = 100;
            var textNotexists = "AAA";
            var templateList = groupTextDataService.SearchGroupedText(departmentIdNotExist, textNotexists, true, false, HospitalId);
            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Count == 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_InActive_Templates()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var templateList = groupTextDataService.SearchGroupedText(groupedTextDTO.DepartmentID, groupedTextDTO.Text, false, false, HospitalId);
            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Count == 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_GetHospitalLevelOnly_Templates()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var templateList = groupTextDataService.SearchGroupedText(null, groupedTextDTO.Text, true, false, HospitalId);
            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Count == 1);
            Assert.AreEqual(groupedTextDTO.ValidFrom.ToString(), templateList[0].ValidFrom.ToString());
            Assert.AreEqual(groupedTextDTO.ValidTo.ToString(), templateList[0].ValidTo.ToString());
            Assert.AreEqual(groupedTextDTO.IsActive, templateList[0].IsActive);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void GroupedTemplateIntegrationTest_FilterTempaltes_GetOnlyForGivenHospital_Success()
        {
            var groupTextDataService = GetInstance<IGroupedTextDataService>();
            var groupedTextDTO = GetGroupedTextDTO(HospitalId);
            var guidId = groupTextDataService.SaveGroupedText(groupedTextDTO);
            var templateList = groupTextDataService.SearchGroupedText(null, null, true, false, HospitalId);

            Assert.IsNotNull(guidId);
            Assert.IsNotNull(templateList);
            Assert.IsTrue(templateList.Any(c => c.GroupedTempateGUID == guidId));         

            var templateList1 = groupTextDataService.SearchGroupedText(null, null, true, false, TestHospitalId2);
            Assert.IsNotNull(templateList1);           
            Assert.IsFalse(templateList1.Any(c => c.GroupedTempateGUID == guidId));
        }

        #region private
        private GroupedTextDTO GetGroupedTextDTO(long hospitalId)
        {
            GroupedTextDTO dto = new GroupedTextDTO
            {
                OrganizationID = OrganizationId,
                DepartmentID = DepartmentId,
                HospitalID = hospitalId,
                Name = "GroupedTem_0001",
                Text = "Dear {patientName} !!",
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(2),
                IsActive = true
            };
            return dto;
        }
        # endregion
    }
}
