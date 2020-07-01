import { API } from './APICallBase';

export async function GetTag(tagGUID) {
    const endpoint = `tag/get/${tagGUID}`;

    let config = {
        headers: { },
        params: {},
      };
    
    let api = await API();
    return api.get(endpoint, config);
}

export async function FilterAndGetTags(departmentId, term, getInactive, isHospitalLevel) {
    const endpoint = `tag/search`;

    let config = {
        headers: { },
        params: { 
          departmentId: departmentId == 0 ? null : departmentId,
          term: term,
          getInactive: getInactive,
          isHospitalLevel: departmentId  ? false : isHospitalLevel
        },
      };
    
    let api = await API();
    return api.get(endpoint, config);
}

export async function DateTimeCommonFormat() {
  const endpoint = `dateFormatConfig/getActive`;

  let api = await API();
  return api.get(endpoint);
}

export async function SaveTag(tagObject) {
    const endpoint = `tag/save`;

    let api = await API();
    return api.post(endpoint, tagObject);
}

export async function FilterAndGetTagsForModal(departmentId, term) {
  const endpoint = `tag/searchTags`;

  let config = {
      headers: { },
      params: { 
        departmentId: departmentId == 0 ? null : departmentId,
        term: term
      },
    };
  
  let api = await API();
  return api.get(endpoint, config);
}