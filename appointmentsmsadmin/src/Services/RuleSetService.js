import { API } from './APICallBase';

export async function SaveRuleSet(rulesetObject) {
      const endpoint = `ruleset/save`;
  
      let api = await API();
      return api.post(endpoint, rulesetObject);
  }

export async function GetRuleSetList() {
    const endpoint = 'ruleset/all/active';
    let api = await API();
    return api.get(endpoint);
}

export async function GetRuleSet(guid) {
    const endpoint = `ruleset/getrulesetbyid`;
    var config = {
      headers: {},
      params: {
        ruleSetguid: guid
       }
    }
    let api = await API();
    return api.get(endpoint, config);
}
  export async function SearchRuleSetBy(departmentId, term,isGetInactive,getHospitalLevel) {
    const endpoint = `ruleset/searchRuleSet`;

    let config = {
        headers: { },
        params: { departmentId: departmentId, searchterm: term ,getInactive:isGetInactive,getHospitalLevel:getHospitalLevel},
      };
    
    let api = await API();
    return api.get(endpoint, config);
}



export async function GetAvailableOPDList(departmentId, ruleSetId) {
    let config = {
        params: {
          departmentid: departmentId,
          RuleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/department/availableOPD`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function GetAvailableSectionList(departmentId ,ruleSetId) {
    let config = {
        params: {
          departmentid: departmentId,
          RuleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/department/availableSection`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function GetAvailableWardList(departmentId ,sectionId,ruleSetId) {
    let config = {
        params: {
          departmentid: departmentId,
          sectionId:sectionId,
          RuleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/department/availableWard`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function GetRuleSetById(ruleSetId) {
    let config = {
        params: {
            ruleSetguid:ruleSetId
        }
    }
    const endpoint = `ruleset/getrulesetbyid`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function GetAvailableLocationList(departmentId ,ruleSetId) {
    let config = {
        params: {
          departmentid: departmentId,
          ruleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/department/availableLocation`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function GetAvailableDepartmentList(ruleSetId) {
    let config = {
        params: {         
          RuleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/hospital/availableDepartment`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

  export async function CheckIsDepartmentExluded(depId,ruleSetId) {
    let config = {
        params: {
          departmentId: depId,         
          RuleSetId:ruleSetId
        }
    }
    const endpoint = `ruleset/department/isExcluded`;
  
    let api = await API();
    return api.get(endpoint, config);
  }

