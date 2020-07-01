using Microsoft.Extensions.Configuration;


namespace TestDIPS.AptSMS.ConfigClient.DAH.AspNetTestHelper
{
    internal static class ConfigurationExtensions
    {
        internal static string BuildConnectionString(this IConfiguration config)
        {
            var dataSource = config.GetValue<string>("Database:DataSource");
            var userName = config.GetValue<string>("Database:APT_APP:UserId") ?? "apt_app";
            var password = config.GetValue<string>("Database:APT_APP:Password");
            return $"Data Source={dataSource};User Id={userName};Password={password}";
        }
        
    }
}
