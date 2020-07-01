using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Dapper;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using DIPS.Infrastructure.Logging;
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class DateTimeFormatDataStore : BaseStore, IDateTimeFormatDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(DateTimeFormatDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public DateTimeFormatDataStore(DbProviderFactory providerFactory)
        {
            m_providerFactory = providerFactory;
        }

        public Guid CreateUpdateDateTimeFormat(SmsConfigurationDTO smsConfigurationDto)
        {
            string spName = "apt_code.SmsAppointmentService.createUpdateConfigurationData";
            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = DateTimeFormatQueryParamsHelper.GetDateTimeFormatCreateUpdateQueryParams(smsConfigurationDto);
                    var newGuid = SaveAndReturnPK(spName, "l_configurationguid", oracleParameters, connection);
                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("CreateUpdateDateTimeFormat (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in CreateUpdateDateTimeFormat", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("CreateUpdateDateTimeFormat Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }



        public List<SmsConfigurationDTO> GetDateTimeFormatByHospital(long hospitalId)
        {
            string spName = "apt_code.SmsAppointmentService.getParameterValues";
            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = DateTimeFormatQueryParamsHelper.GetDateTimeFormatSearchQueryParams(hospitalId);

                    var results = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (results == null)
                    {
                        s_log.Error("GetDateTimeFormats returned empty or null result.");
                        throw new DBOperationException("GetDateTimeFormats returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var smsConfigurationDtos = new List<SmsConfigurationDTO>();
                    foreach (var result in results)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsConfigurationDtos.Add(mapToDateTimeFormatDto(resultDictionary));
                    }
                    return smsConfigurationDtos;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("CreateUpdateDateTimeFormat (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in CreateUpdateDateTimeFormat", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("CreateUpdateDateTimeFormat Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        private SmsConfigurationDTO mapToDateTimeFormatDto(IDictionary<string, object> dictionary)
        {

            var tagGuid = dictionary["CONFIGURATIONGUID"];
            var hospitalID = dictionary["HOSPITALID"];
            var parameterType = dictionary["PARAMETERTYPE"];
            var parameter = dictionary["PARAMETER"];
            var parameterValue = dictionary["PARAMETERVALUE"];
            var validUntil = dictionary["VALIDUNTILDATE"];

            var smsConfigurationDto = new SmsConfigurationDTO();

            if (tagGuid != null) smsConfigurationDto.Id = new Guid((byte[])tagGuid);

            smsConfigurationDto.HospitalId = (long)hospitalID;

            smsConfigurationDto.Name = parameter as string;
            smsConfigurationDto.Value = parameterValue as string;
            smsConfigurationDto.IsActive = validUntil == null;


            return smsConfigurationDto;

        }
    }
}
