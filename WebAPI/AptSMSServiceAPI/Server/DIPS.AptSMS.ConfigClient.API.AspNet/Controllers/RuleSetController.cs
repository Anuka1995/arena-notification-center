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
    [Route("ruleset/")]
    [ApiController]
    public class RuleSetController : ControllerBase
    {
        private static readonly ILog m_log = LogProvider.GetLogger(typeof(RuleSetController));
        private readonly IRuleSetService m_ruleSetService;
        private readonly IUser m_user;

        public RuleSetController(IRuleSetService ruleSetService, IUser user)
        {
            m_ruleSetService = ruleSetService;
            m_user = user;
        }

        [HttpPost, Route("save")]
        public ActionResult SaveRuleSet([FromBody]RuleSet ruleSet)
        {
            try
            {
                if (ruleSet.SendBeforeInDays == 0 && ruleSet.MinimumTimeFromMinutes == null)
                {
                    var ErrorMessage = "MinimumTimeFromMinutes should be specified if the SMS is to be sent on the same day";
                    m_log.Error(ErrorMessage);
                    throw new UserInputValidationException(ErrorMessage);
                }
                if (string.IsNullOrEmpty(ruleSet.RulesetName))
                {
                    m_log.Trace($"RuleSet Name cannot be null");
                    var ErrorMessage = "Schedule name cannot be empty";
                    m_log.Error(ErrorMessage);
                    throw new UserInputValidationException(ErrorMessage);
                }
                if (ruleSet.IsAppointmentTimeValidate &&
                    (string.IsNullOrEmpty(ruleSet.AppointmentFrom) || string.IsNullOrEmpty(ruleSet.AppointmentTo)))
                {
                    var ErrorMessage = "Time range is added but the schedule is set NOT to validate the date range";
                    m_log.Error(ErrorMessage);
                    throw new UserInputValidationException(ErrorMessage);
                }

                var avoidOrgUnitList = GetAvoidOrgUnitList(ruleSet.AvoidOrgUnitIds);
                ruleSet.ExcludeOrgUnits = avoidOrgUnitList;
                ruleSet.HospitalId = m_user.HospitalId;
                var guid = m_ruleSetService.SaveRuleSet(ruleSet);

                m_log.Trace($"Rule Set Saved with Name= {ruleSet.RulesetName} , ScheduleTo = {ruleSet.AppointmentFrom}");

                return Ok(guid);
            }
            catch (DBOperationException e)
            {
                var ErrorMessage = "Error in saving the ruleset";
                m_log.ErrorException(ErrorMessage, e);

                if (e.OracleErrorCode == GlobalOptions.DBErrorCodes.Such_a_rule_set_already_exists)
                    throw new UserInputValidationException("Schedule already exists for the same day count!");

                if (e.OracleErrorCode == GlobalOptions.DBErrorCodes.Schedule_exisits_with_Future_Date)
                    throw new UserInputValidationException("Schedule already exists for the same day count BUT with a future start date!");

                throw;
            }
            catch (Exception ex)
            {
                m_log.ErrorException("Save Schedule  failed RuleSetName", ex);
                throw;
            }
        }

        [HttpGet, Route("all/active")]
        public ActionResult GetAllActive()
        {
            var message = "executing GetAllActive";
            m_log.Info(message);

            var hospitalID = m_user.HospitalId;
            var activeRuleSets = m_ruleSetService.GetAllActiveRuleSets(hospitalID);

            return Ok(activeRuleSets);
        }

        [HttpGet, Route("getrulesetbyid")]
        public ActionResult getRuleSetById([FromQuery]Guid ruleSetguid)
        {

            try
            {
                m_log.Trace($"Get Rule Set By Id   ruleSetguid= {ruleSetguid} ");
                var ruleSet = m_ruleSetService.GetRuleSetById(ruleSetguid);
                return Ok(ruleSet);
            }
            catch (Exception ex)
            {
                m_log.ErrorException("Get Rule Set ById Set failed ", ex);
                throw;
            }


        }

        [HttpGet, Route("searchRuleSet")]
        public ActionResult SearchRuleSetBy([FromQuery]long? departmentid, [FromQuery] string searchterm, [FromQuery]bool getInactive, [FromQuery]bool getHospitalLevel)
        {

            try
            {
                m_log.Trace($"Search Rule Set by departmentid = {departmentid}  searchTerm :{searchterm} and with GetInatcive Rules ={getInactive} ");

                var selectedRuleSets = m_ruleSetService.SearchRuleSet(departmentid, searchterm, getInactive, getHospitalLevel, m_user.HospitalId);

                if (selectedRuleSets != null)
                {
                    m_log.Trace($"Search Rule Set result Set Count :{selectedRuleSets.Count}");
                    var treeNodeList = GetTreeNodesForRuleSet(selectedRuleSets);
                    return Ok(treeNodeList);
                }

                throw new Exception("Failed to load Schedules");
            }
            catch (Exception ex)
            {
                m_log.ErrorException("Search Rule Set failed ", ex);
                throw;
            }
        }

        [HttpGet, Route("department/availableOPD")]
        public ActionResult GetOPDForRuleSetId([FromQuery]long? departmentid, [FromQuery] Guid RuleSetId)
        {
            try
            {
                var opdList = m_ruleSetService.GetOPDListForRuleSetId(departmentid, m_user.HospitalId, RuleSetId, m_user.SecurityToken);
                return Ok(opdList);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/department/availableOPD", e);
                throw;
            }
        }

        [HttpGet, Route("department/availableSection")]
        public ActionResult GetSectionForRuleSetId([FromQuery]long departmentid, [FromQuery] Guid RuleSetId)
        {
            try
            {
                var sectionList = m_ruleSetService.GetSectionListForRuleSetId(departmentid, RuleSetId, m_user.SecurityToken);
                return Ok(sectionList);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/department/availableSection", e);
                throw;
            }
        }

        [HttpGet, Route("department/availableWard")]
        public ActionResult GetWardForRuleSetId([FromQuery]long? departmentid, [FromQuery]long? sectionId, [FromQuery] Guid RuleSetId)
        {
            try
            {
                var wardList = m_ruleSetService.GetWardListForRuleSetId(departmentid, sectionId, RuleSetId, m_user.SecurityToken);
                return Ok(wardList);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/department/availableWard", e);
                throw;
            }
        }

        [HttpGet, Route("department/availableLocation")]
        public ActionResult GetLocationForRuleSetId([FromQuery]long departmentid, [FromQuery] Guid ruleSetId)
        {
            try
            {
                var locationList = m_ruleSetService.GetLocationListForRuleSetId(departmentid, ruleSetId, m_user.SecurityToken);
                return Ok(locationList);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/department/availableLocation", e);
                throw;
            }
        }

        [HttpGet, Route("hospital/availableDepartment")]
        public ActionResult GetDepartmentRuleSetId([FromQuery] Guid ruleSetId)
        {
            try
            {
                var departmentList = m_ruleSetService.GetDepartmentListForRuleSetId(m_user.HospitalId, ruleSetId, m_user.SecurityToken);
                return Ok(departmentList);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/hospital/availableDepartment", e);
                throw;
            }
        }

        [HttpGet, Route("department/isExcluded")]
        public ActionResult CheckIsDepartmentExcluded([FromQuery] long departmentId, [FromQuery] Guid ruleSetId)
        {
            try
            {
                var isDepartmentExcluded = m_ruleSetService.IsDepartmentExcluded(departmentId, ruleSetId, m_user.SecurityToken);
                return Ok(isDepartmentExcluded);
            }
            catch (Exception e)
            {
                m_log.ErrorException("Exception in endpoint ruleset/department/isExcluded", e);
                throw;
            }
        }

        #region private
        private List<TreeNode> GetTreeNodesForRuleSet(List<RuleSet> ruleSetList)
        {
            var ruleSetNodes = m_ruleSetService.GetRuleSetTreeNodes(ruleSetList);
            return ruleSetNodes;

        }
        private List<OrgUnit> GetAvoidOrgUnitList(string avoidOrgUnitIds)
        {
            try
            {
                if (avoidOrgUnitIds != string.Empty)
                {
                    var avoidOrgUnitId = avoidOrgUnitIds.Split(',');

                    var avoidOrgUnitList = avoidOrgUnitId.Select(unitId => new OrgUnit()
                    {
                        UnitID = unitId
                    }).ToList();

                    return avoidOrgUnitList;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                m_log.ErrorException("Converting Exclude Org Unit failed in Add Rule Set ", ex);
                throw;
            }
        }
        #endregion
    }

}
