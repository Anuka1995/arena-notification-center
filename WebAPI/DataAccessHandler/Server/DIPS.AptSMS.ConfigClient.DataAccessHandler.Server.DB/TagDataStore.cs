using DIPS.Infrastructure.Logging;
using System;
using System.Data.Common;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System.Collections.Generic;
using Dapper;
using System.Data;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System.Linq;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class TagDataStore : BaseStore, ITagDataStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(TagDataStore));

        private readonly DbProviderFactory m_providerFactory;

        public TagDataStore(DbProviderFactory providerFactory) 
        {
            m_providerFactory = providerFactory;
        }

        public Guid InsertATag(TagDTO tag)
        {
            string spName = "apt_code.SmsAppointmentService.createNewTag";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TemplateTagQueryParamsHelper.getQPForCreateUpdateTag(tag);
                    var newGuid = SaveAndReturnPK(spName, "l_tagID", oracleParameters, connection);

                    return newGuid;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("createNewTag Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in createNewTag", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("createNewTag Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public Guid UpdateATag(TagDTO tag)
        {
            string spName = "apt_code.SmsAppointmentService.updateTag";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TemplateTagQueryParamsHelper.getQPForCreateUpdateTag(tag);
                    var newGuid = UpdateAndReturnKey(spName, "l_newsmstemplatetagguid", oracleParameters, connection);

                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("InserOrUpdateATag Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in InserOrUpdateATag", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("InserOrUpdateATag Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public Guid InsertOrUpdateTag(TagDTO tag)
        {
            string spName = "apt_code.SmsAppointmentService.createUpdateTag";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleParameters = TemplateTagQueryParamsHelper.getQPForCreateORUpdateTag(tag);
                    var newGuid = UpdateAndReturnKey(spName, "l_tagID", oracleParameters, connection);

                    return newGuid;
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("InserOrUpdateATag Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in InserOrUpdateATag", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("InserOrUpdateATag Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public TagDTO SelectTagBy(Guid tagId)
        {
            const string spName = "apt_code.SmsAppointmentService.getTagDetails";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = TemplateTagQueryParamsHelper.GetQPForGetTagByID(tagId);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("getTagDetails returned empty or null result.");
                        throw new DBOperationException("getTagDetails returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var tags = new List<TagDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        tags.Add(MapToTagDTO(result));
                    }
                    return tags.FirstOrDefault();
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectTagBy(Guid tagId) Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectTagBy(Guid tagId)", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectTagBy(Guid tagId) Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TagDTO> SelectTagsOn(long? departmentId, string keyword, long hospitalId)
        {
            const string spName = "apt_code.SmsAppointmentService.getActiveTags";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = TemplateTagQueryParamsHelper.GetQPForActiveTags(departmentId, keyword, hospitalId);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("searchTag returned empty or null result.");
                        throw new DBOperationException("searchTag returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var tags = new List<TagDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        tags.Add(MapToTagDTO(result));
                    }
                    return tags;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectTagsOn Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectTagsOn", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectTagsOn Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        public List<TagDTO> SelectTagsOn(long? departmentId, string keyword, bool active, bool hospitalLevel, long hospitalId)
        {
            const string spName = "apt_code.SmsAppointmentService.searchTag";

            try
            {
                using (var connection = m_providerFactory.CreateConnection())
                {
                    var oracleparams = TemplateTagQueryParamsHelper.GetQPForSearchTagsMoreOptions(departmentId, keyword, active, hospitalLevel, hospitalId);

                    var resultSet = connection.Query(spName, oracleparams, commandType: CommandType.StoredProcedure);
                    if (resultSet == null)
                    {
                        s_log.Error("searchTag returned empty or null result.");
                        throw new DBOperationException("searchTag returned empty or null result.", GlobalOptions.DBExceptionScenarios.DBReturnedEmptyOrNullDataSet);
                    }

                    var tags = new List<TagDTO>();
                    foreach (var result in resultSet)
                    {
                        var resultDictionary = result as IDictionary<string, object>;
                        tags.Add(MapToTagDTO(resultDictionary));
                    }
                    return tags;
                }

            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SelectTagsOn Fails(Error from Oracle)", ex);
                throw new DBOperationException(ex.Message, ex.Number, DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (DBOperationException e)
            {
                s_log.WarnException("Error Occured in SelectTagsOn", e);
                throw;
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SelectTagsOn Fails", ex);
                throw new DBOperationException(ex.Message, DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        #region Private Functions
        private TagDTO MapToTagDTO(IDictionary<string, object> dictionary)
        {
            var tagDto = new TagDTO();

            var tagGuid = dictionary["SMSTEMPLATETAGGUID"];
            var hospitalID = dictionary["HOSPITALID"];
            var departmentID = dictionary["DEPARTMENTID"];
            var tagName = dictionary["TAG"];
            var description = dictionary["TAGDESCRIPTION"];
            var value = dictionary["TAGVALUE"];
            var isActive = dictionary["ISACTIVE"];
            var replacedBy = dictionary["REPLACEDBYGUID"];
            var type = dictionary["TAGTYPE"];
            var dataType = dictionary["DATATYPE"];

            if (tagGuid != null) tagDto.TagGUID = new Guid((byte[])tagGuid);
            if (replacedBy != null) tagDto.ReplacedByTagGUID = new Guid((byte[])replacedBy);
            if (departmentID != null)
                tagDto.DepartmentID = (long)departmentID;
            else
                tagDto.DepartmentID = null;
            tagDto.TagType =  Convert.ToInt16(type);
            tagDto.DataType = Convert.ToInt16(dataType);
            if(hospitalID != null)
                tagDto.HospitalID = (long)hospitalID;
            tagDto.Name = tagName as string;
            tagDto.Value = value as string;
            tagDto.Description = description as string;
            tagDto.IsActive = (Convert.ToInt16(isActive) == 0 ? false : true);

            return tagDto;
        }
        #endregion
    }
}
