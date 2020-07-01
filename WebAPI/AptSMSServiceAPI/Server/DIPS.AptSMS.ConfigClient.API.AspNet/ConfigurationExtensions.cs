using DIPS.AptSMS.ConfigClient.API.Common.Models;
using Microsoft.Extensions.Configuration;


namespace DIPS.AptSMS.ConfigClient.API.AspNet
{
    internal static class ConfigurationExtensions
    {
        internal static AppSettingConfig BuildAppSettings(this IConfiguration config)
        {
            var appData = new AppSettingConfig();
            //Dips DB Access
            var dataSource = config.GetValue<string>("Database:DataSource");
            var userName = config.GetValue<string>("Database:UserId") ?? "APT_APP";
            var password = config.GetValue<string>("Database:Password");

            appData.DataSourceString = $"Data Source={dataSource};User Id={userName};Password={password}";

            //Server Config
            appData.ServerUrl = config.GetValue<string>("Server:Url");

            return appData;
        }
    }
}
