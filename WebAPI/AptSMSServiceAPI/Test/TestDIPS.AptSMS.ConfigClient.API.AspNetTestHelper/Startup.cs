using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil;
using DIPS.AptSMS.ConfigClient.API.Server;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using DIPS.Infrastructure.Security.Server.AspNetCore;
using DIPS.RabbitMqWrapper;
using DIPS.RabbitMqWrapper.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace TestDIPS.AptSMS.ConfigClient.API.AspNetTestHelper
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
            var config = Configuration.BuildAppSettings();

            // Add Oracle database provider
            services.AddOracleDbProvider((sp, options) 
                => options.ConnectionString = config.DataSourceString);

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

            services.AddScoped<IRuleSetService, RuleSetService>();
            services.AddScoped<IRuleSetDataService, RuleSetDataService>();
            services.AddScoped<IRuleSetDataStore, RulesetDataStore>();

            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagDataService, TagDataService>();
            services.AddScoped<ITagDataStore, TagDataStore>();

            services.AddScoped<ITextTemplateService, TextTemplateService>();
            services.AddScoped<ITextTemplateDataService, TextTemplateDataService>();
            services.AddScoped<ITextTemplateDataStore, TextTemplateDataStore>();

            services.AddScoped<IGroupedTextService, GroupedTextService>();
            services.AddScoped<IGroupedTextDataService, GroupedTemplateDataService>();
            services.AddScoped<IGroupedTextDataStore, GroupedTextDataStore>();

            services.AddScoped<IDIPSContactTypeService, DIPSContactTypeService>();
            services.AddScoped<IDIPSContactTypeDataService, DIPSContactTypeDataService>();
            services.AddScoped<IDIPSContactTypeDataStore, DIPSContactTypeDataStore>();

            services.AddScoped<IRuleSetService, RuleSetService>();
            services.AddScoped<IRuleSetDataService, RuleSetDataService>();
            services.AddScoped<IRuleSetDataStore, RulesetDataStore>();

            services.AddControllers()

               .AddMvcOptions(options =>
               {
                   // Require authorized user for all calls. To allow anonymous call add [AllowAnonymous] to method or controller 
                   var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                   options.Filters.Add(new AuthorizeFilter(policy));

                   // Rewrite ticket exception from Oracle DIPS database to 401
                   options.Filters.Add(new InternalSecurityExceptionFilter());

                   // Call cpSession.ClearSession when leaving service. This is required when using DIPS database and security token
                   options.Filters.Add(new InternalSecurityActionFilter());
               })
              .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped<IOrgUnitsService, OrgUnitsService>();
            services.AddScoped<IWcfClient, WcfClient>();
            services.AddScoped<IWcfConfigurationProvider, WcfConfigurationProvider>();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {           
            app.UseAuthentication();
            app.UseAuthorization();          
        }
    }
}
