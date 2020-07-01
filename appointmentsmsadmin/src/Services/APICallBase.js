import axios from 'axios';

  function parseError (errorResponse) {
    if(errorResponse && errorResponse.config && errorResponse.status){
      return Promise.reject({ 
        message: `Error occured in request. 
        URL: ${errorResponse.config.url} | 
        Status: ${errorResponse.status}, ${errorResponse.statusText} | 
        Data: ${errorResponse.config.data}`,
        statuscode: errorResponse.status,
        cause: errorResponse.data
       });
    } else {
      return Promise.reject({ 
        message: `Error occured in request.`
       });
    }
    
  }

  function parseBody (response) {
    if (response.status === 200) {   
      return response.data;
    } else {
      return parseError(response)
    }
  }

function fetchBaseURL() {
  return fetch('./config.json')
    .then(response => response.json())
    .then(data => {
      return data.SMSAppointmentBaseURL;
    });
}

export async function API() {

  if(axios.defaults.instance) {
     return axios.defaults.instance;
  }
  
  let baseurl = await fetchBaseURL();
  axios.defaults.baseURL = baseurl;

  let instance = axios.create();

  // Do something before request is sent
  instance.interceptors.request.use((config) => {

    // Debug Info
    // console.log(localStorage.getItem('token'));
    // console.log(localStorage.getItem('loggedInData'));
    //

    var authTicket = localStorage.getItem('token');
    if(authTicket != null){
      config.headers = { 'TicketHeader': authTicket };
    }
    return config
    }, 
    error => {
    return Promise.reject(error)
  });

  // Process the response and get return value
  instance.interceptors.response.use((response) => {
      return parseBody(response)
    }, error => {
    if (error.response) {
      return parseError(error.response);
    } else {
      return Promise.reject({ 
        message: `Error occured in request.`
       })
    }
  });

  axios.defaults.instance = instance;

  return instance;
}