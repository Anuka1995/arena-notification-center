using System;
using static DIPS.AptSMS.ConfigClient.Common.Models.GlobalOptions;

namespace DIPS.AptSMS.ConfigClient.Common.Exceptions
{
    public class DBOperationException : Exception
    {
        public DBErrorCodes OracleErrorCode { get; private set; }
        public DBExceptionScenarios ErrorReason { get; private set; }

        public DBOperationException()
        {
        }

        public DBOperationException(string message)
            : base(message)
        {
        }

        public DBOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public DBOperationException(string defaultMessage, long errorCode, DBExceptionScenarios errorReason, Exception inner)
            : base(GetMessageStringFromErrorCode(defaultMessage, errorCode), inner)
        {
            OracleErrorCode = GetErrorCodeFromErrorNumber(errorCode);
            ErrorReason = errorReason;
        }

        public DBOperationException(string errorMessage, DBExceptionScenarios errorReason, Exception inner)
            : base(errorMessage, inner)
        {
            ErrorReason = errorReason;
        }

        public DBOperationException(string errorMessage, DBExceptionScenarios errorReason)
            : base(errorMessage)
        {
            ErrorReason = errorReason;
        }

        #region Private
        private DBErrorCodes GetErrorCodeFromErrorNumber(long oracleErrorCode)
        {
            foreach (var value in Enum.GetValues(typeof(DBErrorCodes)))
            {
                var intval = (int)value;
                if (intval == oracleErrorCode)
                {
                    return (DBErrorCodes)value;
                }
            }
            return DBErrorCodes.ErrorCodeNotDefined;
        }

        private static string GetMessageStringFromErrorCode(string defaultMessage, long errorCode)
        {
            foreach (var value in Enum.GetValues(typeof(DBErrorCodes)))
            {
                var intval = (int)value;
                var stringVal = (DBErrorCodes)value;
                if (intval == errorCode)
                {
                    return stringVal.ToString();
                }
            }
            return defaultMessage;
        }
        #endregion
    }
}