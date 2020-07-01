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
    public class DIPSContactTypeIntegrationTests : TestBase
    {
        
        [TestMethod, TestCategory("IntegrationTest")]
        public void DIPSContactTypeIntegrationTests_GetOfficialLevelOfCareDetails_SUCCESS()
        {

            var DIPSContactTypeDataService = GetInstance<IDIPSContactTypeDataService>();
            var templateDto = DIPSContactTypeDataService.GetOfficialLevelOfCareDetails();
            Assert.IsNotNull(templateDto);
            Assert.IsTrue(templateDto.Count > 0);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DIPSContactTypeIntegrationTests_GetContactTypeDetails_SUCCESS()
        {
            var DIPSContactTypeDataService = GetInstance<IDIPSContactTypeDataService>();
            var templateDto = DIPSContactTypeDataService.GetContactTypeDetails();
            Assert.IsNotNull(templateDto);
            Assert.IsTrue(templateDto.Count > 0);
        }
        #region private

        #endregion
    }
}
