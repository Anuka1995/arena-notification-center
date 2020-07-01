import { API } from './APICallBase';

export async function SaveTextTemplate(template) {

    const endpoint = `textTemplate/save`;
    let api = await API();
    return api.post(endpoint, template);
}

export async function GetPreviewModal(textValue, pssLink) {
    const endpoint = `textTemplate/Preview`;
    let api = await API();
    return api.post(endpoint, {
        SMSTextTemplate: textValue,
        isPathRequired: pssLink
    });
}

export async function SearchTextTemplateBy(departmentId, opdId, sectionId, wardId, searchTerm, showInActive, isHospitalOnly) {
    const endpoint = `textTemplate/filter`;
    let config = {
        headers: {},
        params: {
            departmentId: departmentId,
            opdId: opdId,
            sectionId: sectionId,
            wardId: wardId,
            searchterm: searchTerm,
            isActive: showInActive,
            isHospitalOnly: isHospitalOnly
        },
    };
    let api = await API();
    return api.get(endpoint, config);
}

    export async function FilterTextTemplatBy(scheduleId,departmentID,sectionId,oPDId,wardID,locationId,contactTypes,officialLevelofcare) {
        const endpoint = `textTemplate/advancefilter`;
        //showInActive can be used when sms text template is using the active/inactive feature.
        let config = {
            headers: {},
            params: {
                scheduleId: scheduleId,
                departmentID: departmentID,
                sectionId: sectionId,
                oPDId: oPDId,
                wardID: wardID,
                locationId:locationId,
                contactTypes:contactTypes,
                officialLevelofcare:officialLevelofcare
            },
        };
    
    let api = await API();
    return api.get(endpoint, config);
}

export async function GetSMSTextTemplateBy(smsTextId) {
    const endpoint = `textTemplate/get/${smsTextId}`;

    let config = {
        headers: {},
        params: {
        },
    };

    let api = await API();
    return api.get(endpoint, config);
}
