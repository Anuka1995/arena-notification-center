using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.API.Interface;
using DIPS.AptSMS.ConfigClient.Common.Models;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Server
{
    public class OverviewService : IOverviewService
    {
        private readonly IOrgUnitsService m_orgService;
        private readonly ITextTemplateService m_textTemplateService;

        private static ILog s_log = LogProvider.GetLogger(typeof(OverviewService));

        public OverviewService(IOrgUnitsService orgService, ITextTemplateService templateService)
        {
            m_orgService = orgService;
            m_textTemplateService = templateService;
        }
        public List<DepartmentNode> GenerateOverviewTree(bool getInactive, long hospitalId, Guid SecurityToken)
        {
            var isActive = !getInactive;
            var departments = m_orgService.GetDepartmentListByHospitalId(hospitalId, SecurityToken);
            var departmentNodes = new List<DepartmentNode>();

            var textTemplates = m_textTemplateService.GetFullSMSTextTemplatesFor(isActive, hospitalId);

            // Global ---> Ruleset.Deptid is null AND Template.Deptid is null
            var templatesForGlobalSchedules = textTemplates.Where(t =>
                (t.DepartmentId == null && t.TemplateDepartmentId == null)).ToList();
            var globalRuleSetNodes = GroupByRSAndGetNodes(templatesForGlobalSchedules, true, hospitalId, SecurityToken);

            foreach (var department in departments)
            {
                //Department Schedules ---> Ruleset.DeptId = deptid OR Template.Deptid = deptid
                var deptTempalts = textTemplates.Where(t =>
                    (t.DepartmentId == department.DepartmentId || t.TemplateDepartmentId == department.DepartmentId)).ToList();
                //var scheduleNodes = new List<RuleSetNode>();
                var scheduleNodes = GroupByRSAndGetNodes(deptTempalts, false, hospitalId, SecurityToken);

                //Remove global ruleset if it has excluded the same department considering here.
                var filteredGlobalRuleSets = new List<RuleSetNode>();
                foreach (var globalRuleSet in globalRuleSetNodes)
                {
                    if (globalRuleSet.ExcludedOrgUnitNames != null && globalRuleSet.ExcludedOrgUnitNames.Any())
                    {
                        if (!globalRuleSet.ExcludedOrgUnitNames.Any(c => c == department.UnitGid))
                        {
                            filteredGlobalRuleSets.Add(globalRuleSet);
                        }
                    }
                    else
                    {
                        filteredGlobalRuleSets.Add(globalRuleSet);
                    }
                }

                //Add global schedules/templates into all the departments
                scheduleNodes.AddRange(filteredGlobalRuleSets);

                if (scheduleNodes.Count == scheduleNodes.Select(i => i.RuleSetId).Distinct().Count())
                {
                    var depNode = new DepartmentNode
                    {
                        Name = department.DepartmentName,
                        Id = department.DepartmentId,
                        Schedules = scheduleNodes
                    };
                    departmentNodes.Add(depNode);
                }
                else
                {
                    var depNode = new DepartmentNode
                    {
                        Name = department.DepartmentName,
                        Id = department.DepartmentId,
                        Schedules = GetDistinctRuleSetNode(scheduleNodes)
                    };
                    departmentNodes.Add(depNode);
                }

            }
            return departmentNodes;
        }

        #region Private methods

        private List<RuleSetNode> GetDistinctRuleSetNode(List<RuleSetNode> currentRuleSetList)
        {
            //Filter ruleset list to identify duplication
            var distinctRuleSetNodeList = new List<RuleSetNode>();
            foreach (var ruleSet in currentRuleSetList)
            {
                if (distinctRuleSetNodeList.Any(c => c.RuleSetId == ruleSet.RuleSetId))
                    continue;

                var selectedList = currentRuleSetList.Where(x => x.RuleSetId == ruleSet.RuleSetId).ToList();

                if (selectedList.Count > 1)
                {
                    var globalRuleSet = selectedList.Where(c => c.IsGlobal == true).FirstOrDefault();
                    if (globalRuleSet != null)
                    {
                        var generatedRS = new RuleSetNode
                        {
                            TextTemplates = new List<TextTemplateNode>(),
                            RuleSetId = globalRuleSet.RuleSetId,
                            ExcludedOrgUnitNames = globalRuleSet.ExcludedOrgUnitNames,
                            Name = globalRuleSet.Name,
                            IsGlobal = globalRuleSet.IsGlobal,
                            DaysCountBeforeApt = globalRuleSet.DaysCountBeforeApt
                        };

                        generatedRS.TextTemplates.AddRange(globalRuleSet.TextTemplates);

                        foreach (var rule in selectedList)
                        {
                            if (!rule.IsGlobal)
                            {
                                generatedRS.TextTemplates.AddRange(rule.TextTemplates);
                            }
                        }
                        distinctRuleSetNodeList.Add(generatedRS);
                    }
                }
                else
                {
                    distinctRuleSetNodeList.Add(ruleSet);
                }
            }
            return distinctRuleSetNodeList;
        }

        private List<RuleSetNode> GroupByRSAndGetNodes(List<SMSText> templateDtoList, bool globalRuleset, long hospitalId, Guid securityToken)
        {
            var groupedByRS = templateDtoList.GroupBy(t => t.RulesetId)
                 .Select(rsGroup => new { RuleSetId = (Guid)rsGroup.Key, TemplatesOnRS = rsGroup.ToList() })
                 .ToList();
            var scheduleNodes = groupedByRS.Select(rs =>
                new RuleSetNode()
                {
                    Name = rs.TemplatesOnRS.First().RuleSetName,
                    IsGlobal = globalRuleset,
                    DaysCountBeforeApt = rs.TemplatesOnRS.First().SendSMSBeforeDays,
                    ExcludedOrgUnitNames = rs.TemplatesOnRS.First().EcludedOrgIds,
                    RuleSetId = (Guid)rs.RuleSetId,
                    TextTemplates = GetTemplateNodes(rs.TemplatesOnRS, hospitalId, securityToken)
                }).ToList();
            return scheduleNodes;
        }

        private List<TextTemplateNode> GetTemplateNodes(List<SMSText> templatesGroup, long hospitalId, Guid securityToken)
        {
            var templateNodes = new List<TextTemplateNode>();
            foreach (var tmpl in templatesGroup)
            {
                if (tmpl.TextTemplateId == null)
                    continue;

                templateNodes.Add(new TextTemplateNode()
                {
                    Name = tmpl.TextTemplateName,
                    TempalteId = (Guid)tmpl.TextTemplateId,
                    IsSMSGenerate = tmpl.IsGenerateSMS,
                    TargetOrgUnitName = FindTargetOrgID(hospitalId, securityToken,
                        tmpl.TemplateDepartmentId, tmpl.OPDId, tmpl.SectionId, tmpl.WardId),
                    LocationName = FindTargetLocationID(hospitalId, securityToken, tmpl.LocationId),
                    TargetOpdName = FindTargetOpdID(hospitalId, securityToken, tmpl.OPDId)
                });

            }
            return templateNodes;
        }
        private string FindTargetOrgID(long hospitalId, Guid securityToken, long? templateDepartmentId, long? OPDId, long? sectionId, long? wardId)
        {
            if (wardId != null)
                return m_orgService.FindNameForOrgUnitId(hospitalId, securityToken, OrganizationalUnitType.Ward.ToString(), (long)wardId);
            else if (sectionId != null)
                return m_orgService.FindNameForOrgUnitId(hospitalId, securityToken, OrganizationalUnitType.Section.ToString(), (long)sectionId);
            else if (templateDepartmentId != null)
                return m_orgService.FindNameForOrgUnitId(hospitalId, securityToken, OrganizationalUnitType.Department.ToString(), (long)templateDepartmentId);
            else
                return null;
        }

        private string FindTargetOpdID(long hospitalId, Guid securityToken, long? OPDId)
        {
            return (OPDId == null ? null : m_orgService.FindNameForOrgUnitId(hospitalId, securityToken, OrganizationalUnitType.OPD.ToString(), (long)OPDId));
        }

        private string FindTargetLocationID(long hospitalId, Guid securityToken, long? locationId)
        {
            return (locationId == null ? null : m_orgService.FindNameForOrgUnitId(hospitalId, securityToken, OrganizationalUnitType.Location.ToString(), (long)locationId));
        }
        #endregion
    }
}
