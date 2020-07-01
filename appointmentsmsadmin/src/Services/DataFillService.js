import { API } from './APICallBase';

export async function GetRuleSetList() {
    const endpoint = 'orgunit/departmentList';
    try {
        let api = await API();
        const departmentsList = api.get(endpoint)
            .then(result => {
                const data = result.data;
                return data;
            }
            );
        return departmentsList;
    }
    catch (error) {
        //Handle the error
        console.log(error);
    }
}