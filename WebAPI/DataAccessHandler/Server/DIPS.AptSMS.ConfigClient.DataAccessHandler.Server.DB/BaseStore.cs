using System;
using System.Data;
using Dapper;
using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility;
using DIPS.Infrastructure.Logging;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB
{
    public class BaseStore
    {
        private static readonly ILog s_log = LogProvider.GetLogger(typeof(BaseStore));

        protected Guid SaveAndReturnPK(string sql, string pkName, OracleDynamicParameters oracleParameters, IDbConnection connection)
        {
            try
            {
                connection.Execute(sql, param: oracleParameters, commandType: CommandType.StoredProcedure);
                var returnValueByteArray = oracleParameters.Get<byte[]>(pkName);
                var primaryKeyguidId = GuidConvert.FromRaw(returnValueByteArray);
                return primaryKeyguidId;
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("SaveAndReturnPK Fails (Oracle Exception)", ex, "BaseStore");
                throw new DBOperationException(ex.Message, ex.Number, GlobalOptions.DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (Exception ex)
            {
                s_log.ErrorException("SaveAndReturnPK Fails", ex, "BaseStore");
                throw new DBOperationException(ex.Message, GlobalOptions.DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        protected int Update(string sql, OracleDynamicParameters oracleParameters, IDbConnection connection)
        {
            try
            {
                var affectedRows = connection.Execute(sql, param: oracleParameters, commandType: CommandType.StoredProcedure);
                return affectedRows;
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Update Fails (Oracle Exception)", ex, "BaseStore");
                throw new DBOperationException(ex.Message, ex.Number, GlobalOptions.DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Update Fails", ex, "BaseStore");
                throw new DBOperationException(ex.Message, GlobalOptions.DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        protected Guid UpdateAndReturnKey(string sql, string pkName, OracleDynamicParameters oracleParameters, IDbConnection connection)
        {
            try
            {
                var affectedRows = connection.Execute(sql, param: oracleParameters, commandType: CommandType.StoredProcedure);
                if (affectedRows == -1)
                {
                    s_log.Trace($"Update success [UpdateTag]");
                    var returnValueByteArray = oracleParameters.Get<byte[]>(pkName);
                    var primaryKeyguidId = GuidConvert.FromRaw(returnValueByteArray);
                    return primaryKeyguidId;
                }

                throw new DBOperationException("Update Tag was not successful!");
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Update Fails (Oracle Exception)", ex, "BaseStore");
                throw new DBOperationException(ex.Message, ex.Number, GlobalOptions.DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Update Fails", ex, "BaseStore");
                throw new DBOperationException(ex.Message, GlobalOptions.DBExceptionScenarios.ExceptionOccured, ex);
            }
        }

        protected int Delete(string sql, OracleDynamicParameters oracleParameters, IDbConnection connection)
        {
            try
            {
                var affectedRows = connection.Execute(sql, param: oracleParameters, commandType: CommandType.StoredProcedure);
                return affectedRows;
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex)
            {
                s_log.ErrorException("Delete Fails (Oracle Exception)", ex, "BaseStore");
                throw new DBOperationException(ex.Message, ex.Number, GlobalOptions.DBExceptionScenarios.OracleExceptionOccured, ex);
            }
            catch (Exception ex)
            {
                s_log.ErrorException("Delete Fails", ex, "BaseStore");
                throw new DBOperationException(ex.Message, GlobalOptions.DBExceptionScenarios.ExceptionOccured, ex);
            }
        }
    }
}
