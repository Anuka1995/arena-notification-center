using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface ITagService
    {
        ///<summary>
        ///Save or Update a Tag
        /// </summary>
        /// 
        Guid SaveTag(TagItem tag);

        ///<summary>
        ///Get all Tags
        /// </summary>
        /// 
        List<TagItem> GetAllTags(long hospitalid);

        ///<summary>
        ///Get a Tag by guid
        /// </summary>
        /// 
        TagItem GetTagBy(Guid guid);

        ///<summary>
        ///Get a Tag by Name
        /// </summary>
        /// 
        TagItem GetTagBy(string tagName, long hospitalid);

        ///<summary>
        ///Get All Tags for a Department
        /// </summary>
        /// 
        List<TagItem> GetAllTagsBy(long departmetnID, long hospitalid);

        ///<summary>
        ///Search and get tags for ther filtering
        /// </summary>
        /// 
        List<TagItem> GetTagsBy(long? departmentID, string searchTerm, bool getInactive, bool isHospitalLevel, long hospitalId);

        ///<summary>
        ///get Tree nodes from xpath Tags
        /// </summary>
        /// 
        List<TreeNode> GetXPathTreeNodes(List<TagItem> xpathTags);

        ///<summary>
        ///get Tree nodes from static Tags
        /// </summary>
        /// 
        List<TreeNode> GetStaticTreeNodes(List<TagItem> staticTags);

        /// <summary>
        /// Get common formats for date time 
        /// </summary>
        /// <returns></returns>
        List<DateTimeFormat> GetCommonDataTimeFormats();

        ///<summary>
        ///Search and get tags for the filtering with hospital level
        /// </summary>
        /// 
        List<TagItem> GetTagsBy(long? departmentID, string searchTerm, long hospitalId);
    }
}
