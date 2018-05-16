import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    activeOrgParam:{},
    orgParamSearchResult:[]
};
let orgParamReducerMap = {};

orgParamReducerMap[actionTypes.ORG_PARAM_ADD] = function (state, action) {
    console.log("readucer新增组织参数" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'add', dialogOpen: true } });
}
orgParamReducerMap[actionTypes.ORG_PARAM_EDIT] = function (state, action) {
    console.log("readucer修改组织参数" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit', dialogOpen: true },activeOrgParam:state.payload});
}
orgParamReducerMap[actionTypes.ORG_PARAMLIST_UPDATE] = function (state, action) {
    console.log("更新组织参数列表" + JSON.stringify(action.payload));
    return Object.assign({}, state, {orgParamSearchResult:action.payload});
}
orgParamReducerMap[actionTypes.ORG_PARAM_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: ''} });
}
export default handleActions(orgParamReducerMap, initState)