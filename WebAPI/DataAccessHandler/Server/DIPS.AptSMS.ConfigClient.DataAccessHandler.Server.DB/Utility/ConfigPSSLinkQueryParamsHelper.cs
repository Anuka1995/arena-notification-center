using Dapper.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    internal static class ConfigPSSLinkQueryParamsHelper
    {
        internal static OracleDynamicParameters GetPSSLinkCreateUpdateQueryParams(SmsConfigurationDTO smsConfigurationDto)
        {
            var isActiveIntVal = 1; // we don't support to inactive the link status
            var parameterType = (short)GlobalOptions.ConfigurationParameterType.PSSLink;
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

        internal static OracleDynamicParameters GetPSSLinkSearchQueryParams(long? hospitalId)
        {
            var parameterType = (short)GlobalOptions.ConfigurationParameterType.PSSLink;
            var dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_parametertype", parameterType, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("l_parameterValues", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 300);

            return dynamicParameters;
        }
    }
}
