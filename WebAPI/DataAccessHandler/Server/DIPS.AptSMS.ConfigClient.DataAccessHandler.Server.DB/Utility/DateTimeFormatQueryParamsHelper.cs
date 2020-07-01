using Dapper.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    static class DateTimeFormatQueryParamsHelper
    {
        internal static OracleDynamicParameters GetDateTimeFormatCreateUpdateQueryParams(SmsConfigurationDTO smsConfigurationDto)
        {
            var isActiveIntVal = smsConfigurationDto.IsActive ? 1 : 0;
            var parameterType = (short)GlobalOptions.ConfigurationParameterType.DateTimeFormat;
            var dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_configurationguid", GuidConvert.ToRaw(smsConfigurationDto.Id), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", smsConfigurationDto.HospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_parametertype", parameterType, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_parameter", smsConfigurationDto.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_parametervalue", smsConfigurationDto.Value, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isActiveIntVal, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("l_configurationguid", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetDateTimeFormatSearchQueryParams(long? hospitalId)
        {
            var parameterType = (short)GlobalOptions.ConfigurationParameterType.DateTimeFormat;
            var dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_parametertype", parameterType, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("l_parameterValues", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 300);

            return dynamicParameters;
        }
    }
}
