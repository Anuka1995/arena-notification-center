using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("tag/")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(TagController));

        private readonly ITagService m_tagService;
        private readonly IUser m_user;

        public TagController(ITagService tagService, IUser user)
        {
            m_tagService = tagService;
            m_user = user;
        }

        [HttpPost, Route("save")]
        public ActionResult SaveTag([FromBody]TagItem tag)
        {
            var message = "saving Tag, Tag name -  " + tag.TagName + " and tag Description -" + tag.Description;
            m_log.Info(message);
            if (m_user.HospitalId == 0)
            {
                var errormessage = "Failed to load hospital ID from the User" + m_user.UserName;
                m_log.Error(errormessage);
                throw new NullOrEmptyValueException(errormessage);
            }
            if (string.IsNullOrEmpty(tag.TagName))
            {
                var errormessage = "Phrase name cannot be empty";
                m_log.Error(errormessage);
                throw new UserInputValidationException(errormessage);
            }
            if (string.IsNullOrEmpty(tag.TagValue))
            {
                var errormessage = "Phrase value cannot be empty";
                m_log.Error(errormessage);
                throw new UserInputValidationException(errormessage);
            }

            try
            {
                tag.HospitalId = m_user.HospitalId;
                var guid = m_tagService.SaveTag(tag);
                m_log.Trace($"Save tag Tag id :{guid}");
                return Ok(guid);
            }
            catch (DBOperationException dbError)
            {
                var error = $"Tag save failed from data layer.";
                m_log.ErrorException(error, dbError);

                if (dbError.OracleErrorCode == GlobalOptions.DBErrorCodes.Such_a_phrase_already_exists)
                    throw new UserInputValidationException("Phrase already exists!");

                throw;
            }
            catch (Exception ex)
            {
                var error = "Failed Save Tag name - " + tag.TagName + " and tag Description -" + tag.Description;
                m_log.ErrorException(error, ex);
                throw;
            }
        }

        [HttpGet, Route("get/{tagGUID}")]
        public ActionResult GetTagBy([FromRoute]Guid? tagGUID)
        {
            var message = "executing GetTagBy fro Tag GUID-" + tagGUID;
            m_log.Info(message);
            try
            {
                m_log.Info("Get a Tag by GUID");
                var tag = m_tagService.GetTagBy(tagGUID.Value);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                var errormessage = $"fail load GetTagBy Tag GUID-{tagGUID}";
                m_log.ErrorException(errormessage, ex);
                throw;
            }
        }

        [HttpGet, Route("search")]
        public ActionResult SearchTags([FromQuery]long? departmentId, [FromQuery]string term, [FromQuery]bool getInactive, [FromQuery]bool isHospitalLevel)
        {
            if (departmentId == 0) departmentId = null;

            if (string.IsNullOrEmpty(term)) term = null;

            var message = "executing SearchTags for departmentId-" + departmentId + " and term-" + term;
            m_log.Info(message);
            try
            {
                var filteredTags = m_tagService.GetTagsBy(departmentId, term, getInactive, isHospitalLevel,m_user.HospitalId);

                var xPathNodes = m_tagService.GetXPathTreeNodes((filteredTags.Where(t => t.TagType == TagType.XPATH).ToList()));

                var staticNodes = m_tagService.GetStaticTreeNodes((filteredTags.Where(t => t.TagType == TagType.STATIC_VALUE).ToList()));

                List<TreeNode> fullTree = new List<TreeNode>();
                if (xPathNodes.Count() > 0)
                    fullTree.Add(new TreeNode()
                    {
                        Title = "X-Paths",
                        ChildNodes = xPathNodes
                    });

                if (staticNodes.Count() > 0)
                    fullTree.Add(new TreeNode()
                    {
                        Title = "Text Phrases",
                        ChildNodes = staticNodes
                    });

                return Ok(fullTree);
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get results for get All tags by DepartmentId and Search text departmentId-" + departmentId + " and term-" + term;
                m_log.ErrorException(errorMessage, ex);
                throw;
            }
        }

        [HttpGet, Route("commonFormatDateTime")]
        public ActionResult GetCommonTimeFormat()
        {          
                m_log.Info("Calling CommonFormatDateTime");
                var commonFormats = m_tagService.GetCommonDataTimeFormats();
                return Ok(commonFormats);
           
        }

        [HttpGet, Route("searchTags")]
        public ActionResult SearchTags([FromQuery]long? departmentId, [FromQuery]string term)
        {
            if (departmentId == 0) departmentId = null;

            if (string.IsNullOrEmpty(term)) term = null;

            var message = "executing SearchTags for departmentId-" + departmentId + " and term-" + term;
            m_log.Info(message);
            try
            {
                var filteredTags = m_tagService.GetTagsBy(departmentId, term, m_user.HospitalId);

                var xPathNodes = m_tagService.GetXPathTreeNodes((filteredTags.Where(t => t.TagType == TagType.XPATH).ToList()));

                var staticNodes = m_tagService.GetStaticTreeNodes((filteredTags.Where(t => t.TagType == TagType.STATIC_VALUE).ToList()));

                List<TreeNode> fullTree = new List<TreeNode>();
                if (xPathNodes.Count() > 0)
                    fullTree.Add(new TreeNode()
                    {
                        Title = "X-Paths",
                        ChildNodes = xPathNodes
                    });

                if (staticNodes.Count() > 0)
                    fullTree.Add(new TreeNode()
                    {
                        Title = "Text Phrases",
                        ChildNodes = staticNodes
                    });

                return Ok(fullTree);
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to get results for get All tags by DepartmentId and Search text departmentId-" + departmentId + " and term-" + term;
                m_log.ErrorException(errorMessage, ex);
                throw;
            }
        }

    }
}