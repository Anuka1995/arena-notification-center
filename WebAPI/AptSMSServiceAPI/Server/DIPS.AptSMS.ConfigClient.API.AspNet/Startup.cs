using DIPS.Infrastructure.HealthCheck;
using DIPS.Infrastructure.Profiling;
using DIPS.Infrastructure.Profiling.AspNetCore;
using DIPS.Infrastructure.Profiling.Sink.Logging;
using DIPS.Infrastructure.Security.Server.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.API.Server;
using log4net;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using System;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.API.Interface.WcfClientUtil;

namespace DIPS.AptSMS.ConfigClient.API.AspNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            var configuration = Configuration.BuildAppSettings();
            // Add Oracle database provider
            services.AddOracleDbProvider((sp, options)
                => options.ConnectionString = configuration.DataSourceString);

            services.AddDbConnection();

            // Add profiling using values from ApplicationConstants
            services.AddProfiling(options =>
            {
                options.ApplicationId = () => ApplicationConstants.ApplicationId;
                options.SessionId = () => ApplicationConstants.SessionId;
            });

            // Add profiling sink
            services.AddSingleton<IProfilingWriter>(sp => new LogProfilingWriter());

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

            services.AddScoped<IPSSLinkService, PSSLinkService>();
            services.AddScoped<IConfigPSSLinkDataService, ConfigPSSLinkDataService>();
            services.AddScoped<IConfigPSSLinkDataStore, ConfigPSSLinkDataStore>();


            services.AddScoped<IDateTimeFormatService, DateTimeFormatService>();
            services.AddScoped<IDateTimeFormatDataService, DateTimeFormatDataService>();
            services.AddScoped<IDateTimeFormatDataStore, DateTimeFormatDataStore>();

            services.AddScoped<IRuleSetService, RuleSetService>();
            services.AddScoped<IRuleSetDataService, RuleSetDataService>();
            services.AddScoped<IRuleSetDataStore, RulesetDataStore>();

            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagDataService, TagDataService>();
            services.AddScoped<ITagDataStore, TagDataStore>();

            services.AddScoped<IGroupedTextService, GroupedTextService>();
            services.AddScoped<IGroupedTextDataService, GroupedTemplateDataService>();
            services.AddScoped<IGroupedTextDataStore, GroupedTextDataStore>();

            services.AddScoped<ITextTemplateService, TextTemplateService>();
            services.AddScoped<ITextTemplateDataService, TextTemplateDataService>();
            services.AddScoped<ITextTemplateDataStore, TextTemplateDataStore>();


            services.AddScoped<IDIPSContactTypeService, DIPSContactTypeService>();
            services.AddScoped<IDIPSContactTypeDataService, DIPSContactTypeDataService>();
            services.AddScoped<IDIPSContactTypeDataStore, DIPSContactTypeDataStore>();

            services.AddScoped<IOverviewService, OverviewService>();

            //Inject HttpClient with Account service
            services.AddHttpClient<IAccountProvisionService, AccountProvisionService>(client =>
            {
                client.BaseAddress = new Uri(configuration.ServerUrl);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));

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

            // Add specific health checks here
            services.AddHealthChecks()
                .AddDatabaseConnectionHealthCheck();

            services.AddScoped<IOrgUnitsService, OrgUnitsService>();
            services.AddScoped<ITextTemplateService, TextTemplateService>();
            services.AddScoped<IWcfClient, WcfClient>();
            services.AddScoped<IWcfConfigurationProvider, WcfConfigurationProvider>();

            services.AddMemoryCache();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalContext.Properties["profilingCallId"] = app.ApplicationServices.GetRequiredService<ProfilingCallId>();
            app.UseCors("AllowOrigin");
            app.UseProfiling();

            app.AddHealthCheckEndpoints();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var theException = exceptionHandlerPathFeature?.Error;

                    if(theException is AccessDeniedException unAuthExp)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync($"{unAuthExp.Message}");
                    }
                    if (theException is DBOperationException dbException)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync($"Error! {dbException.Message}");
                    }
                    else if (theException is RESTCallException restException)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync($"{restException.Message}");
                    }
                    else if (theException is UserInputValidationException vException)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync($"Incorrect Inputs! {vException.Message}");
                        vException.EmptyInputs.ForEach(async i =>
                        {
                            await context.Response.WriteAsync($"Empty Param = {i}");
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "text/html";
                        var errorMessage = $"Error occured! {theException.Message}";

                        await context.Response.WriteAsync(errorMessage);
                    }
                });
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(builder => builder.MapControllers());
            
        }       
    }   
}
