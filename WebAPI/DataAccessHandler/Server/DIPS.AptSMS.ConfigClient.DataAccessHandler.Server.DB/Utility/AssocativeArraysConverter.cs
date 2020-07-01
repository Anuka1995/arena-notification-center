using Dapper.Oracle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    public static class AssocativeArraysConverter
    {
        public static OracleDynamicParameters ConvertCollectionToPLSQLAssociativeArray<T>(IEnumerable<T> collection, string parameterName)
        {
            Type type = typeof(T);
            var oracleDbType = GetOracleDbType(type);
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            if (collection != null && collection.Any())
            {
                if (type.IsEnum)
                {
                    dynamicParameters.Add(parameterName,
                        ConvertCollectionOfEnumToArrayOfStrings(collection),
                        direction: ParameterDirection.Input,
                        collectionType: OracleMappingCollectionType.PLSQLAssociativeArray);
                }
                else if (type == typeof(Guid))
                {
                    dynamicParameters.Add(parameterName,
                        ConvertCollectionOfGuidToArrayOfByte((IEnumerable<Guid>)collection),
                        direction: ParameterDirection.Input,
                        collectionType: OracleMappingCollectionType.PLSQLAssociativeArray);
                }
                else if (type == typeof(Guid?))
                {
                    dynamicParameters.Add(parameterName,
                        ConvertCollectionOfGuidToArrayOfByte((IEnumerable<Guid?>)collection),
                        direction: ParameterDirection.Input,
                        collectionType: OracleMappingCollectionType.PLSQLAssociativeArray);
                }
                else
                {
                    dynamicParameters.Add(parameterName,
                        collection.ToArray(),
                        direction: ParameterDirection.Input,
                        collectionType: OracleMappingCollectionType.PLSQLAssociativeArray);
                }
            }
            else
            {
                dynamicParameters.Add(parameterName,
                       value: new object[] { null },
                       direction: ParameterDirection.Input,
                       collectionType: OracleMappingCollectionType.PLSQLAssociativeArray,
                       size: 0,
                       dbType: oracleDbType);
            }

            return dynamicParameters;
        }

        public static byte[] ToRaw(Guid? value)
        {
            if (value == null)
            {
                return new byte[0];
            }

            return ToRaw(value.Value);
        }

        public static byte[] ToRaw(Guid value)
        {
            return value.ToByteArray();
        }

        #region PrivteMethods
        private static IEnumerable<byte[]> ConvertCollectionOfGuidToArrayOfByte(IEnumerable<Guid?> guidCollection)
        {
            var collectionOfByteArray = new Collection<byte[]>();
            foreach (Guid? guid in guidCollection)
            {
                collectionOfByteArray.Add(ToRaw(guid));
            }

            return collectionOfByteArray.ToArray();
        }

        private static IEnumerable<string> ConvertCollectionOfEnumToArrayOfStrings<T>(IEnumerable<T> enumCollection)
        {
            var collectionOfStrings = new Collection<string>();
            foreach (T status in enumCollection)
            {
                collectionOfStrings.Add(status.ToString());
            }

            return collectionOfStrings.ToArray();
        }

        private static IEnumerable<byte[]> ConvertCollectionOfGuidToArrayOfByte(IEnumerable<Guid> guidCollection)
        {
            var collectionOfByteArray = new Collection<byte[]>();
            foreach (Guid guid in guidCollection)
            {
                collectionOfByteArray.Add(ToRaw(guid));
            }

            return collectionOfByteArray.ToArray();
        }

        private static OracleMappingType GetOracleDbType(Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsEnum)
            {
                return OracleMappingType.Varchar2;
            }

            if (type == typeof(string))
            {
                return OracleMappingType.Varchar2;
            }

            if (type == typeof(Guid))
            {
                return OracleMappingType.Raw;
            }

            if (type == typeof(Byte[]))
            {
                return OracleMappingType.Blob;
            }

            if (type == typeof(int))
            {
                return OracleMappingType.Int32;
            }

            if (type == typeof(long))
            {
                return OracleMappingType.Int64;
            }

            var missingMappingException = new ArgumentException(String.Format("Type : {0} is missing mapping to OracleDbType", type.Name));
            throw missingMappingException;
        }
        #endregion
    }
}
