import { API } from './APICallBase';

export async function SaveGroupTemplate(template) {
    const endpoint = `templategroup/save`;

    let api = await API();
    return api.post(endpoint, {
        DepartmentId: parseInt(template.DepartmentId),
        TextTemplateName: template.TextTemplateName,
        TextTemplateString: template.TextTemplateString,
        TextTemplateTextId: template.TextTemplateTextId,
        IsActive: true
    });
}

export async function GetGroupedTemplates(departmentId, searchText, isHospitalLevel) {

    const endpoint = `templategroup/filter`;
    let config = {
        headers: {},
        params: {
            departmentID: departmentId == 0 ? null : departmentId,
            searchTerm: searchText,
            ishospitalOnly: isHospitalLevel
        },
    };

    let api = await API();
    return api.get(endpoint, config);
}

export async function GetGroupedTemplate(guid) {
    const endpoint = `templategroup/get/${guid}`;

    let config = {
        headers: {},
        params: {},
    };

    let api = await API();
    return api.get(endpoint, config);
}