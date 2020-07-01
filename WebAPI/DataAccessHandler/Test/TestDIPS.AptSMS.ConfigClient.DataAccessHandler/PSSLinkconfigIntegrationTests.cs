using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class PSSLinkconfigIntegrationTests : TestBase
    {
        private readonly long HospitalId = 1;

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_UpdateFormat_SUCCESS()
        {
            var service = GetInstance<IConfigPSSLinkDataService>();
            var pssConfigurationDto = service.GetPSSLinkByHospital(1).First();
            pssConfigurationDto.Value = "http://sl-kl-srv02.dipscloud.com:9997/";
            var newGuid = service.CreateUpdatePSSLink(pssConfigurationDto);
            Assert.IsTrue(newGuid != null);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_GetFormats_SUCCESS()
        {
            var service = GetInstance<IConfigPSSLinkDataService>();
            var list = service.GetPSSLinkByHospital(HospitalId);
            Assert.IsTrue(list.Any());
        }
    }
}
