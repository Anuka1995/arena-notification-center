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
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;
using System.Linq;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class TextTemplateDataStore : BaseStore, ITextTemplateDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(TextTemplateDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public TextTemplateDataStore(DbProviderFactory providerFactory)
        {
            m_providerFactory = providerFactory;
        }

        public Guid InsertOrUpdateTextTemplate(TextTemplateDTO templateDTO)
        {
            string spName = "apt_code.SmsAppointmentService.createUpdateSMSText";
            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetTextTemplateSaveUpdateQueryParams(templateDTO);
                    var newGuid = SaveAndReturnPK(spName, "l_SMSTEXTID", oracleParameters, connection);
                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("InsertOrUpdateTextTemplate (Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in InsertOrUpdateTextTemplate", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("InsertOrUpdateTextTemplate Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public TextTemplateDTO SelectTextTemplateById(Guid templateGuid)
        {
            string spName = "apt_code.SmsAppointmentService.getSMSTextByGuid";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetTextTemplateBYIdQueryParams(templateGuid);
                    var resultSet = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error($"GetSMSTextByGuid sp returned null result. No entry exist for template id - {templateGuid}");
                        throw new DBOperationException("Get SMStextTemplate By Id returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTOGetOne(resultDictionary));
                    }
                    return smsTextList.FirstOrDefault();
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Get SMStextTemplate By Id Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Get SMStextTemplate By Id", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Get SMStextTemplate By Id Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TextTemplateDTO> SelectTextTemplateOn(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly, long hospitalId)
        {
            string spName = "apt_code.SmsAppointmentService.searchSMSText";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetTextTemplateSearchQueryParams(departmetnID, opdID, sectionID, wardID, searchTerm, isActive, isHospitalOnly, hospitalId);
                    var resultSet = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Search SMSText Template Set returned null result.");
                        throw new DBOperationException("Search SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }
        
        public List<TextTemplateDTO> GetTextTemplatByWard(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare) {

            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchByWardID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByWardQueryParams(scheduleId, depId, wardId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Search SMSText Template Set returned null result.");
                        throw new DBOperationException("Enhanced Search By WardID returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TextTemplateDTO> GetTextTemplatByWard_BySections(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchBySecWardID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByWardBySectionQueryParams(scheduleId, depId, wardId, sectionId,hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Search SMSText Template Set returned null result.");
                        throw new DBOperationException("Enhanced Search By WardID returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }
        
        public List<TextTemplateDTO> GetTextTemplatByOPD(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchByOPDID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByOPDQueryParams(scheduleId, depId, opdId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Filter SMSText Template by OPD Set returned null result.");
                        throw new DBOperationException("Filter By OPD SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }
        
        public List<TextTemplateDTO> GetTextTemplateBySection(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchBySectionID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrBySectionQueryParams(scheduleId, depId, sectionId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Filter SMSText Template by OPD Set returned null result.");
                        throw new DBOperationException("Filter By Section, SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TextTemplateDTO> GetTextTemplatByDepartment(Guid scheduleId, long departmentID, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchByDeptID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByDepartmentQueryParams(scheduleId, departmentID, hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Filter SMSText Template by department Set returned null result.");
                        throw new DBOperationException("Filter By department, SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }
        
        public List<TextTemplateDTO> GetTextTemplatByHospital(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchByHospitalID";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByHospitalLevelQueryParams(scheduleId, hospitalId, locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Filter SMSText Template by Hospital, Set returned null result.");
                        throw new DBOperationException("Filter By Hospital level, SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TextTemplateDTO> GetGetTextTemplateByHospitalLevel_OPD(Guid scheduleId, long hospitalId, long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {

            string spName = "apt_code.SmsAppointmentService.getEnhancedSearchByHospitalOPD";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetFiletrByHospitalOPDLevelQueryParams(scheduleId, hospitalId,opdId ,locationId, contactTypes, officialLevelofcare);
                    var selectedSmsTextList = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (selectedSmsTextList == null)
                    {
                        s_log.Error("Filter SMSText Template by Hospital and OPD , Set returned null result.");
                        throw new DBOperationException("Filter By Hospital level and OPD, SMSText Template Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in selectedSmsTextList)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Search text template Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Search text template", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Search text template Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TextTemplateDTO> SelectFullTextTemplatesOn(bool isActive,long hospitalId)
        {
            string spName = "apt_code.SmsAppointmentService.getOverviewTree";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TextTemplateQueryParamsHelper.GetTextTemplateOverviewQueryParams(isActive,hospitalId);
                    var resultSet = connection.Query(spName, oracleParameters, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Get overview of text templates returned null result.");
                        throw new DBOperationException("Overview of all SMSText Templates Set returned null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }
                    var smsTextList = new List<TextTemplateDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        smsTextList.Add(MapToTextTemplateDTO(resultDictionary));
                    }
                    return smsTextList;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Get overview of text templates Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Get overview of text templates", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Get overview of text templates Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        #region private
        public TextTemplateDTO MapToTextTemplateDTOGetOne(IDictionary<string, object> dictionary)
        {
            var smsTextId = dictionary["SMSTEXTGUID"];
            var textTemplateName = dictionary["SMSTEXTNAME"];
            var ruleSetGuid = dictionary["RULESETGUID"];
            var hospitalId = dictionary["HOSPITALID"];
            var departmentId = dictionary["DEPARTMENTID"];
            var opdId = dictionary["OPDID"];
            var locationId = dictionary["LOCATIONID"];
            var sectionId = dictionary["SECTIONID"];
            var wardId = dictionary["WARDID"];
            var officailLevelCare = dictionary["OFFICIALLEVELOFCARE"];
            var contactType = dictionary["CONTACTTYPE"];
            var GroupTemplateId = dictionary["SMSTEXTTEMPLATEGUID"];
            var smsText = dictionary["SMSTEXT"];
            var groupSMSText = dictionary["GROUPSMSTEXT"];
            var validFrom = dictionary["VALIDFROM"];
            var validTo = dictionary["VALIDTO"];
            var isPSSLinkAvailable = dictionary["SENDPSSAPPLINK"];
            var isVideoAppoinment = dictionary["ISVIDEO"];
            var isActive = dictionary["ISACTIVE"];
            var isSMSGenColumnVal = dictionary["ISGENERATESMS"];

            var smsTextDto = new TextTemplateDTO();

            if (smsTextId != null) smsTextDto.TemplateGUID = GuidConvert.FromRaw((byte[])smsTextId);
            if (ruleSetGuid != null) smsTextDto.RuleSetGUID = GuidConvert.FromRaw((byte[])ruleSetGuid);
            if (GroupTemplateId != null)
            {
                smsTextDto.GroupedTextGUID = GuidConvert.FromRaw((byte[])GroupTemplateId);
                if (groupSMSText != null) smsTextDto.SMSText = groupSMSText.ToString();
            }
            else if (smsText != null) smsTextDto.SMSText = smsText.ToString();

            if (textTemplateName != null) smsTextDto.Name = textTemplateName.ToString();

            if (hospitalId != null) smsTextDto.HospitalID = (long)hospitalId;
            if (departmentId != null) smsTextDto.DepartmentID = (long)departmentId;
            if (opdId != null) smsTextDto.OPDID = (long)opdId;
            if (locationId != null) smsTextDto.LocationID = (long)locationId;
            if (sectionId != null) smsTextDto.SectionID = (long)sectionId;
            if (wardId != null) smsTextDto.WardID = (long)wardId;

            if (contactType != null)
            {
                smsTextDto.ContactType = contactType.ToString().Split(',').Select(conType => Convert.ToInt64(conType)).ToList();
            }
            if (officailLevelCare != null)
            {
                smsTextDto.OfficialLevelOfCare = officailLevelCare.ToString().Split(',').Select(offcarelvl => Convert.ToInt64(offcarelvl)).ToList();
            }

            if (validFrom != null) smsTextDto.ValidFrom = (DateTime)validFrom;
            if (validTo != null) smsTextDto.ValidTo = (DateTime)validTo;
            if (isActive != null) smsTextDto.IsActive = (Convert.ToInt16(isActive) == 0 ? false : true);
            if (isPSSLinkAvailable != null) smsTextDto.AttachPSSLink = (Convert.ToInt16(isPSSLinkAvailable) == 0 ? false : true);
            if (isVideoAppoinment != null) smsTextDto.IsVideoAppoinment = (Convert.ToInt16(isVideoAppoinment) == 0 ? false : true);
            if (isSMSGenColumnVal != null)
                smsTextDto.IsGenerateSMS = (Convert.ToInt16(isSMSGenColumnVal) == 0 ? false : true);

            return smsTextDto;
        }
        public TextTemplateDTO MapToTextTemplateDTO(IDictionary<string, object> dictionary)
        {
            var smsTextId = dictionary["SMSTEXTGUID"];
            var textTemplateName = dictionary["SMSTEXTNAME"];
            var ruleSetGuid = dictionary["RULESETGUID"];
            var hospitalId = dictionary["HOSPITALID"];
            var departmentId = dictionary["DEPARTMENTID"];
            var opdId = dictionary["OPDID"];
            var locationId = dictionary["LOCATIONID"];
            var sectionId = dictionary["SECTIONID"];
            var wardId = dictionary["WARDID"];
            var officailLevelCare = dictionary["OFFICIALLEVELOFCARE"];
            var contactType = dictionary["CONTACTTYPE"];
            var GroupTemplateId = dictionary["SMSTEXTTEMPLATEGUID"];
            var smsText = dictionary["SMSTEXT"];
            var groupSMSText = dictionary["GROUPSMSTEXT"];
            var validFrom = dictionary["VALIDFROM"];
            var isPSSLinkAvailable = dictionary["SENDPSSAPPLINK"];
            var isVideoAppoinment = dictionary["ISVIDEO"];
            var isSMSGenerate = dictionary["GENERATESMS"];
            var validTo = dictionary["VALIDTO"];
            var isActive = dictionary["ISACTIVE"];
            var rulesetName = dictionary["RULESETNAME"];
            var sendbeforedays = dictionary["SENDBEFOREDAYS"];
            var excludedOrgUnits = dictionary["RESHIDS"];
            var templateDepartmentId = dictionary["TEXTDEPTID"];

            var smsTextDto = new TextTemplateDTO();

            if (smsTextId != null) smsTextDto.TemplateGUID = GuidConvert.FromRaw((byte[])smsTextId);
            if (ruleSetGuid != null) smsTextDto.RuleSetGUID = GuidConvert.FromRaw((byte[])ruleSetGuid);
            if (GroupTemplateId != null)
            {
                smsTextDto.GroupedTextGUID = GuidConvert.FromRaw((byte[])GroupTemplateId);
                if (groupSMSText != null) smsTextDto.SMSText = groupSMSText.ToString();
            }
            else
             if (smsText != null) smsTextDto.SMSText = smsText.ToString();

            if (textTemplateName != null) smsTextDto.Name = textTemplateName.ToString();

            if (hospitalId != null) smsTextDto.HospitalID = (long)hospitalId;
            if (departmentId != null) smsTextDto.DepartmentID = (long)departmentId;
            if (templateDepartmentId != null) smsTextDto.TemplateDepartmentID = (long)templateDepartmentId;
            if (opdId != null) smsTextDto.OPDID = (long)opdId;
            if (locationId != null) smsTextDto.LocationID = (long)locationId;
            if (sectionId != null) smsTextDto.SectionID = (long)sectionId;
            if (wardId != null) smsTextDto.WardID = (long)wardId;

            if (contactType != null)
            {
                smsTextDto.ContactType = contactType.ToString().Split(',').Select(conType => Convert.ToInt64(conType)).ToList();
            }
            if (officailLevelCare != null)
            {
                smsTextDto.OfficialLevelOfCare = officailLevelCare.ToString().Split(',').Select(offcarelvl => Convert.ToInt64(offcarelvl)).ToList();
            }

            if (validFrom != null) smsTextDto.ValidFrom = (DateTime)validFrom;
            if (validTo != null) smsTextDto.ValidTo = (DateTime)validTo;
            if (isActive != null) smsTextDto.IsActive = (Convert.ToInt16(isActive) == 0 ? false : true);
            if (isPSSLinkAvailable != null) smsTextDto.AttachPSSLink = (Convert.ToInt16(isPSSLinkAvailable) == 0 ? false : true);
            if (isVideoAppoinment != null) smsTextDto.IsVideoAppoinment = (Convert.ToInt16(isVideoAppoinment) == 0 ? false : true);
            if (isSMSGenerate != null) 
                smsTextDto.IsGenerateSMS = (Convert.ToInt16(isSMSGenerate) == 0 ? false : true);

            if (rulesetName != null) smsTextDto.RuleSetName = rulesetName.ToString();

            if (excludedOrgUnits != null) smsTextDto.ExcludedOrgUnits = excludedOrgUnits.ToString().Split(',').ToList();
            if (sendbeforedays != null) smsTextDto.SendSMSBeforeDays = Convert.ToInt32(sendbeforedays);
            return smsTextDto;
        }

        #endregion

    }
}
