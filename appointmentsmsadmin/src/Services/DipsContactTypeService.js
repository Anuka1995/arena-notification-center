import { API } from './APICallBase';

export async function GetCareLevelDetails() {
    const endpoint = `officialLevelOfCare/get`;

    let api = await API();
    return api.get(endpoint); 
}

export async function GetContactTypes() {
    const endpoint = `contactType/get`;

    let api = await API();
    return api.get(endpoint);
}