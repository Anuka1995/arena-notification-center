using System;
using System.Data;
using Dapper.Oracle;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    public static class TemplateTagQueryParamsHelper
    {
        internal static OracleDynamicParameters getQPForCreateORUpdateTag(TagDTO tag)
        {
            var isactive = (tag.IsActive ? 1 : 0);
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_oldTagGUID", GuidConvert.ToRaw(tag.TagGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", tag.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", tag.DepartmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_tagName", tag.Name, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_tagDescription", tag.Description, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_tagType", tag.TagType, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_tagValue", tag.Value, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_dataType", tag.DataType, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);

            dynamicParameters.Add("l_tagID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);

            return dynamicParameters;
        }

        internal static OracleDynamicParameters getQPForCreateUpdateTag(TagDTO tag)
        {
            var isactive = (tag.IsActive ? 1 : 0);
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            if (tag.TagGUID != null)
                dynamicParameters.Add("p_smsTemplateTagGUID", GuidConvert.ToRaw(tag.TagGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", tag.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", tag.DepartmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_tagName", tag.Name, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_tagDescription", tag.Description, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_tagType", tag.TagType, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_tagValue", tag.Value, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            dynamicParameters.Add("p_dataType", tag.DataType, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);

            if (tag.TagGUID != null)
                dynamicParameters.Add("l_newsmstemplatetagguid", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            else
                dynamicParameters.Add("l_tagID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);

            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetQPForGetTagByID(Guid tagId)
        {
            var byteArray = GuidConvert.ToRaw(tagId);

            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add("p_SMSTemplateTagGUID", byteArray, OracleMappingType.Raw, ParameterDirection.Input, 16);
            dynamicParameters.Add("l_tagDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 300);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetQPForActiveTags(long? departmentId, string keyword, long hospitalId)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            if (departmentId != null)
                dynamicParameters.Add("p_departmentID", (long)departmentId, OracleMappingType.Long, ParameterDirection.Input);
            else
                dynamicParameters.Add("p_departmentID", null, OracleMappingType.Long, ParameterDirection.Input);

            if (!string.IsNullOrEmpty(keyword))
                dynamicParameters.Add("p_searchKey", keyword, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            else
                dynamicParameters.Add("p_searchKey", null, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);

            dynamicParameters.Add("l_tagDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 300);

            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetQPForSearchTagsMoreOptions(long? departmentId, string keyword, bool active, bool hospitalLevel,long hospitalId)
        {
            var isactive = (active ? 1 : 0);
            var hospitslSearch = (hospitalLevel ? 1 : 0);

            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            if (departmentId != null)
                dynamicParameters.Add("p_departmentID", (long)departmentId, OracleMappingType.Long, ParameterDirection.Input);
            else
                dynamicParameters.Add("p_departmentID", null, OracleMappingType.Long, ParameterDirection.Input);

            if (!string.IsNullOrEmpty(keyword))
                dynamicParameters.Add("p_searchKey", keyword, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);
            else
                dynamicParameters.Add("p_searchKey", null, OracleMappingType.Varchar2, ParameterDirection.Input, 0, true, 9, 0, String.Empty, DataRowVersion.Current);

            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalOnly", hospitslSearch, OracleMappingType.Int16, ParameterDirection.Input);

            dynamicParameters.Add("l_tagDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue, 300);

            return dynamicParameters;
        }
    }
}
