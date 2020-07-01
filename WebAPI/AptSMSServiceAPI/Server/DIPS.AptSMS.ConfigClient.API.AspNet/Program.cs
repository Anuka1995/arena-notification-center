using System.IO;
using System.Reflection;
using DIPS.Configuration.Client.Log4net;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace DIPS.AptSMS.ConfigClient.API.AspNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            // Log4net setup
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            GlobalContext.Properties["applicationid"] = ApplicationConstants.ApplicationId;
            GlobalContext.Properties["sessionid"] = ApplicationConstants.SessionId;

            var ret = WebHost.CreateDefaultBuilder(args)
             .UseStartup<Startup>()
            .ConfigureAppConfiguration((context, builder) =>
            {
                // Load local configuration first, is used to configure central configuration location
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                    .AddJsonFile("appsettings.json", false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();

                // Configuration Client should be first, allowing others to override values from central location
                if (configuration.GetValue<bool>("CentralConfig:Enabled"))
                {
                  builder
                        // Add configuration client provider to IConfiguration
                        .AddConfigurationClient(clientOptions =>
                        {
                            clientOptions.ApplicationName = ApplicationConstants.ApplicationName;

                            // Read from default config the location of central configuration
                            clientOptions.ReadFromDefaultConfiguration(configuration);

                            // Add log4net adapter, letting ConfigurationClient configure this adapter
                            clientOptions.LoggingAdapter = new Log4netConfigurationAdapter();
                        });
                }
                // add the configuration initialized above
                builder.AddConfiguration(configuration);
            })
            .ConfigureLogging((context, builder) =>
            {
                // Configure MS log to send log statements to configured log framework
                builder.AddLog4Net(new Log4NetProviderOptions
                {
                    ExternalConfigurationSetup = true
                });
            });
            
            return ret;
        }
    }
}
