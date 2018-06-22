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
    return Object.assign({}, state, {activePft:action.payload, operInfo: { operType: 'add', dialogOpen: true } });
}
ppFtReducerMap[actionTypes.ORG_FT_PARAM_EDIT] = function (state, action) {
    console.log("readucer修改人数组织分摊参数" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit', dialogOpen: true },activePft:action.payload});
}
ppFtReducerMap[actionTypes.ORG_FT_PARAMLIST_UPDATE] = function (state, action) {
    console.log("更新" + JSON.stringify(action.payload));
    return Object.assign({}, state, {ppFtSearchResult:action.payload,operInfo:{operType:'ORG_FT_PARAMLIST_UPDATE'}});
}
ppFtReducerMap[actionTypes.ORG_FT_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: ''} });
}
ppFtReducerMap[actionTypes.ORG_FT_PARAM_SAVE_SUCCESS] = function (state, action) {
    console.log("readucer保存成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'FT_PARAM_SAVE_SUCCESS'} });
}
ppFtReducerMap[actionTypes.ORG_FT_PARAM_DELETE_UPDATE] = function (state, action) {
    console.log("readucer删除成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'ORG_FT_PARAM_DELETE_UPDATE'} });
}
export default handleActions(ppFtReducerMap, initState)