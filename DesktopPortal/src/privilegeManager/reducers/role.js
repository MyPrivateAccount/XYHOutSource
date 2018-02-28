import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import { saveOrgAsync } from '../../saga/sagas';
import { notification } from 'antd';

const initState = {
    activeRole: {},
    operInfo: { operType: '' },
    roleSource: [],
    privilegeTreeSource: [],//工具权限数据源
    checkPrivileges: { checkkeys: [], checkedPrivilegeItemNodes: [] },
};
let roleReducerMap = {};
//角色编辑
roleReducerMap[actionTypes.ROLE_EDIT] = function (state, action) {
    console.log("readucer角色编辑");
    return Object.assign({}, state, { activeRole: action.payload, operInfo: { operType: 'edit' } });
}
roleReducerMap[actionTypes.ROLE_ADD] = function (state, action) {
    console.log("readucer角色新增");
    return Object.assign({}, state, { operInfo: { operType: 'add' } });
}
roleReducerMap[actionTypes.ROLE_DIALOG_CLOSE] = function (state, action) {

    return Object.assign({}, state, { operInfo: { operType: '' } });
}

roleReducerMap[actionTypes.ROLE_LIST_UPDATE] = function (state, action) {
    console.log("readucer角色列表");
    return Object.assign({}, state, { roleSource: action.payload });
}
roleReducerMap[actionTypes.ROLE_SELECTED] = function (state, action) {
    console.log("reducer选择", action.payload);
    let activeRole = {};
    for (let i in state.roleSource) {
        if (state.roleSource[i].id == action.payload[0]) {
            activeRole = state.roleSource[i];
            break;
        }
    }
    return Object.assign({}, state, { activeRole: activeRole, operInfo: { operType: '' } });
}

//角色权限数据更新
roleReducerMap[actionTypes.ROLE_PRIVILEGE_UPDATE] = function (state, action) {
    console.log("readucer角色权限数据更新:" + JSON.stringify(action.payload));
    let selectedRolePrivileges = action.payload;
    let checkkeys = [], checkedPrivilegeItemNodes = [];
    for (let i in selectedRolePrivileges) {
        let app = selectedRolePrivileges[i];
        if (app.permissions) {
            app.permissions.map((item) => {
                checkkeys.push(item.permissionId);
                item.applicationId = app.applicationId;
                checkedPrivilegeItemNodes.push(item);
            });
        }
    }
    return Object.assign({}, state, { checkPrivileges: { checkkeys: checkkeys, checkedPrivilegeItemNodes: checkedPrivilegeItemNodes } });
}


//角色权限编辑
roleReducerMap[actionTypes.ROLE_PRIVILEGE_EDIT] = function (state, action) {

    return Object.assign({}, state, { checkPrivileges: action.payload });
}

//工具权限列表更新
roleReducerMap[actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_UPDATE] = function (state, action) {
    console.log("readucer工具权限列表更新：", action.payload);
    let privilegeTreeSource = state.privilegeTreeSource.slice();
    if (action.payload.type) {
        if (action.payload.type == 'app') {
            action.payload.extension.map((app, i) => {
                let privilege = privilegeTreeSource.find(p => p.key === app.id);
                if (!privilege) {
                    privilegeTreeSource.push({ key: app.id, name: app.displayName, children: [], original: { ...app, nodeType: 'app' } });
                }
            });
        } else {
            privilegeDataTransfer(privilegeTreeSource, action.payload.extension);
        }
    }
    return Object.assign({}, state, { privilegeTreeSource: privilegeTreeSource });
}

//单个工具权限数据转换为树形结构
function privilegeDataTransfer(privilegeTreeSource, privilegeList) {
    let groupList = [];
    if (privilegeList.length == 0) return;
    privilegeList.map((privilegeInfo, i) => {
        if (groupList.filter((g) => g.name == privilegeInfo.groups) == 0) {
            groupList.push({
                key: privilegeInfo.id + privilegeInfo.groups + i,
                name: privilegeInfo.groups,
                applicationId: privilegeInfo.applicationId,
                children: [],
                original: { groupName: privilegeInfo.groups, nodeType: 'groups' }
            });
        }
        for (let i in groupList) {
            let groupInfo = groupList[i];
            if (privilegeInfo.groups == groupInfo.name) {
                groupInfo.children.push({
                    key: privilegeInfo.id,
                    name: privilegeInfo.name,
                    isLeaf: true,
                    original: { ...privilegeInfo, nodeType: 'item' }
                });
                break;
            }
        }
    });
    for (let i in privilegeTreeSource) {
        let toolApp = privilegeTreeSource[i];
        if (groupList[0].applicationId == toolApp.key) {//一次只会查询一个app下面的权限列表
            toolApp.children = groupList;
            break;
        }
    }
}

export default handleActions(roleReducerMap, initState)