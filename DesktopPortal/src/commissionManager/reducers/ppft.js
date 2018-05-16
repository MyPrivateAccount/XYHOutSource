import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    activePft:{},
    ppFtSearchResult:[]
};
let ppFtReducerMap = {};

ppFtReducerMap[actionTypes.ORG_FT_PARAM_ADD] = function (state, action) {
    console.log("readucer新增人数组织分摊参数" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'add', dialogOpen: true } });
}
ppFtReducerMap[actionTypes.ORG_FT_PARAM_EDIT] = function (state, action) {
    console.log("readucer修改人数组织分摊参数" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit', dialogOpen: true },activePft:state.payload});
}
ppFtReducerMap[actionTypes.ORG_FT_PARAMLIST_UPDATE] = function (state, action) {
    console.log("更新" + JSON.stringify(action.payload));
    return Object.assign({}, state, {ppFtSearchResult:action.payload});
}
ppFtReducerMap[actionTypes.ORG_FT_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: ''} });
}
export default handleActions(ppFtReducerMap, initState)