using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface
{
    public interface IRuleSetDataStore
    {
        /// <summary>
        /// Add new RuleSet.
        /// </summary>
        Guid InsertUpdateRuleSet(RuleSetDTO ruleSetDTO);

        /// <summary>
        /// Get a RuleSet by ID.
        /// </summary>
        RuleSetDTO SelectRuleSetBy(Guid ruleSetGuid);

        /// <summary>
        /// Get RuleSet.
        /// </summary>
        List<RuleSetDTO> SelectAllActiveRuleSets(long hospitalId);

        /// <summary>
        /// Filter RuleSets.
        /// </summary>
        List<RuleSetDTO> SelectRuleSetsOn(Guid? rulesetGuid, long? departmetnID, string searchTerm, bool? getActive,bool getHospitalLevel, long hospitalId);
    }
}
