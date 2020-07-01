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
    public class DIPSContactTypeIntegrationTests : TestBase
    {
        [TestMethod, TestCategory("IntegrationTest")]
        public void DIPSContactTypeIntegrationTest_getOfficialLevelOfCare_SUCCESS()
        {
            var officialLevelOfCareService = GetInstance<IDIPSContactTypeService>();

            var officialLevelOfCareList = officialLevelOfCareService.GetOfficialLevelOfCareInfo();
            Assert.IsNotNull(officialLevelOfCareList);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DIPSContactTypeIntegrationTest_getContactType_SUCCESS()
        {
            var contactTypeService = GetInstance<IDIPSContactTypeService>();

            var contactTypeServiceList = contactTypeService.GetContactTypeInfo();
            Assert.IsNotNull(contactTypeServiceList);
        }

        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
