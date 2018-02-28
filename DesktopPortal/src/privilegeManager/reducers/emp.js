import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    activeEmp: {},
    operInfo: { operType: '', empRoleOperType: '' },
    empList: [],
    privList: [],
    empSearchResult: []//用户查询结果
};
let empReducerMap = {};

empReducerMap[actionTypes.EMP_ADD] = function (state, action) {
    console.log("readucer用户新增" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: 'emp', operType: 'add', dialogOpen: true } });
}
empReducerMap[actionTypes.EMP_EDIT] = function (state, action) {
    console.log("readucer用户编辑" + JSON.stringify(action.payload));
    return Object.assign({}, state, { activeEmp: action.payload, operInfo: { objType: 'emp', operType: 'edit', dialogOpen: true } });
}

empReducerMap[actionTypes.EMP_LIST_UPDATE] = function (state, action) {
    console.log("readucer用户的列表:" + JSON.stringify(action.payload));
    return Object.assign({}, state, { empList: action.payload });
}

empReducerMap[actionTypes.EMP_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: '', empRoleOperType: '' } });
}

empReducerMap[actionTypes.EMP_PRIV_LIST_UPDATE] = function (state, action) {
    console.log("readucer获取职级", action.payload);
    return Object.assign({}, state, { privList: action.payload.extension });
}
//用户角色编辑
empReducerMap[actionTypes.EMP_ROLE_EDIT] = function (state, action) {

    return Object.assign({}, state, { activeEmp: action.payload, operInfo: { empRoleOperType: 'edit' } });
}
//查询
empReducerMap[actionTypes.EMP_SERACH_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { empSearchResult: action.payload });
}

//设置遮罩层
empReducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}
export default handleActions(empReducerMap, initState)