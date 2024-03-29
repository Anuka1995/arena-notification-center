﻿using System.Transactions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDIPS.AptSMS.ConfigClient.API.AspNetTestHelper;
using DIPS.Infrastructure.Security;
using DIPS.Infrastructure.Security.Common.Implementation;
using DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil;
using DIPS.AptSMS.ConfigClient.API.Interface;
using System;
using Microsoft.Extensions.Configuration;

namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    public class TestBase
    {
        // Factory for creating clients for the service that is being tested.
        private readonly WebApplicationFactory<Startup> _serviceUnderTestFactory;
        // Factory for creating test services to add testdata etc.
        private readonly WebApplicationFactory<Startup> _testFactory;
        // Scope for test classes, should be disposed on cleanup
        protected IServiceScope m_serviceScope;
        private TransactionScope m_transactionScope;
        protected IWcfClient m_wcfClient;
        protected IWcfConfigurationProvider m_configurationProvider;
        protected IOrgUnitsService m_OrgUnitService;
        protected Guid m_ticket = new Guid("02591eea-57de-4d5a-9d66-c7f968c81afa");
        protected long TestHospitalId = 1;
        protected long TestHospitalId2 = 21;
        protected long TestdepId_1;
        protected long TestdepId_2;
        protected readonly long TestLocationId = 11;
        protected readonly long TestSectionId = 21;
        protected readonly long TestWardId = 21;
        protected readonly long TestOpdId = 11;
        protected readonly string DeparmentLevelOPDUnitgid = "aabaea+0000000000011";
        protected readonly string HospitalLevelOPDUnitgid = "aabaea+0000000000010";
        protected readonly string SectionUnitgid = "aabaju+0000001000003";
        protected readonly string WardUnitgid = "aabahl+0000000000021";
        protected readonly string LocationUnitgid = "aabaea+0000000000011";

        public TestBase()
        {
            _serviceUnderTestFactory = new WebApplicationFactory<Startup>();

            _testFactory = _serviceUnderTestFactory.WithWebHostBuilder(config =>
            {
                config.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ISecurityTokenStorage>(sp =>
                    {
                        var storage = new GlobalSecurityTokenStorage();
                        //storage.Set(SecurityConstants.TicketStorageConstant, "fad3f55a-b3a3-455f-80b0-bd4f82c29bf6");
                        storage.Set(SecurityConstants.TicketStorageConstant, "02591eea-57de-4d5a-9d66-c7f968c81afa");
                        return storage;
                    });
                });
            });
            _testFactory.CreateDefaultClient();
        }

        private TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            m_serviceScope = _testFactory.Server.Host.Services.CreateScope();
            m_transactionScope = new TransactionScope();
            TestDataHelper.serviceScope = m_serviceScope;
            m_wcfClient = GetInstance<IWcfClient>();
            m_configurationProvider = GetInstance<IWcfConfigurationProvider>();
            m_OrgUnitService = GetInstance<IOrgUnitsService>();
            var config = GetInstance<IConfiguration>();
            TestdepId_1 = config.GetValue<long>("TestDepid1", 1);
            TestdepId_2 = config.GetValue<long>("TestDepid2", 22);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            m_serviceScope.Dispose();
            m_transactionScope.Dispose();
        }
    }
}
