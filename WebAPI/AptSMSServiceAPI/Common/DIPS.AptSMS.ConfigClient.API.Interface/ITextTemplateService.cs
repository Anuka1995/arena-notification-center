using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{

    public interface ITextTemplateService
    {
        ///<summary>
        ///Get preview string
        /// </summary>
        /// 
        string GetPreview(string templateTextStr, bool isPathPSSRequred,long hospitalId);

        /// <summary>
        /// Create or update SMS Text Template
        /// </summary>
        /// <param name="textTemplate"></param>
        /// <returns></returns>
        Guid SaveSMSTextTemplate(SMSText textTemplate);

        /// <summary>
        /// Search SMS tex template by params
        /// </summary>
        /// <param name="departmetnID"></param>
        /// <param name="opdID"></param>
        /// <param name="sectionID"></param>
        /// <param name="wardID"></param>
        /// <param name="searchTerm"></param>
        /// <param name="isActive"></param>
        /// <param name="isHospitalOnly"></param>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        List<SMSText> SearchSMSTextTemplate(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly,long hospitalId);

        /// <summary>
        /// Get SMStext template by sms text id
        /// </summary>
        /// <param name="SMSTextId"></param>
        /// <returns></returns>
        SMSText GetSMSTextTemplateById(Guid SMSTextId);

        /// <summary>
        /// Search SMS tex template by params and return TreeNode List
        /// </summary>
        /// <param name="departmetnID"></param>
        /// <param name="opdID"></param>
        /// <param name="sectionID"></param>
        /// <param name="wardID"></param>
        /// <param name="searchTerm"></param>
        /// <returns>List<TreeNode></returns>
        List<TreeNode> GetSMSTextTemplateTreeNodes(long? departmetnID, long? opdID, long? sectionID, long? wardID, string searchTerm, bool isActive, bool isHospitalOnly,long hospitalId);

        /// <summary>
        /// Get all active/inactive SMS text templates
        /// </summary>
        /// <param name="departmetnID"></param>
        /// <returns>List<SMSText></returns>
        List<SMSText> GetAllSMSTextTemplates(bool isActive, long hospitalId);

        List<SMSText> GetEnhancedSearchByWard(Guid scheduleId, long depId, long wardId,long hospitalId, long? locationId, List<int> contactTypes,List<int> officialLevelofcare);
        List<SMSText> GetEnhancedSearchByOPDId(Guid scheduleId, long depId, long opdId,long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<SMSText> GetEnhancedSearchByWard_BySection(Guid scheduleId, long depId, long wardId, long sectionId,long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<SMSText> GetEnhancedSearchBySection(Guid scheduleId, long depId, long sectionId,long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<SMSText> GetEnhancedSearchByDep(Guid scheduleId, long depId,long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);
        List<SMSText> GetEnhancedSearchOnHospitalLevel(Guid scheduleId, long hospitalId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);

        List<SMSText> GetEnhancedSearchOnHospitalLevel_OPD(Guid scheduleId, long hospitalIdlong ,long opdId, long? locationId, List<int> contactTypes, List<int> officialLevelofcare);

        /// <summary>
        /// Get all the text templates with ruleset data
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        List<SMSText> GetFullSMSTextTemplatesFor(bool isActive, long hospitalId);

    }
}
