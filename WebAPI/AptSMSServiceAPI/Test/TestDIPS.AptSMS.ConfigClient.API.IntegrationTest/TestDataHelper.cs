using Microsoft.Extensions.DependencyInjection;


namespace TestDIPS.AptSMS.ConfigClient.API.IntegrationTest
{
    public class TestDataHelper
    {
        public static IServiceScope serviceScope;

        private TService GetInstance<TService>()
        {
            return serviceScope.ServiceProvider.GetRequiredService<TService>();
        }
    }
}
