using Dapper.Oracle;
using System;
using System.Data;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    class TextTemplateQueryParamsHelper
    {
        internal static OracleDynamicParameters GetTextTemplateSaveUpdateQueryParams(TextTemplateDTO textTemplate)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var isactive = textTemplate.IsActive ? 1 : 0;
            var isAttachPSSLink = textTemplate.AttachPSSLink ? 1 : 0;
            var isVideoAppoinment = textTemplate.IsVideoAppoinment ? 1 : 0;

            var levelOfCareParam = AssocativeArraysConverter
                .ConvertCollectionToPLSQLAssociativeArray(textTemplate.OfficialLevelOfCare, "p_offLevelOfCare");

            var contactTypeParam = AssocativeArraysConverter
                .ConvertCollectionToPLSQLAssociativeArray(textTemplate.ContactType, "p_contactType");

            dynamicParameters.AddDynamicParams(levelOfCareParam);
            dynamicParameters.AddDynamicParams(contactTypeParam);
            dynamicParameters.Add("p_oldSMSTextID", GuidConvert.ToRaw(textTemplate.TemplateGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_ruleSetGuid", GuidConvert.ToRaw(textTemplate.RuleSetGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", textTemplate.HospitalID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentID", textTemplate.DepartmentID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_OPDID", textTemplate.OPDID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", textTemplate.LocationID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_sectionID", textTemplate.SectionID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_wardID", textTemplate.WardID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSTextName", textTemplate.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSText", textTemplate.SMSText, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_SMSTextTempID", GuidConvert.ToRaw(textTemplate.GroupedTextGUID), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_validFrom", textTemplate.ValidFrom, OracleMappingType.Date, ParameterDirection.Input);
            dynamicParameters.Add("p_validTo", textTemplate.ValidTo, OracleMappingType.Date, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_sendPSSLink", isAttachPSSLink, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_isVideoCall", isVideoAppoinment, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTEXTID", null, OracleMappingType.Raw, ParameterDirection.ReturnValue, 16);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetTextTemplateSearchQueryParams(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly,long hospitalId)
        {
            var isActiveIntVal = isActive ? 1 : 0;
            var isHospitalOnlyIntVal = isHospitalOnly ? 1 : 0;
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            dynamicParameters.Add("p_departmentId", departmetnID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_OPDId", opdID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_sectionId", sectionID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_wardId", wardID, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_text", searchTerm, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_isActive", isActiveIntVal, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalOnly", isHospitalOnlyIntVal, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTextTemplates", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetTextTemplateBYIdQueryParams(Guid textTemplateId)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add("p_SMSTextGUID", GuidConvert.ToRaw(textTemplateId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTextDetails", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetFiletrByWardQueryParams(Guid scheduleId, long? depId, long? wardId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);
        
            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentId", depId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_wardId", wardId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }
        internal static OracleDynamicParameters GetFiletrByOPDQueryParams(Guid scheduleId, long? depId, long? opdId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare) {

            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentId", depId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_OPDId", opdId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetFiletrByWardBySectionQueryParams(Guid scheduleId, long depId, long wardId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentId", depId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_wardId", wardId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_sectionID", sectionId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }
        internal static OracleDynamicParameters GetFiletrBySectionQueryParams(Guid scheduleId, long depId, long sectionId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentId", depId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_sectionID", sectionId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }
        internal static OracleDynamicParameters GetFiletrByDepartmentQueryParams(Guid scheduleId, long depId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_departmentId", depId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }

        
          internal static OracleDynamicParameters GetFiletrByHospitalLevelQueryParams(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
           {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }
        internal static OracleDynamicParameters GetFiletrByHospitalOPDLevelQueryParams(Guid scheduleId, long hospitalId,long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();

            var careLevelString = (officialLevelofcare == null) ? null : string.Join(",", officialLevelofcare);
            var contactTypeString = (contactTypes == null) ? null : string.Join(",", contactTypes);

            dynamicParameters.Add("p_rulesetguid", GuidConvert.ToRaw(scheduleId), OracleMappingType.Raw, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_OPDId", opdId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_locationID", locationId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("p_levelOfCare", careLevelString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("p_contactType", contactTypeString, OracleMappingType.Varchar2, ParameterDirection.Input);
            dynamicParameters.Add("l_smsTemplateList", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            return dynamicParameters;
        }

        internal static OracleDynamicParameters GetTextTemplateOverviewQueryParams(bool isActive,long hospitalId)
        {
            OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
            var isactive = (isActive ? 1 : 0);
            dynamicParameters.Add("p_isActive", isactive, OracleMappingType.Int16, ParameterDirection.Input);
            dynamicParameters.Add("p_hospitalID", hospitalId, OracleMappingType.Long, ParameterDirection.Input);
            dynamicParameters.Add("l_SMSTextTemplates", null, OracleMappingType.RefCursor, ParameterDirection.ReturnValue);
            
            return dynamicParameters;
        }
    }
}
