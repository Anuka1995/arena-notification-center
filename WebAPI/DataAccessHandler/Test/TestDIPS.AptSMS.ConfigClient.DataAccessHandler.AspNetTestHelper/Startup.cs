using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestDIPS.AptSMS.ConfigClient.DAH.AspNetTestHelper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Oracle database provider
            services.AddOracleDbProvider((sp, options) 
                => options.ConnectionString = Configuration.BuildConnectionString());

            services.AddDbConnection();           

            // Add authentication that supports both ticket and jwt 
            services.AddInternalAuthentication(Configuration, options =>
            {
                // Set to true to allow JWT. Requires security:authority configuration setting
                options.AllowJwt = false;
                options.AllowTicket = true;

                // Set security token (ticket) on dbConnection when creating new connection
                // Required when updating data in DIPS database
                options.SetSecurityTokenOnDbProviderFactory = true;
            });

            services.AddScoped<IRuleSetDataService, RuleSetDataService>();
            services.AddScoped<IRuleSetDataStore, RulesetDataStore>();

            services.AddScoped<ITagDataService, TagDataService>();
            services.AddScoped<ITagDataStore, TagDataStore>();

            services.AddScoped<IGroupedTextDataService, GroupedTemplateDataService>();
            services.AddScoped<IGroupedTextDataStore, GroupedTextDataStore>();

            services.AddScoped<ITextTemplateDataService, TextTemplateDataService>();
            services.AddScoped<ITextTemplateDataStore, TextTemplateDataStore>();

            services.AddScoped<IDIPSContactTypeDataService, DIPSContactTypeDataService>();
            services.AddScoped<IDIPSContactTypeDataStore, DIPSContactTypeDataStore>();
            
            services.AddScoped<IDateTimeFormatDataService, DateTimeFormatDataService>();
            services.AddScoped<IDateTimeFormatDataStore, DateTimeFormatDataStore>();

            services.AddScoped<IConfigPSSLinkDataService, ConfigPSSLinkDataService>();
            services.AddScoped<IConfigPSSLinkDataStore, ConfigPSSLinkDataStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
