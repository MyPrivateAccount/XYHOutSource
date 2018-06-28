import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    activeScale:{},
    scaleSearchResult:[],
    param:{}
};
let scaleReducerMap = {};

scaleReducerMap[actionTypes.INCOME_SCALE_ADD] = function (state, action) {
    console.log("readucer新增提成比例" + JSON.stringify(action.payload));
    return Object.assign({}, state, {param:action.payload, operInfo: { operType: 'add', dialogOpen: true } });
}
scaleReducerMap[actionTypes.INCOME_SCALE_EDIT] = function (state, action) {
    console.log("readucer修改提成比例" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit', dialogOpen: true },activeScale:action.payload});
}
scaleReducerMap[actionTypes.INCOME_SCALE_LIST_UPDATE] = function (state, action) {
    console.log("更新提成比例列表" + JSON.stringify(action.payload));
    return Object.assign({}, state, {scaleSearchResult:action.payload,operInfo:{operType:'INCOME_SCALE_LIST_UPDATE'}});
}
scaleReducerMap[actionTypes.INCOME_SCALE_DLGCLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'INCOME_SCALE_DLGCLOSE'} });
}
scaleReducerMap[actionTypes.INCOME_SCALE_DEL_UPDATE] = function (state, action) {
    console.log("readucer删除成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'INCOME_SCALE_DEL_UPDATE'} });
}
scaleReducerMap[actionTypes.INCOME_SCALE_SAVE_SUCCESS] = function (state, action) {
    console.log("readucer新增成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: 'INCOME_SCALE_SAVE_SUCCESS'} });
}
export default handleActions(scaleReducerMap, initState)