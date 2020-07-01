using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    public static class RuleSetQueryParamsHelper
    {
        internal static OracleDynamicParameters getQPForCreateORUpdateRuleSet(RuleSetDTO ruleSetDTO)
        {
            var isactive = (ruleSetDTO.IsActive ? 1 : 0);
            var isValidateAptTime = (ruleSetDTO.isValidateAptTime ? 1 : 0);
            var sendSMSIfAdmitted = (ruleSetDTO.IgnoreSMStoAdmittedPatient ? 1 : 0);

            OracleDynamicParameters dynamicParameters = AssocativeArraysConverter
                .ConvertCollectionToPLSQLAssociativeArray(ruleSetDTO.ExcludingOrgUnitIDs, "p_excludedReshIDs");

            dynamicParameters.Add("p_oldRulesetGuid", GuidConvert.ToRaw(ruleSetDTO.RuleSetGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", ruleSetDTO.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", ruleSetDTO.DepartmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_ruleSetName", ruleSetDTO.Name, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_sendBeforeDays", ruleSetDTO.SendSMSBeforeDays, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_retryExpireDays", ruleSetDTO.DaysForRetryExpiry, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_validateAptTime", isValidateAptTime, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_validAptFromTime", ruleSetDTO.AptValidate_From, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_validAptToTime", ruleSetDTO.AptValidate_To, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_validFrom", ruleSetDTO.ValidFrom, OracleMappingType.TimeStamp, ParameterDirection.InputOutput, 25);
            dynamicParameters.Add("p_validTo", ruleSetDTO.ValidTo, OracleMappingType.TimeStamp, ParameterDirection.InputOutput, 25);
            dynamicParameters.Add("p_timeWindowFrom", ruleSetDTO.SendingTimeWindowFrom, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_timeWindowTo", ruleSetDTO.SendingTimeWindowTo, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_sendBefore", ruleSetDTO.SendSMSBeforeInMins, OracleMappingType.Int16, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_ignoreForAdmitted", sendSMSIfAdmitted, OracleMappingType.Int16, ParameterDirection.Input);

            dynamicParameters.Add("l_ruleSetGuid", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);

            return dynamicParameters;
        }

        internal static OracleDynamicParameters getQPForGetAllActiveRuleSets(long hospitalId)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("l_activeRulesets ", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
           
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetRuleSetByIdQueryParams(Guid ruleSetGuid)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_ruleSetGuid ", GuidConvert.ToRaw(ruleSetGuid), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("l_ruleSetDetails ", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetSelectRuleSetsOnQueryParams(Guid? ruleSetGuid, long? departmetnID, string searchTerm, bool? getActive,bool getHospitalLevel,long hospitalId)
        {
            int? getActiveValue = null;
            if (getActive != null)
                getActiveValue = ((bool)getActive ? 1 : 0);

            int getHospitalLevelValue = (getHospitalLevel ? 1 : 0 );
           
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_departmentId", departmetnID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_rulesetGUID ", GuidConvert.ToRaw(ruleSetGuid), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_rulesetName", searchTerm, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_isActive", getActiveValue, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalOnly", getHospitalLevelValue, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("l_ruleSet ", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);

            return dynamicParameters;
        }
    }
}
