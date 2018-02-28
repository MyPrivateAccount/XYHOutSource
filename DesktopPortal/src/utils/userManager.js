import { createUserManager } from 'redux-oidc';
import getRootPath from './getRootPath';
import { AuthorUrl } from '../constants/baseConfig';

let root = window.location.pathname;
root = getRootPath(root);


const userManagerConfig = {
    client_id: 'privilegeManager',
    redirect_uri: `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}/callback`,
    response_type: 'token id_token',
    scope: 'openid profile',
    authority: AuthorUrl,//'http://server-d01:5000',
    silent_redirect_uri: `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}/silent_renew.html`,
    automaticSilentRenew: true,
    filterProtocolClaims: true,
    post_logout_redirect_uri: `${window.location.protocol}//${window.location.hostname}${window.location.port ? `:${window.location.port}` : ''}`,
    loadUserInfo: true,
}
if (module.hot) {
    userManagerConfig.client_id = "xtwh";
}
const userManager = createUserManager(userManagerConfig);

export default userManager;