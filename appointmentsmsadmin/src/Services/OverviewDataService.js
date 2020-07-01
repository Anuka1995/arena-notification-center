import { API } from './APICallBase';

export async function GetOverview(getInactive) {
    const endpoint = `treeviews/overview`;

    let config = {
        headers: { },
        params: {
            getInactive: getInactive
        },
    };

    let api = await API();
    return api.get(endpoint,config);
}