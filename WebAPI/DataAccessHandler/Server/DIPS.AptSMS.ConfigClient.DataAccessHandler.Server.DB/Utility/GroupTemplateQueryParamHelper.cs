using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Data;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    class GroupTemplateQueryParamHelper
    {

        internal static OracleDynamicParameters GetGroupTemplateSaveUpdateQueryParams(GroupedTextDTO groupedTextDTO)
        {
            var isactive = groupedTextDTO.IsActive ? 1 : 0;
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add("p_oldTemplateGUID", groupedTextDTO.GroupedTempateGUID, OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_organizationID", (groupedTextDTO.OrganizationID == null ? null : groupedTextDTO.OrganizationID), OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", groupedTextDTO.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", (groupedTextDTO.DepartmentID == null ? null : groupedTextDTO.DepartmentID), OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSTextTempName", groupedTextDTO.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSText", groupedTextDTO.Text, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_validFrom", groupedTextDTO.ValidFrom, OracleMappingType.Date, ParameterDirection.Input);
            dynamicParameters.Add("p_validTo", groupedTextDTO.ValidTo, OracleMappingType.Date, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTextTempID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }

        /// <summary>
        /// Query params for saving grouped Templates
        /// </summary>
        /// <param name="groupedTextDTO"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetGroupTemplateSaveQueryParams(GroupedTextDTO groupedTextDTO)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_organizationID", (groupedTextDTO.OrganizationID == null ? null : groupedTextDTO.OrganizationID), OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", groupedTextDTO.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", (groupedTextDTO.DepartmentID == null ? null : groupedTextDTO.DepartmentID), OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSTextTempName", groupedTextDTO.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSText", groupedTextDTO.Text, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTextTempID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }
        /// <summary>
        /// update text Template Query params
        /// </summary>
        /// <param name="groupedTextDTO"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetGroupTemplateUpdateQueryParams(GroupedTextDTO groupedTextDTO)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            if (groupedTextDTO.GroupedTempateGUID != null)
                dynamicParameters.Add("p_textTemplateGUID", GuidConvert.ToRaw(groupedTextDTO.GroupedTempateGUID), OracleMappingType.Raw, ParameterDirection.Input);

            dynamicParameters.Add("p_hospitalID", groupedTextDTO.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", groupedTextDTO.DepartmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSTextTempName", groupedTextDTO.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSText", groupedTextDTO.Text, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_newtextTemplateGUID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }

        /// <summary>
        /// Get GroupTemplate By Id 
        /// </summary>
        /// <param name="groupedTextDTO"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetGroupTemplateByIdQueryParams(Guid groupedTextGuid)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_textTemplateGUID", groupedTextGuid, OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("l_textTemplateDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }
        /// <summary>
        /// Search Group Template by Department and Search Text.
        /// </summary>
        /// <param name="groupedText"></param>
        /// <returns></returns>
        internal static OracleDynamicParameters GetGroupTemplateBySearchParams(long? departmentID, string searchTerm, bool isActive, bool isHospitalOnly,long hospitalId)
        {
            var isActiveIntVal = isActive ? 1 : 0;
            var isHospitalOnlyIntVal = isHospitalOnly ? 1 : 0;
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_departmentID", departmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSText", searchTerm, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isActiveIntVal, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalOnly", isHospitalOnlyIntVal, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("l_textTemplateDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }
    }
}
