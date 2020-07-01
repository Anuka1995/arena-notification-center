using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Exceptions;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.Infrastructure.Logging;
using DIPS.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DIPS.AptSMS.ConfigClient.API.AspNet.Controllers
{
    [Route("textTemplate/")]
    [ApiController]
    public class TextTemplateController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(TagController));
        private readonly ITextTemplateService m_textTeplateService;
        private readonly IUser m_user;
        public TextTemplateController(ITextTemplateService textTeplateService, IUser user)
        {
            m_textTeplateService = textTeplateService;
            m_user = user;
        }

        [HttpPost, Route("Preview")]
        public ActionResult GetPreviewString([FromBody]PreviewModel templateStr)
        {
            m_log.Info("Calling GetPreview");
            try
            {
                var previewtxt = m_textTeplateService.GetPreview(templateStr.SMSTextTemplate, templateStr.isPathRequired, m_user.HospitalId);
                return Ok(previewtxt);
            }
            catch (Exception ex)
            {
                var error = "Failed to generate preview";
                m_log.ErrorException(error, ex);
                throw;
            }
        }

        [HttpPost, Route("save")]
        public ActionResult CreateUpdateTextTemplate([FromBody] SMSText smsText)
        {
            try
            {
                m_log.Info("Calling textTemplate/save endpoint");
                if (m_user.HospitalId == 0)
                {
                    var ErrorMessage = "Failed to load hospital ID from the User ";
                    m_log.Error(ErrorMessage);
                    throw new NullOrEmptyValueException(ErrorMessage);
                }

                if (string.IsNullOrEmpty(smsText.TextTemplateName))
                    throw new UserInputValidationException("Text Template name cannot be empty");

                if (smsText.RulesetId == null || smsText.RulesetId == Guid.Empty)
                    throw new UserInputValidationException("Cannot add empty schedule for a text template");

                smsText.HospitalId = m_user.HospitalId;
                var textTemplateId = m_textTeplateService.SaveSMSTextTemplate(smsText);
                return Ok(textTemplateId);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at textTemplate/save endpoint.", e);
                var ErrorMessage = "Save SMS text template failed!";
                m_log.ErrorException(ErrorMessage, e);
                throw;
            }
        }

        [HttpGet, Route("filter")]
        public ActionResult SearchSMSTextTemplate(
            [FromQuery]long? departmentID,
            [FromQuery]long? opdID,
            [FromQuery]long? sectionID,
            [FromQuery]long? wardID,
            [FromQuery]string searchTerm,
            [FromQuery] bool isActive = true,
            [FromQuery] bool isHospitalOnly = false)
        {
            try
            {
                m_log.Info("Calling textTemplate/filter endpoint");
                if (m_user.HospitalId == 0)
                {
                    var ErrorMessage = "Failed to load hospital ID from the User ";
                    m_log.Error(ErrorMessage);
                    throw new NullOrEmptyValueException(ErrorMessage);
                }

                var textTemplateId = m_textTeplateService.GetSMSTextTemplateTreeNodes(departmentID, opdID, sectionID, wardID, searchTerm, isActive, isHospitalOnly, m_user.HospitalId);
                return Ok(textTemplateId);

            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at textTemplate/filter endpoint.", e);
                throw;
            }
        }

        [HttpGet, Route("get/{TextTemplateId}")]
        public ActionResult GetSMSTextTemplateForSmsTextID([FromRoute]Guid TextTemplateId)
        {
            try
            {
                m_log.Info("Calling textTemplate/get/{TextTemplateId} endpoint");
                if (m_user.HospitalId == 0)
                {
                    var ErrorMessage = "Failed to load hospital ID from the User ";
                    m_log.Error(ErrorMessage);
                    throw new NullOrEmptyValueException(ErrorMessage);
                }
                if (TextTemplateId == Guid.Empty || TextTemplateId == null)
                {
                    var ErrorMessage = "Empty of Null value not accepted for SMS text template guid.";
                    m_log.Error(ErrorMessage);
                    throw new UserInputValidationException(ErrorMessage);
                }
                var textTemplate = m_textTeplateService.GetSMSTextTemplateById(TextTemplateId);
                return Ok(textTemplate);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception thrown at textTemplate/get/{TextTemplateId} endpoint.", e);
                throw;
            }
        }
        [HttpGet, Route("advancefilter")]
        public ActionResult SearchSMSTextBySchedule([FromQuery] Guid scheduleId, [FromQuery] long? departmentID, [FromQuery]long? sectionId, [FromQuery]long? oPDId, [FromQuery]long? wardID, [FromQuery]long? locationId, [FromQuery] List<int> contactTypes, [FromQuery] List<int> officialLevelofcare)
        {
            try
            {
                long hospitalID = m_user.HospitalId;
                //Select Hospital Level Rule Set and keep all other org fields null.
                if (scheduleId != null && departmentID == null && wardID == null && sectionId == null && oPDId == null)
                {
                    var textTemplates = m_textTeplateService.GetEnhancedSearchOnHospitalLevel(scheduleId, hospitalID, locationId, contactTypes, officialLevelofcare);

                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }

                }
                //Hospital level Schedule and only OPD set from hospital level
                if (scheduleId != null && departmentID == null && wardID == null && sectionId == null && oPDId != null)
                {
                    var textTemplates = m_textTeplateService.GetEnhancedSearchOnHospitalLevel_OPD(scheduleId, hospitalID, oPDId.Value, locationId, contactTypes, officialLevelofcare);

                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }

                }
                //Assume : same time only can select either OPD  or ward .
                //Search on Ward Id (section is not set) ,As section is null , this ward  should directly bind to DEP.
                if (scheduleId != null && departmentID != null && wardID != null && sectionId == null)
                {
                    var textTemplates = m_textTeplateService.GetEnhancedSearchByWard(scheduleId, departmentID.Value, wardID.Value, hospitalID, locationId, contactTypes, officialLevelofcare);
                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0) {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }

                }
                if (scheduleId != null && departmentID != null && oPDId != null && sectionId == null)
                {
                    //Search on OPD id ,OPD is bind to DEP
                    var textTemplates = m_textTeplateService.GetEnhancedSearchByOPDId(scheduleId, departmentID.Value, oPDId.Value, hospitalID, locationId, contactTypes, officialLevelofcare);
                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }
                }
                //Search on Ward +Section 
                if (scheduleId != null && departmentID != null && oPDId == null && wardID != null && sectionId != null)
                {

                    var textTemplates = m_textTeplateService.GetEnhancedSearchByWard_BySection(scheduleId, departmentID.Value, wardID.Value, sectionId.Value, hospitalID, locationId, contactTypes, officialLevelofcare);
                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }
                }

                //Only Section Set(Both OPd and ward not set)
                if (scheduleId != null && departmentID != null && oPDId == null && wardID == null && sectionId != null)
                {
                    var textTemplates = m_textTeplateService.GetEnhancedSearchBySection(scheduleId, departmentID.Value, sectionId.Value, hospitalID, locationId, contactTypes, officialLevelofcare);
                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }
                }

                //only set mandatory filelds (only schedule and dep) ward or section or opd not set
                if (scheduleId != null && departmentID != null && oPDId == null && wardID == null && sectionId == null)
                {
                    var textTemplates = m_textTeplateService.GetEnhancedSearchByDep(scheduleId, departmentID.Value, hospitalID, locationId, contactTypes, officialLevelofcare);
                    if (textTemplates == null) { return new EmptyResult(); }
                    if (textTemplates.Count == 0)
                    {
                        return new EmptyResult();
                    }
                    else if (textTemplates.Count == 1)
                    {
                        return Ok(textTemplates[0]);
                    }
                    else
                    {
                        m_log.Debug($"More than one Text Template Selected count={0} with textTemplates ={1}", textTemplates.Count, textTemplates);
                        return Ok(textTemplates[0]);
                    }
                }
                return BadRequest("Requested org unit combination was not found");
            }
            catch (Exception ex)
            {
                var ErrorMessage = "Advance filter for Sms Text String was failed with exception";
                m_log.ErrorException("Advance filter for Sms Text String was failed with exception", ex);
                throw new UserInputValidationException(ErrorMessage);
            }

        }

    }
}