using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.API.Common.ViewModels;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class TagService : ITagService
    {
        private readonly ITagDataService m_tagDataService;

        private static readonly ILog s_log = LogProvider.GetLogger(typeof(TagService));

        private IConfiguration m_config;
        public TagService(ITagDataService tagSetDataService, IConfiguration config)
        {
            m_tagDataService = tagSetDataService;
            m_config = config;
        }

        public Guid SaveTag(TagItem tagmodel)
        {
            var tagDTO = ModelToDataDTO(tagmodel);
            var guid = m_tagDataService.SaveTag(tagDTO);
            return guid;
        }

        public List<DateTimeFormat> GetCommonDataTimeFormats()
        {
            List<DateTimeFormat> tagItemList = new List<DateTimeFormat>();
            int i = 1;
            while (i < 5)
            {
                var timeFormat = GetConfigValue("timeFormat" + i);
                if (timeFormat!=null && timeFormat.Length > 2)
                {
                    var dateString = GetDateTimeString(timeFormat);
                    if (dateString.Length > 2)
                    {
                        tagItemList.Add( new DateTimeFormat { displaySample = dateString, format = timeFormat });
                    }
                }
                i++;
            }
            return tagItemList;
        }

        public TagItem GetTagBy(Guid guid)
        {
            s_log.Trace("Get Tag by guid: ", guid);

            var tagDto = m_tagDataService.GetTagBy(guid);
            TagItem tagItem = DataDTOToModel(tagDto);

            if (tagItem.TagType == TagType.STATIC_VALUE && 
                (tagItem.HospitalId == null || tagItem.HospitalId == 0)) 
                throw new NullOrEmptyValueException("Phrase cannot have empty hospital");
            
            return tagItem;
        }

        public List<TagItem> GetTagsBy(long? departmentID, string searchTerm, bool getInactive, bool isHospitalLevel, long hospitalId)
        {
            bool isActive = !getInactive;
            var tagDtoList = m_tagDataService.SearchTags(departmentID, searchTerm, isActive, isHospitalLevel, hospitalId);

            if (tagDtoList == null) throw new Exception("Null result returned from the data layer.");

            s_log.Trace("Get All Tags  by Department Id and tagName. TagDto Count: ", tagDtoList.Count);

            List<TagItem> tagItemList = tagDtoList.Select(t => DataDTOToModel(t)).ToList();

            s_log.Trace("Get All Tags  by DepartmentId and tagName called. Item Count: ", tagItemList.Count);

            var hospitalEmpty = tagItemList.Any(tag => ((tag.HospitalId == null || tag.HospitalId == 0) && tag.TagType == TagType.STATIC_VALUE));
            if (hospitalEmpty) throw new NullOrEmptyValueException("Phrase cannot have empty hospital");

            return tagItemList;
        }

        public List<TagItem> GetAllTags(long hospitalid)
        {
            var tagDtoList = m_tagDataService.SearchTags(null,null, true, false, hospitalid);
            if (tagDtoList == null) throw new Exception("Null retuened from data layer.");

            s_log.Trace("Get All Tags from DB TagDto Count :", tagDtoList.Count);

            List<TagItem> tagItemList = tagDtoList.Select(t => DataDTOToModel(t)).ToList();

            s_log.Trace("Get All Tags called Item Count :", tagItemList.Count);

            return tagItemList;
        }

        public TagItem GetTagBy(string tagName,long hospitalid)
        {
            var tagDtoList = m_tagDataService.SearchTags(null, tagName, true, false, hospitalid);
            if (tagDtoList == null) throw new Exception("Null returned from data layer.");

            s_log.Trace("Get All Tags  by tagName. TagDto Count :", tagDtoList.Count);

            List<TagItem> tagItemList = tagDtoList.Select(t => DataDTOToModel(t)).ToList();

            s_log.Trace("Get All Tags  by tagname called. Item Count :", tagItemList.Count);
            return tagItemList.FirstOrDefault();
        }

        public List<TagItem> GetAllTagsBy(long departmetnID, long hospitalid)
        {
            var tagDtoList = m_tagDataService.SearchTags(departmetnID, null, true, false, hospitalid);

            if (tagDtoList == null) throw new Exception("Null returned from data layer.");

            s_log.Trace("Get All Tags  by Department Id. TagDto Count :", tagDtoList.Count);

            List<TagItem> tagItemList = tagDtoList.Select(t => DataDTOToModel(t)).ToList();

            s_log.Trace("Get All Tags  by DepartmentId called. Item Count :", tagItemList.Count);

            return tagItemList;
        }

        public List<TreeNode> GetXPathTreeNodes(List<TagItem> xpathTags)
        {
            return xpathTags.Select(st =>
                new TreeNode()
                {
                    Title = st.TagName,
                    Id = st.TagId.ToString(),
                    Tag = st,
                    ChildNodes = new List<TreeNode>()
                })
                .ToList();
        }

        public List<TreeNode> GetStaticTreeNodes(List<TagItem> staticTags)
        {
            return staticTags.Select(st => 
                new TreeNode() { 
                    Title = st.TagName, 
                    Id = st.TagId.ToString(), 
                    Tag = st,
                    ChildNodes = new List<TreeNode>()})
                .ToList();
        }

        #region Private
        private TagDTO ModelToDataDTO(TagItem tagmodel)
        {
            long? departmentID  = ((tagmodel.DepartmentId == null || tagmodel.DepartmentId == 0) ? null : tagmodel.DepartmentId);

            return new TagDTO()
            {
                TagGUID = (tagmodel.TagId == Guid.Empty || tagmodel.TagId == null ? null : tagmodel.TagId),
                HospitalID = tagmodel.HospitalId,
                DepartmentID = departmentID,
                Name = tagmodel.TagName,
                Value = tagmodel.TagValue,
                TagType = (int)tagmodel.TagType,
                DataType = (int)tagmodel.DataType,
                Description = tagmodel.Description,
                IsActive = tagmodel.IsActive
            };
        }

        private TagItem DataDTOToModel(TagDTO tagDto)
        {
            return new TagItem()
            {
                TagId = tagDto.TagGUID.Value,
                HospitalId = tagDto.HospitalID,
                DepartmentId = tagDto.DepartmentID,
                TagName = tagDto.Name,
                Description = tagDto.Description,
                TagValue = tagDto.Value,
                TagType = (TagType)tagDto.TagType,
                DataType = (TagDataType)tagDto.DataType,
                IsActive = tagDto.IsActive
            };
        }

        private string GetConfigValue(string keyValue)
        {
            try
            {
                var configValue = m_config.GetValue<string>(keyValue.Trim());
                return configValue;
            }
            catch (Exception ex)
            {
                var error = "cannot find/Read the config value" + keyValue;
                s_log.Error(error, ex);
                return "";
            }
        }

        private string GetDateTimeString(string dateTimeFormat)
        {
            try
            {
              return  DateTime.Now.ToString(dateTimeFormat);               
            }
            catch (Exception ex)
            {
                var error = "cannot convert date time from given format in the config value" + dateTimeFormat;
                s_log.Error(error, ex);
                return "";
            }
        }

        public List<TagItem> GetTagsBy(long? departmentID, string searchTerm, long hospitalId)
        {
           
            var tagDtoList = m_tagDataService.SearchTags(departmentID, searchTerm, hospitalId);

            if (tagDtoList == null) throw new Exception("Null result returned from the data layer.");

            s_log.Trace("Get All Tags  by Department Id and tagName. TagDto Count: ", tagDtoList.Count);

            List<TagItem> tagItemList = tagDtoList.Select(t => DataDTOToModel(t)).ToList();

            s_log.Trace("Get All Tags  by DepartmentId and tagName called. Item Count: ", tagItemList.Count);

            var hospitalEmpty = tagItemList.Any(tag => ((tag.HospitalId == null || tag.HospitalId == 0) && tag.TagType == TagType.STATIC_VALUE));
            if (hospitalEmpty) throw new NullOrEmptyValueException("Phrase cannot have empty hospital");

            return tagItemList;
        }
        #endregion
    }
}
