//所有的reducer在这里合并
import {combineReducers} from 'redux';
import {routerReducer} from 'react-router-redux'
import {reducer as oidcReducer} from 'redux-oidc';
import ppFtReducer from './ppft'
import treeReducer from './org'
export default combineReducers({
    router: routerReducer,
    oidc: oidcReducer,
    ppft: ppFtReducer,
    org:  treeReducer
});