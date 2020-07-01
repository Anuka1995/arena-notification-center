using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    [TestClass]
    public class GroupedTemplateIntegrationTests : TestBase
    {
        private readonly long HospitalId = 1;
        private readonly long DepartmentId = 1;
        private readonly long DepartmentId2 = 22;
        private readonly long? EmptyDepartmentId = null;
        private readonly long OrganizationId = 1;
        private readonly long? EmptyOrganizationId = null;

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_CreateGroupedTemplate_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();
            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(DepartmentId, OrganizationId, true, DateTime.Now, DateTime.Now.AddDays(1)));

            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_CreateGlobalGroupedTemplate_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();

            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(EmptyDepartmentId, OrganizationId, true, DateTime.Now, DateTime.Now.AddDays(1)));

            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_CreateGroupedTemplateEmptyOrg_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();

            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(EmptyDepartmentId, EmptyOrganizationId, true, DateTime.Now, DateTime.Now.AddDays(1)));

            Assert.IsNotNull(guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SaveGetGroupedTemplate_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();
            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(DepartmentId, OrganizationId,true, DateTime.Now, DateTime.Now.AddDays(1)));

            var template = templateService.GetTextTemplateBy(guid);

            Assert.IsNotNull(template);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_SaveGetGroupedTemplate_Validate_SUCCESS()
        {
            //Validate ToDate,FromDate,Isactive
            var templateService = GetInstance<IGroupedTextService>();
            var toDate = DateTime.Now.AddDays(1);
            var FromDate = DateTime.Now;
            bool isActive = true;
            var template = GetTempalteForSave(DepartmentId, OrganizationId, isActive, FromDate, toDate);
            Assert.IsNotNull(template);
            var guid = templateService.SaveGroupedTemplate(template);

            var resultTemplate = templateService.GetTextTemplateBy(guid);
            Assert.AreEqual(isActive, resultTemplate.IsActive);
            Assert.AreEqual(FromDate.ToString(), resultTemplate.ValidFrom.ToString());
            Assert.AreEqual(toDate.ToString(), resultTemplate.ValidTo.ToString());

            //test for making inactive the template
            resultTemplate.IsActive = false;
            var guid1 = templateService.SaveGroupedTemplate(resultTemplate);
            var resultTemplate1 = templateService.GetTextTemplateBy(guid1);
            Assert.AreEqual(false, resultTemplate1.IsActive);
            Assert.AreEqual(FromDate.ToString(), resultTemplate1.ValidFrom.ToString());
            Assert.AreEqual(toDate.ToString(), resultTemplate1.ValidTo.ToString());           
        }


        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_UpdateGroupedTemplate_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();
            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(DepartmentId, OrganizationId,true, DateTime.Now, DateTime.Now.AddDays(1)));

            var groupedTemplate = templateService.GetTextTemplateBy(guid);
            groupedTemplate.TextTemplateString = "My Updated string";

            var newGuid = templateService.SaveGroupedTemplate(groupedTemplate);
            Assert.IsNotNull(newGuid);
            Assert.AreNotEqual(newGuid, guid);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void TagIntegrationTest_FilterTemplate_SUCCESS()
        {
            var templateService = GetInstance<IGroupedTextService>();
            var guid = templateService.SaveGroupedTemplate(GetTempalteForSave(DepartmentId, OrganizationId, true, DateTime.Now, DateTime.Now.AddDays(1), name: "Text template reuse"));
            var guid2 = templateService.SaveGroupedTemplate(GetTempalteForSave(DepartmentId2, OrganizationId, true, DateTime.Now, DateTime.Now.AddDays(1), name: "Text template 2 reuse"));

            var templateList = templateService.FilterTextTemplatesBy(DepartmentId, null, m_ticket, false, false, HospitalId);
            bool isAvailable = false;
            foreach (var template in templateList)
            {
                if (template.DepartmentID == DepartmentId && template.TextTemplateName.Equals("Text template reuse"))
                {
                    isAvailable = true;
                }
            }
            Assert.IsNotNull(templateList);
            Assert.IsTrue(isAvailable);


            var templateList1 = templateService.FilterTextTemplatesBy(null, "template", m_ticket, false, false, HospitalId);
            if (templateList1.Count >= 2)
            {
                Assert.IsTrue(true);
            }

            var templateList3 = templateService.FilterTextTemplatesBy(DepartmentId, "Text template reuse", m_ticket, false, false, HospitalId);
            if (templateList3.Count >= 1)
            {
                Assert.IsTrue(true);
            }

        }

        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        private SMSTextTemplate GetTempalteForSave(long? department, long? organization, bool isActive, DateTime? fromDate, DateTime? todate, string name = "My Grouped Template", string text = "Reuseable string")
        {
            return new SMSTextTemplate()
            {
                TextTemplateTextId = null,
                HospitalID = HospitalId,
                DepartmentID = department,
                OrganizationID = organization,
                TextTemplateName = name,
                TextTemplateString = text,
                IsActive = isActive,
                ValidFrom = fromDate,
                ValidTo = todate
            };
        }
    }
}
