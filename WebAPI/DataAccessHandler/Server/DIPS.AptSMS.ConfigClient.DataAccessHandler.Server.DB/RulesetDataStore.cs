using Dapper;
using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using DIPS.Infrastructure.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class RulesetDataStore : BaseStore, IRuleSetDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(RulesetDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public RulesetDataStore(DbProviderFactory providerFactory)
        {
            m_providerFactory = providerFactory;
        }

        public Guid InsertUpdateRuleSet(RuleSetDTO ruleSetDTO)
        {
            string spName = "apt_code.SmsAppointmentService.createUpdateRuleSet";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = RuleSetQueryParamsHelper.getQPForCreateORUpdateRuleSet(ruleSetDTO);
                    var newGuid = SaveAndReturnPK(spName, "l_ruleSetGuid", oracleParameters, connection);

                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("InsertUpdateRuleSet Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in InsertUpdateRuleSet", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("InsertUpdateRuleSet Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<RuleSetDTO> SelectAllActiveRuleSets(long hospitslId)
        {
            string spName = "apt_code.SmsAppointmentService.getActiveRuleSetList";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = RuleSetQueryParamsHelper.getQPForGetAllActiveRuleSets(hospitslId);
                    var resultSet = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);

                    if (resultSet == null)
                    {
                        s_log.Error("Select all active RuleSets returned empty or null result.");
                        throw new DBOperationException("SelectAllActiveRuleSets returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var mappedRuleSetDTOs = resultSet.ToList().Select(r => (RuleSetDTO)MapToRuleSetDTOForListItem(r)).ToList();

                    return mappedRuleSetDTOs;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectAllActiveRuleSets Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectAllActiveRuleSets", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectAllActiveRuleSets Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public RuleSetDTO SelectRuleSetBy(Guid ruleSetGuid)
        {
            const string spName = "apt_code.SmsAppointmentService.getRuleSetDetails";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparameters = RuleSetQueryParamsHelper.GetRuleSetByIdQueryParams(ruleSetGuid);

                    var resultSet = connection.Query(spName, oracleparameters, commandType: CommandType.StoredProcedure);

                    if (resultSet == null)
                    {
                        s_log.Error("Select RuleSet By Id returned empty or null result.");
                        throw new DBOperationException("SelectRuleSetBy returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var mappedRuleSetDTOs = resultSet.ToList().Select(r => MapToRuleSetDTO(r)).ToList();

                    if (mappedRuleSetDTOs.Select(p => p.RuleSetGUID).Distinct().Count() > 1)
                        throw new DBOperationException("Select RuleSet By Id returned more than one ruleset.");

                    RuleSetDTO dtoToReturn = mappedRuleSetDTOs.FirstOrDefault();
                    if (dtoToReturn == null)
                        throw new DBOperationException("SelectRuleSetBy returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);

                    if (dtoToReturn.ExcludingOrgUnitIDs == null)
                        dtoToReturn.ExcludingOrgUnitIDs = new List<string>();

                    return dtoToReturn;
                }
            }
            catch (OracleException ex)
            {
                s_log.ErrorException("SelectRuleSetBy(Guid RuleSetID) Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectRuleSetBy Guid DB function.)", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectRuleSetByID Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<RuleSetDTO> SelectRuleSetsOn(Guid? ruleSetGuid, long? departmentId, string searchTerm, bool? getActive,bool getHospitalLevel,long hospitalId)
        {
            const string spName = "apt_code.SmsAppointmentService.searchRuleset";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = RuleSetQueryParamsHelper.GetSelectRuleSetsOnQueryParams(ruleSetGuid, departmentId, searchTerm, getActive, getHospitalLevel, hospitalId);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("SelectRuleSetsOn returned empty or null result.");
                        throw new DBOperationException("SelectRuleSetsOn returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var mappedRuleSetDTOs = resultSet.ToList().Select(r => (RuleSetDTO)MapToRuleSetDTO(r)).ToList();

                    // Group the DTOs by Ruleset GUID
                    var groupedByRuleSetGUID =
                        from ruleDto in mappedRuleSetDTOs
                        group ruleDto by ruleDto.RuleSetGUID into newGroup
                        orderby newGroup.Key
                        select newGroup;

                    return groupedByRuleSetGUID.Select(ruleGroup => ruleGroup.FirstOrDefault()).ToList();
                }
            }
            catch (OracleException ex)
            {
                s_log.ErrorException("SelectRuleSetsOn Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectRuleSetsOn", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectRuleSetsOn Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        #region Private Methods
        private RuleSetDTO MapToRuleSetDTO(IDictionary<string, Object> dictionary)
        {
            var rulesetGuid = dictionary["RULESETGUID"];
            var hospitalID = dictionary["HOSPITALID"];
            var departmentID = dictionary["DEPARTMENTID"];
            var rulesetName = dictionary["RULESETNAME"];
            var sendBeforeDays = dictionary["SENDBEFOREDAYS"];
            var retryDaysCount = dictionary["RETRYEXPIREDAYS"];
            var validateApt = dictionary["VALIDATEAPTTIME"];
            var validAptToTime = dictionary["VALIDAPTTOTIME"];
            var validAptFromTime = dictionary["VALIDAPTFROMTIME"];
            var validFrom = dictionary["VALIDFROM"];
            var validTo = dictionary["VALIDTO"];
            var sendBeforeInMins = dictionary["SENDBEFOREINMINS"];
            var timeWindowFrom = dictionary["SCHSENDTIMEWINDOWFROM"];
            var timeWindowTo = dictionary["SCHSENDTIMEWINDOWTO"];
            var isActive = dictionary["ISACTIVE"];
            var IgnoreSMStoAdmittedPatient = dictionary["IGNOREADMITTEDPATIENT"];
            var replacedByGuid = dictionary["REPLACEDBYGUID"];
            var excludedOrgUnits = dictionary["RESHIDS"];

            RuleSetDTO dto = new RuleSetDTO();

            if (rulesetGuid != null) dto.RuleSetGUID = new Guid((byte[])rulesetGuid);
            if (replacedByGuid != null) dto.ReplacedByGUID = new Guid((byte[])replacedByGuid);
            if (validFrom != null) dto.ValidFrom = (DateTime)validFrom;
            if (validTo != null) dto.ValidTo = (DateTime)validTo;
            if (sendBeforeDays != null) dto.SendSMSBeforeDays = (int)sendBeforeDays;

            dto.HospitalID = (long)hospitalID;
            if (departmentID != null)
                dto.DepartmentID = (long)departmentID;
            else
                dto.DepartmentID = null;

            dto.Name = rulesetName as string;
            if (retryDaysCount != null) dto.DaysForRetryExpiry = (int)retryDaysCount;
            dto.isValidateAptTime = (Convert.ToInt16(isActive) == 0 ? false : true);
            dto.IsActive = (Convert.ToInt16(isActive) == 0 ? false : true);
            dto.isValidateAptTime = (Convert.ToInt16(validateApt) == 0 ? false : true);
            dto.AptValidate_From = validAptFromTime as string;
            dto.AptValidate_To = validAptToTime as string;
            if (sendBeforeInMins != null) dto.SendSMSBeforeInMins = Convert.ToInt32(sendBeforeInMins);
            dto.SendingTimeWindowFrom = timeWindowFrom as string;
            dto.SendingTimeWindowTo = timeWindowTo as string;
            dto.IgnoreSMStoAdmittedPatient = (Convert.ToInt16(IgnoreSMStoAdmittedPatient) == 0 ? false : true);
           
            if (excludedOrgUnits != null) dto.ExcludingOrgUnitIDs = excludedOrgUnits.ToString().Split(',').ToList();

            return dto;
        }

        private RuleSetDTO MapToRuleSetDTOForListItem(IDictionary<string, Object> dictionary)
        {
            var rulesetGuid = dictionary["RULESETGUID"];
            var rulesetName = dictionary["RULESETNAME"];
            var isActive = dictionary["ISACTIVE"];
            var hospitalID = dictionary["HOSPITALID"];
            var departmentID = dictionary["DEPARTMENTID"];

            RuleSetDTO dto = new RuleSetDTO();

            if (rulesetGuid != null) dto.RuleSetGUID = GuidConvert.FromRaw((byte[])rulesetGuid);
            dto.Name = rulesetName as string;
            dto.IsActive = (Convert.ToInt16(isActive) == 0 ? false : true);

            dto.HospitalID = (long)hospitalID;
            if (departmentID != null)
                dto.DepartmentID = (long)departmentID;
            else
                dto.DepartmentID = null;

            return dto;
        }
        #endregion
    }
}
