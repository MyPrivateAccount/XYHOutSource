import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import {saveOrgAsync} from '../../saga/sagas';
import {notification} from 'antd';

const initState = {
    activeTreeNode: {},
    //selectedKeys: [],
    treeSource: [],
    operInfo: {objType: '', operType: '', snackbar: ''},
    empList: [],
    permissionOrgTree: {
        AddUserTree: [],//添加用户时
        AddNormalRoleTree: [],//添加普通角色时
        AddPublicRoleTree: [],//添加公共角色时
        AddRolePermissionTree: []//角色授权时
    },
    areaList: []
};
let treeReducerMap = {};

treeReducerMap[actionTypes.ORG_NODE_ADD] = function (state, action) {
    console.log("readucer部门新增" + JSON.stringify(action.payload));
    return Object.assign({}, state, {operInfo: {objType: 'org', operType: 'add', dialogOpen: true}});
}
treeReducerMap[actionTypes.ORG_NODE_EDIT] = function (state, action) {
    console.log("readucer部门编辑" + JSON.stringify(action.payload));
    return Object.assign({}, state, {operInfo: {objType: 'org', operType: 'edit', dialogOpen: true}});
}

treeReducerMap[actionTypes.ORG_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, {operInfo: {objType: '', operType: '', dialogOpen: false}});
}

treeReducerMap[actionTypes.ORG_NODE_SELECTED] = function (state, action) {
    console.log("readucer节点选择", action.payload);
    let activeNode = getActiveNode(action.payload, state.treeSource) || {};
    console.log("对应的activeTreeNode：", activeNode);
    return Object.assign({}, state, {activeTreeNode: activeNode, selectedKeys: action.payload});
}


//树源数据获取
treeReducerMap[actionTypes.ORG_DATA_UPDATE] = function (state, action) {
    console.log("readucer:数据源更新：", action.payload.nodeList);
    let activeTreeNode = {...state.activeTreeNode};
    let {type} = action.payload;
    var source = state.treeSource.slice();
    if (type == 'add') {
        var nodeList = TreeSourceTransfer(action.payload.nodeList, action.payload.parentID);
        AddNodeListToSource(source, action.payload.parentID, nodeList);
    }
    else if (type == 'delete') {
        let {removeId} = action.payload;
        RemoveNodeFromeSource(source, removeId);
    }
    if (Object.keys(state.activeTreeNode).length > 0) {
        activeTreeNode = getActiveNode(activeTreeNode.id, state.treeSource) || {};
    }
    return Object.assign({}, state, {treeSource: source, activeTreeNode: activeTreeNode});
}


//数据源转换
function TreeSourceTransfer(source, parentID) {
    var nodeList = [];
    for (var i in source) {
        var node = source[i];
        // {id: '1', name: '新耀行', type: 'org', expand: true, hasChild: true, childs:[]}
        var orgNode = {key: node.id, name: node.organizationName, children: [], Original: node};
        nodeList.push(orgNode);
    }
    return nodeList;
}
//子节点添加
function AddNodeListToSource(source, parentID, nodeList) {
    if (source == null || source.length == 0) {
        nodeList.map((node, i) => {source.push(node);});
        return;
    }
    for (var i in source) {
        if (source[i].key == parentID) {
            Object.assign(source[i].children, nodeList);
            return;
        }
        if (source[i].children.length > 0) {
            AddNodeListToSource(source[i].children, parentID, nodeList);
        }
    }
}
//移除节点
function RemoveNodeFromeSource(source, removeId) {
    if (source == null || source.length == 0) {
        return;
    }
    for (let i = source.length - 1; i > -1; i--) {
        let nodeInfo = source[i];
        if (nodeInfo.key == removeId) {
            source.splice(i, 1);
            break;
        } else {
            RemoveNodeFromeSource(nodeInfo.children, removeId);
        }
    }
}

function getActiveNode(selectedID, treeSource) {
    let orgInfo;
    for (let i in treeSource) {
        let org = treeSource[i];
        if (org.key == selectedID) {
            orgInfo = org.Original;
        } else {
            orgInfo = getActiveNode(selectedID, org.children);
        }
        if (orgInfo) {
            break;
        }
    }
    return orgInfo;
}
//根据权限获取部门树
treeReducerMap[actionTypes.ORG_GET_PERMISSION_TREE_UPDATE] = function (state, action) {
    let AddUserTree = state.permissionOrgTree.AddUserTree, AddNormalRoleTree = state.permissionOrgTree.AddNormalRoleTree;
    let AddPublicRoleTree = state.permissionOrgTree.AddPublicRoleTree, AddRolePermissionTree = state.permissionOrgTree.AddRolePermissionTree;
    let formatNodeList = [];
    for (var i in action.payload.extension) {
        var node = action.payload.extension[i];
        var orgNode = {key: node.id, value: node.id, children: [], Original: node};
        if (action.payload.permissionType === "AuthorizationPermission") {
            orgNode.name = node.organizationName;
        } else {
            orgNode.label = node.organizationName;
        }
        formatNodeList.push(orgNode);
    }
    let orgTreeSource = [];//顶层节点
    formatNodeList.map(node => {
        let parentID = node.Original.parentId;
        let result = formatNodeList.filter(n => n.key === parentID);
        if (result.length == 0) {//找不到父级的部门为顶级部门
            orgTreeSource.push(node);
        }
    });
    orgTreeSource.map(node => {
        getAllChildrenNode(node, node.key, formatNodeList)
    });
    console.log("顶级部门：", orgTreeSource);
    if (action.payload.permissionType === "UserInfoCreate") {
        AddUserTree = orgTreeSource;

    } else if (action.payload.permissionType === "RoleCreate") {
        AddNormalRoleTree = orgTreeSource;
    }
    else if (action.payload.permissionType === "PublicRoleOper") {
        AddPublicRoleTree = orgTreeSource;
    }
    else if (action.payload.permissionType === "AuthorizationPermission") {
        AddRolePermissionTree = orgTreeSource;
    }
    return Object.assign({}, state, {permissionOrgTree: {AddUserTree: AddUserTree, AddNormalRoleTree: AddNormalRoleTree, AddPublicRoleTree: AddPublicRoleTree, AddRolePermissionTree: AddRolePermissionTree}});
}

function getAllChildrenNode(node, parentId, formatNodeLit) {
    let nodeList = formatNodeLit.filter(n => n.Original.parentId === parentId);
    if (nodeList.length === 0) {
        return [];
    }
    nodeList.map(n => {
        n.children = getAllChildrenNode(n, n.key, formatNodeLit);
    });
    node.children = nodeList;
    return nodeList;
}

treeReducerMap[actionTypes.DIC_GET_AREA_COMPLETE] = function (state, action) {
    let allAreas = action.payload;
    let areaList = [];
    let firstLevelAreas = allAreas.filter((area) => area.level === "1");
    firstLevelAreas.map((city) => {
        let cityNode = {value: city.code, label: city.name, children: []};
        areaList.push(cityNode);
    });
    console.log("城市列表：", areaList);
    return Object.assign({}, state, {areaList: areaList});
}

export default handleActions(treeReducerMap, initState)