using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface
{
    public interface IRuleSetDataService
    {
        ///<summary>
        ///Save A RuleSet
        ///</summary>
        Guid SaveRuleSet(RuleSetDTO rulseSet);

        ///<summary>
        ///Get a ruleset by guid
        ///</summary>
        RuleSetDTO GetRuleSetBy(Guid rulesetGuid);

        ///<summary>
        ///Get all active rulesets
        ///</summary>
        List<RuleSetDTO> GetAllActiveRuleSets(long hospitalId);

        ///<summary>
        ///Search for RuleSets
        /// </summary>
        List<RuleSetDTO> SearchRuleSets(Guid? ruleSetGUID, long? departmentid, string searchterm, bool? getActive,bool getHospitalLevel, long hospitalId);
    }
}
