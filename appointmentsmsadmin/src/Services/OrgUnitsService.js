import { API } from './APICallBase';

export async function GetDepartmentList() {
  const endpoint = `orgunit/departmentList`;
  let api = await API();
  return api.get(endpoint);
}

export async function GetLocationsByDepartment(departmentId){
  let config = {
    params: {
        departmentId: departmentId
    }
  }
  const endpoint = `orgunit/department/location`;

  let api = await API();
  return api.get(endpoint, config);
}

export async function GetOPDList(departmentId) {
  let config = {
      params: {
          departmentId: departmentId
      }
  }
  const endpoint = `orgunit/department/opd`;

  let api = await API();
  return api.get(endpoint, config);
}

export async function GetOPDListByHospital() { 
  const endpoint = `orgunit/opdList`;

  let api = await API();
  return api.get(endpoint);
}

export async function GetSectionsByDepartment(departmentId) {
  let config = {
      params: {
          departmentId: departmentId
      }
  }
  const endpoint = `orgunit/department/section`;

  let api = await API();
  return api.get(endpoint, config);
}

export async function GetWardsByDepartment(departmentId) {
  let config = {
      params: {
          departmentId: departmentId
      }
  }
  const endpoint = `orgunit/department/ward`;

  let api = await API();
  return api.get(endpoint, config);
}

export async function GetWardsBySection(sectionId) {
  let config = {
      params: {
          sectionId: sectionId
      }
  }
  const endpoint = `orgunit/section/ward`;

  let api = await API();
  return api.get(endpoint, config);
}

export async function GetFullOrgTree(){
  const endpoint = `orgunit/hospital/fullTree`
  let api = await API();
  return api.get(endpoint);
}

export async function GetDepartmentTree(departmentId){
  let config = {
    params: {
        departmentId: departmentId
    }
  }
  const endpoint = `orgunit/department/fullTree`
  let api = await API();
  return api.get(endpoint, config);
}