import { API } from './APICallBase';

export async function GetFormats() {
    const endpoint = `dateFormatConfig/get`;

    let config = {
        headers: { },
        params: {},
      };
    
    let api = await API();
    return api.get(endpoint, config);
}

export async function UpdateFormats(DateTimeFormat) {
    const endpoint = `dateFormatConfig/update`;
    
    let api = await API();
    return api.post(endpoint, DateTimeFormat);
}


export async function GetPreview(foramtStr, displaySample) {
    const endpoint = `dateFormatConfig/getPreview`;
    let config = {
        headers: { },
        params: {
            format: foramtStr,
            dateSample: displaySample
        },
      };
    
    let api = await API();
    return api.get(endpoint,  config );
}

