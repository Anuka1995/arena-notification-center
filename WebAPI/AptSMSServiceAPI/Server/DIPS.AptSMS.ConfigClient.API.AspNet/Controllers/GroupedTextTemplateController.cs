using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("templategroup/")]
    [ApiController]
    public class GroupedTextTemplateController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(GroupedTextTemplateController));

        private IGroupedTextService m_groupedTextService;
        private readonly IUser m_user;

        public GroupedTextTemplateController(IGroupedTextService textTemplateService, IUser user)
        {
            m_groupedTextService = textTemplateService;
            m_user = user;
        }

        [HttpPost, Route("save")]
        public ActionResult SaveTextTemplate([FromBody]SMSTextTemplate textTemplate)
        {
            m_log.Info("executing SaveTextTemplate");

            if (m_user.HospitalId == 0)
            {
                var ErrorMessage = "Failed to load hospital ID from the User";
                m_log.Error(ErrorMessage);
                throw new Exception(ErrorMessage);
            }
            if (string.IsNullOrEmpty(textTemplate.TextTemplateName))
            {
                var ErrorMessage = "Template name could not be empty";
                m_log.Error(ErrorMessage);
                throw new UserInputValidationException(ErrorMessage);
            }

            try
            {
                textTemplate.HospitalID = m_user.HospitalId;
                var newGuid = m_groupedTextService.SaveGroupedTemplate(textTemplate);

                return Ok(newGuid);
            }
            catch (DBOperationException e)
            {
                var ErrorMessage = "Error in db call for text template save.";
                m_log.ErrorException(ErrorMessage, e);

                if (e.OracleErrorCode == GlobalOptions.DBErrorCodes.Such_a_text_template_already_exists)
                    throw new UserInputValidationException("Text Template already exist!");

                throw;
            }
            catch (Exception e)
            {
                var ErrorMessage = "Exception thrown for add/edit group template";
                m_log.ErrorException(ErrorMessage, e);
                throw;
            }
        }

        [HttpGet, Route("get/{textGUID}")]
        public ActionResult GetTextTemplate([FromRoute]Guid textGUID)
        {
            m_log.Info("executing 'GetTextTemplate'");

            if (textGUID == Guid.Empty || textGUID == null)
            {
                var ErrorMessage = "Text template id cannot be empty.";
                m_log.Error(ErrorMessage);
                throw new UserInputValidationException(ErrorMessage);
            }

            try
            {
                var textTemplate = m_groupedTextService.GetTextTemplateBy(textGUID);
                return Ok(textTemplate);
            }
            catch (Exception e)
            {
                var ErrorMessage = "Exception thrown for get group template by ID";
                m_log.ErrorException(ErrorMessage, e);
                throw;
            }

        }

        [HttpGet, Route("filter")]
        public ActionResult SearchTextTemplate([FromQuery]long? departmentID, [FromQuery]string searchTerm, [FromQuery]bool getInactive, [FromQuery]bool ishospitalOnly)
        {
            var message = "executing SearchTextTemplate for department -" + departmentID + "  and term -" + searchTerm;
            m_log.Info("executing SearchTextTemplate");
            m_log.Info(message);

            try
            {
                var textTemplates = m_groupedTextService.FilterTextTemplatesBy(departmentID, searchTerm, m_user.SecurityToken, getInactive, ishospitalOnly, m_user.HospitalId);

                return Ok(textTemplates);
            }
            catch (Exception e)
            {
                var ErrorMessage = "Exception thrown for search group template";
                m_log.ErrorException(ErrorMessage, e);
                throw;
            }

        }
    }
}
