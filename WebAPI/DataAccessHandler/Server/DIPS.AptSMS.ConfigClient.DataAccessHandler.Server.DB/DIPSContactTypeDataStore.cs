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
    public class DIPSContactTypeDataStore : BaseStore, IDIPSContactTypeDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(DIPSContactTypeDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public DIPSContactTypeDataStore(DbProviderFactory providerFactory)
        {
            m_providerFactory = providerFactory;
        }

        public List<OfficialLevelOfCareDTO> SelectOfficialLevelOfCare()
        {
            const string spName = "apt_code.SmsAppointmentService.getOfficialLevelOfCare";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = DIPSContactTypeQueryParamHelper.GetOfficialLevelOfCareQueryParams();

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Select Official Level Of Care returned empty or null result.");
                        throw new DBOperationException("Select Official Level Of Care returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var officialLevelOfCareDTOs = new List<OfficialLevelOfCareDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        officialLevelOfCareDTOs.Add(MapToOfficialLevelOfCareDTO(result));
                    }
                    return officialLevelOfCareDTOs;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectOfficialLevelOfCare() Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in Select Official Level Of Care)", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectOfficialLevelOfCare() Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<ContactTypeDTO> SelectContactType()
        {
            const string spName = "apt_code.SmsAppointmentService.getContactType";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = DIPSContactTypeQueryParamHelper.GetContactTypeQueryParams();

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("Select contact type returned empty or null result.");
                        throw new DBOperationException("Select contact type returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var contactTypeDTOs = new List<ContactTypeDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        contactTypeDTOs.Add(MapToContactTypeDTO(result));
                    }
                    return contactTypeDTOs;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectContactType() Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in select contact type)", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectContactType() Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        private OfficialLevelOfCareDTO MapToOfficialLevelOfCareDTO(IDictionary<string, Object> dictionary)
        {
            var OfficialLevelOfCareId = dictionary["KODEID"];
            var OfficialLevelOfCareName = dictionary["LANGTNAVN"];

            OfficialLevelOfCareDTO dto = new OfficialLevelOfCareDTO();

            if (OfficialLevelOfCareId != null) dto.Id = (long)OfficialLevelOfCareId;
            if (OfficialLevelOfCareName != null) dto.Name = OfficialLevelOfCareName.ToString();
            return dto;
        }

        private ContactTypeDTO MapToContactTypeDTO(IDictionary<string, Object> dictionary)
        {
            var ContactTypeId = dictionary["KODEID"];
            var ContactTypeName = dictionary["LANGTNAVN"];

            ContactTypeDTO dto = new ContactTypeDTO();

            if (ContactTypeId != null) dto.Id = (long)ContactTypeId;
            if (ContactTypeName != null) dto.Name = ContactTypeName.ToString();
            return dto;
        }
    }
}
