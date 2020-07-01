using Dapper;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using DIPS.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class GroupedTextDataStore : BaseStore, IGroupedTextDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(GroupedTextDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public GroupedTextDataStore(DbProviderFactory providerFactory)
        {
            m_providerFactory = providerFactory;
        }

        public Guid CreateUpdateGroupText(GroupedTextDTO template)
        {
            string spName = "apt_code.SmsAppointmentService.createUpdateTextTemplate";
            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = GroupTemplateQueryParamHelper.GetGroupTemplateSaveUpdateQueryParams(template);
                    var newGuid = SaveAndReturnPK(spName, "l_SMSTextTempID", oracleParameters, connection);
                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("CreateUpdateGroupedText (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in CreateUpdateGroupedText", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("CreateUpdateGroupedText Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public Guid InsertGroupedText(GroupedTextDTO groupedTextDTO)
        {
            string spName = "apt_code.SmsAppointmentService.createTextTemplate";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = GroupTemplateQueryParamHelper.GetGroupTemplateSaveQueryParams(groupedTextDTO);
                    var newGuid = SaveAndReturnPK(spName, "l_SMSTextTempID", oracleParameters, connection);
                    return newGuid;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("InsertGroupedText (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in InsertGroupedText", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("InsertGroupedText Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public Guid UpdateGroupedText(GroupedTextDTO groupedTextDTO)
        {
            string spName = "apt_code.SmsAppointmentService.updateTextTemplate";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = GroupTemplateQueryParamHelper.GetGroupTemplateUpdateQueryParams(groupedTextDTO);
                    var newGuid = SaveAndReturnPK(spName, "l_newtextTemplateGUID", oracleParameters, connection);
                    return newGuid;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("UpdateGroupedText (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in UpdateGroupedText", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("UpdateGroupedText Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public GroupedTextDTO SelectAGroupedTextBy(Guid tempalteGuid)
        {
            const string spName = "apt_code.SmsAppointmentService.getTextTemplateByGuid";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = GroupTemplateQueryParamHelper.GetGroupTemplateByIdQueryParams(tempalteGuid);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Select GroupedText By Id returned empty or null result.");
                        throw new DBOperationException("SelectAGroupedTextBy returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var groupedTextDTOs = new List<GroupedTextDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        groupedTextDTOs.Add(MapToGroupedTextDTO(result));
                    }
                    return groupedTextDTOs.FirstOrDefault();
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectTagBy(Guid TemplateId) Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectByTemplate Guid )", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Select By(Template Guid) Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<GroupedTextDTO> SelectGroupedTextsOn(long? departmentID, string searchTerm, bool isActive, bool ishospitalOnly, long hospitalId)
        {

            const string spName = "apt_code.SmsAppointmentService.searchTextTemplate";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = GroupTemplateQueryParamHelper.GetGroupTemplateBySearchParams(departmentID, searchTerm, isActive, ishospitalOnly, hospitalId);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Search  Grouped Template Set returned empty or null result.");
                        throw new DBOperationException("Search  Grouped Template returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var groupedTextDTOs = new List<GroupedTextDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        groupedTextDTOs.Add(MapToGroupedTextDTO(result));
                    }
                    return groupedTextDTOs;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search grouped template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search grouped template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search grouped template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        private GroupedTextDTO MapToGroupedTextDTO(IDictionary<string, Object> dictionary)
        {

            var textTemplateGuid = dictionary["SMSTEXTTEMPLATEGUID"];
            var organizationId = dictionary["ORGANIZAIONID"];
            var hospitalId = dictionary["HOSPITALID"];
            var departmentId = dictionary["DEPARTMENTID"];
            var templateName = dictionary["TEMPLATENAME"];
            var smstext = dictionary["SMSTEXT"];
            var replacedByGuid = dictionary["REPLACEDBYGUID"];
            var validToDate = dictionary["VALIDTO"];
            var validFromDate = dictionary["VALIDFROM"];
            var isActive = dictionary["ISACTIVE"];

            GroupedTextDTO dto = new GroupedTextDTO();
            if (textTemplateGuid != null) dto.GroupedTempateGUID = new Guid((byte[])textTemplateGuid);
            if (replacedByGuid != null) dto.ReplacedByGUID = new Guid((byte[])replacedByGuid);

            if (organizationId != null) dto.OrganizationID = (long)organizationId;
            if (hospitalId != null) dto.HospitalID = (long)hospitalId;
            if (departmentId != null) dto.DepartmentID = (long)departmentId;
            if (templateName != null) dto.Name = templateName.ToString();
            if (smstext != null) dto.Text = smstext.ToString();
            if (validFromDate != null) dto.ValidFrom = (DateTime)validFromDate;
            if (validToDate != null) dto.ValidTo = (DateTime)validToDate;
            if (isActive != null) dto.IsActive = Convert.ToInt16(isActive) == 0 ? false : true;
            return dto;
        }
    }
}
