import { store } from '../index'

const ApiClient = {
    get(url, useToken=true,  qs) {
        
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        if(useToken){
            const token = store.getState().oidc.user.access_token;
            headers.append('Authorization', `Bearer ${token}`);
            console.log("userToken:",`Bearer ${token}`);
        }
        const options = {
            method: 'GET',
            headers,
            mode: 'cors'
        };
        const params = new URLSearchParams();
        if (qs) {
            Object.keys(qs).forEach(key => params.append(key, qs[key]));
            url = url + "?" + params.toString();
        }

      
        return fetch(url, options)
            .then((res) => res.json())
            .then((data) => ({ data }))
            .catch((error) => ({ error }));
    },
    post(url, body,qs, method = 'POST') {
        const token = store.getState().oidc.user.access_token;
        const headers = new Headers();
        headers.append('Content-Type','application/json');
        headers.append('Accept', 'application/json');
        headers.append('Authorization', `Bearer ${token}`);

        var postData = undefined;
        if (body) {
            postData = JSON.stringify(body);
        }

        const options = {
            method: method,
            headers,
            mode: 'cors',
            body: postData
        };

        const params = new URLSearchParams();
        if (qs) {
            Object.keys(qs).forEach(key => params.append(key, qs[key]));
            url = url + "?" + params.toString();
        }

        return fetch(url, options)
            .then((res) => res.json())
            .then((data) => ({ data }))
            .catch((error) => ({ error }));
    },
    postForm(url, body, qs, method = 'POST', headerSetter) {
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        if (headerSetter) {
            headerSetter(headers);
        }
        const params = new URLSearchParams();
        if (qs) {
            Object.keys(qs).forEach(key => params.append(key, qs[key]));
            url = url + "?" + params.toString();
        }
        //   headers.append('Authorization', `Bearer ${token}`);


        const options = {
            method: method,
            headers,
            mode: 'cors'
        };
        var fd = new FormData();
        if (body) {
            for (var k in body) {
                if (body.hasOwnProperty(k)) {
                    fd.append(k, body[k]);
                }
            }
            options.body = fd;
        }


        return fetch(url, options)
            .then((res) => res.json())
            .then((data) => ({ data }));
    },
    postFormUrlEncode(url, body, qs, method = 'POST', headerSetter) {
        const headers = new Headers();
        headers.append('Content-Type', 'application/x-www-form-urlencoded');
        headers.append('Accept', 'application/json');
        if (headerSetter) {
            headerSetter(headers);
        }
        const params = new URLSearchParams();
        if (qs) {
            Object.keys(qs).forEach(key => params.append(key, qs[key]));
            url = url + "?" + params.toString();
        }

        const options = {
            method: method,
            headers,
            mode: 'cors'
        };
        if (body) {
            const bodyParams = new URLSearchParams();

            Object.keys(body).forEach(key => bodyParams.append(key, body[key]));
            options.body = bodyParams.toString();

        }

        return fetch(url, options)
            .then((res) => res.json())
            .then((data) => ({ data }));
    }
}


export default ApiClient;