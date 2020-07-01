using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
{
    [TestClass]
    public class DateTimeFormatConfigIntegrationTests : TestBase
    {
        private readonly long HospitalId = 1;

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_Create_SUCCESS()
        {
            var service = GetInstance<IDateTimeFormatDataService>();
            var smsConfigurationDto = new SmsConfigurationDTO()
            {
                Name = "Test Format1",
                HospitalId = HospitalId,
                Value = "dd/MM/yy",
                IsActive = true
            };
            var newGuid = service.SaveDateTimeFormat(smsConfigurationDto);
            Assert.IsTrue(newGuid != null);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_UpdateFormat_SUCCESS()
        {
            var service = GetInstance<IDateTimeFormatDataService>();
            var dateTimeFormat = service.GetDateTimeFormatByHospital(1).First();
            dateTimeFormat.Value = "yyyy/MM/dd";
            var newGuid = service.SaveDateTimeFormat(dateTimeFormat);
            Assert.IsTrue(newGuid != null);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_UpdateStatus_SUCCESS()
        {
            var service = GetInstance<IDateTimeFormatDataService>();
            var dateTimeFormat = service.GetDateTimeFormatByHospital(HospitalId).First();
            dateTimeFormat.IsActive = false;
            var newGuid = service.SaveDateTimeFormat(dateTimeFormat);

            var updatedFormat = service.GetDateTimeFormatByHospital(HospitalId).Find(format => format.Id == newGuid);

            Assert.IsFalse(updatedFormat.IsActive);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void DateTimeFormatIntegration_GetFormats_SUCCESS()
        {
            var service = GetInstance<IDateTimeFormatDataService>();
            var list = service.GetDateTimeFormatByHospital(HospitalId);
            Assert.IsTrue(list.Any());
        }
    }
}
