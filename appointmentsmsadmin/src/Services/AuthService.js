
import { API } from './APICallBase';

export function Logout() {
    localStorage.removeItem('token');
}

export async function GetTicket(username, password, userrole) {
    const endpoint = `auth/getticket`;

    let api = await API();
    return api.post(endpoint, {
        UserName: username,
        Password: password,
        UserRole: userrole
    });
}

export async function GetUserRoles(logonParams) {
    let data = {
        params: logonParams,
      };

    const endpoint = `auth/userroles`;
    
    let api = await API();
    return api.get(endpoint, data);
}

export async function PokeSession() {

    const endpoint = `auth/poke`;
    
    let api = await API();
    return api.put(endpoint);
}
