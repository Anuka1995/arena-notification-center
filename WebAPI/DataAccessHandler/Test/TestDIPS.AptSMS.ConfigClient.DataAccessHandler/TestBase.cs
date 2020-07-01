using System.Transactions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIPS.Infrastructure.Security;
using DIPS.Infrastructure.Security.Common.Implementation;
using Microsoft.Extensions.Configuration;
using TestDIPS.AptSMS.ConfigClient.DAH.AspNetTestHelper;

namespace TestDIPS.AptSMS.ConfigClient.DAH.IntegrationTest
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
        protected readonly long TestHospitalId1 = 1;
        protected readonly long TestHospitalId2 = 21;
        protected long TestDepartmentId1 = 1;

        //Org unit belong to dep id 22
        protected long TestDepartmentId2 = 22;
        protected long TestLocationId = 11;
        protected long TestSectionId = 21;
        protected long TestWardId = 21;
        protected long TestOpdId = 11;

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

        protected virtual TService GetInstance<TService>()
        {
            return m_serviceScope.ServiceProvider.GetRequiredService<TService>();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            m_serviceScope = _testFactory.Server.Host.Services.CreateScope();
            m_transactionScope = new TransactionScope();
            TestDataHelper.serviceScope = m_serviceScope;
            var config = GetInstance<IConfiguration>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            m_serviceScope.Dispose();
            m_transactionScope.Dispose();
        }
    }
}
