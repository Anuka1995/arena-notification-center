using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Data;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    class DIPSContactTypeQueryParamHelper
    {

        /// <summary>
        /// Get Offical Level Of Care Details 
        /// </summary>
        /// <param name="OfficialLevelOfCareDTO"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetOfficialLevelOfCareQueryParams()
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("l_officialLevelOfCare", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }

        /// <summary>
        /// Get Contact Type Details 
        /// </summary>
        /// <param name="ContactTypeDTO"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetContactTypeQueryParams()
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("l_contactType", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }
    }
}
