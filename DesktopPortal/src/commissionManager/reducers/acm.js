import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    activeScale:{},
    scaleSearchResult:[],
    ext:[]
};
let acmReducerMap = {};

acmReducerMap[actionTypes.ACMENT_PARAM_ADD] = function (state, action) {
    console.log("readucer新增业绩分摊" + JSON.stringify(action.payload));
    return Object.assign({}, state, {activeScale:action.payload, operInfo: { operType: 'add', dialogOpen: true } });
}
acmReducerMap[actionTypes.ACMENT_PARAM_ITEM_ADD] = function (state, action) {
    console.log("readucer新增业绩分摊" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'itemadd', dialogOpen: true } });
}
acmReducerMap[actionTypes.ACMENT_PARAM_EDIT] = function (state, action) {
    console.log("readucer修改业绩分摊" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit', dialogOpen: true },activeScale:action.payload});
}
acmReducerMap[actionTypes.ACMENT_PARAM_LIST_UPDATE] = function (state, action) {
    console.log("更新业绩分摊列表数据" + JSON.stringify(action.payload));
    return Object.assign({}, state, {scaleSearchResult:action.payload,operInfo:{operType:'ACMENT_PARAM_LIST_UPDATE'}});
}
acmReducerMap[actionTypes.ACMENT_PARAM_DLGCLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'ACMENT_PARAM_DLGCLOSE'} });
}
acmReducerMap[actionTypes.ACMENT_PARAM_ITEM_SAVEUPDATE] = function (state, action) {
    console.log("reducer item update" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'itemupdate'} });
}
acmReducerMap[actionTypes.ACMENT_PARAM_UPDATE] = function (state, action) {
    console.log("reducer param update" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'ACMENT_PARAM_UPDATE'} });
}
acmReducerMap[actionTypes.ACMENT_PARAM_DEL_UPDATE] = function (state, action) {
    console.log("reducer del update" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'ACMENT_PARAM_DEL_UPDATE'} });
}
acmReducerMap[actionTypes.ACMENT_PARAM_ITEM_GET_UPDATE] = function (state, action) {
    console.log("reducer acment items get" + JSON.stringify(action.payload));
    return Object.assign({}, state, {ext:action.payload.extension, operInfo: { objType: '', operType: 'ACMENT_PARAM_ITEM_GET_UPDATE'} });
}
export default handleActions(acmReducerMap, initState)