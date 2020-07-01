using DIPS.AptSMS.ConfigClient.API.Common.TreeView;
using DIPS.AptSMS.ConfigClient.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.API.Interface
{
    public interface IRuleSetService
    {
        Guid SaveRuleSet(RuleSet ruleSet);
        List<RuleSetListItem> GetAllActiveRuleSets(long hospitalID);
        List<RuleSet> SearchRuleSet(long? departmentid, string searchterm, bool getInactive, bool getHospitalLeve, long hospitalId);
        RuleSet GetRuleSetById(Guid RuleSetId);
        List<DepartmentListItem> GetDepartmentListForRuleSetId(long hospitalId, Guid ruleSetId, Guid SecurityToken);
        List<OPDListItem> GetOPDListForRuleSetId(long? departmentId, long hospitalId, Guid ruleSetId, Guid SecurityToken);
        List<SectionListItem> GetSectionListForRuleSetId(long departmentId, Guid ruleSetId, Guid SecurityToken);
        List<WardListItem> GetWardListForRuleSetId(long? departmentId, long? sectionId, Guid ruleSetId, Guid SecurityToken);
        List<LocationListItem> GetLocationListForRuleSetId(long departmentId, Guid ruleSetId, Guid SecurityToken);
        List<TreeNode> GetRuleSetTreeNodes(List<RuleSet> ruleSetList);
        bool IsDepartmentExcluded(long DepartmentId, Guid ruleSetId, Guid SecurityToken);
    }
}
