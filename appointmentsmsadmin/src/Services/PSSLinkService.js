import { API } from './APICallBase';

export async function SaveHospitalURL(PSSLinkModel) {
    const endpoint = `PSSLinkPage/save`
    let api = await API();
    return api.post(endpoint, PSSLinkModel);
}

export async function GetHospitalURL() {
    const endpoint = `PSSLinkPage/get`
    let api = await API();
    return api.get(endpoint);
}