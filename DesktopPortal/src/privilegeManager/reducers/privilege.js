import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import { notification } from 'antd';

const initState = {
    operInfo: { operType: '', snackbar: '' },
    privilegeList: [],
    activePrivilege: {},
    appPrivilegeTree: {},//工具权限树
    rolePrivileges: [],//当前角色拥有的权限
    selectAppId: ''
};
let privilegeReducerMap = {};
//新增模式
privilegeReducerMap[actionTypes.PRIVILEGE_ADD] = function (state, action) {

    return Object.assign({}, state, { selectAppId: action.payload, operInfo: { operType: 'add' } });
}
//编辑模式
privilegeReducerMap[actionTypes.PRIVILEGE_EDIT] = function (state, action) {

    return Object.assign({}, state, { operInfo: { operType: 'edit' }, activePrivilege: action.payload });
}

privilegeReducerMap[actionTypes.PRIVILEGE_DIALOG_CLOSE] = function (state, action) {

    return Object.assign({}, state, { operInfo: { operType: '' } });
}

privilegeReducerMap[actionTypes.PRIVILEGE_LIST_UPDATE] = function (state, action) {
    console.log("权限列表更新 reducer:", action.payload);
    return Object.assign({}, state, { privilegeList: action.payload });
}


export default handleActions(privilegeReducerMap, initState);