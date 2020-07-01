using DIPS.AptSMS.ConfigClient.DataAccessHandler.Common.DTO;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Interface;
using DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.Interface;
using System;
using System.Collections.Generic;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server
{
    public class RuleSetDataService : IRuleSetDataService
    {
        private readonly IRuleSetDataStore m_rulesetDataStore;

        public RuleSetDataService(IRuleSetDataStore ruleSetDataStore)
        {
            m_rulesetDataStore = ruleSetDataStore;
        }

        public List<RuleSetDTO> GetAllActiveRuleSets(long hospitalId)
        {
            return m_rulesetDataStore.SelectAllActiveRuleSets(hospitalId);
        }

        public RuleSetDTO GetRuleSetBy(Guid rulesetGuid)
        {
            return m_rulesetDataStore.SelectRuleSetBy(rulesetGuid);
        }

        public Guid SaveRuleSet(RuleSetDTO rulseSet)
        {
            return m_rulesetDataStore.InsertUpdateRuleSet(rulseSet);
        }

        public List<RuleSetDTO> SearchRuleSets(Guid? ruleSetGUID, long? departmentid, string searchterm, bool? getActive,bool getHospitalLevel, long hospitalId)
        {
            return m_rulesetDataStore.SelectRuleSetsOn(ruleSetGUID, departmentid, searchterm, getActive, getHospitalLevel, hospitalId);
        }
    }
}
