import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

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
    return Object.assign({}, state, {permissionOrgTree: {AddUserTree: AddUserTree, AddNormalRoleTree: AddNormalRoleTree, AddPublicRoleTree: AddPublicRoleTree, AddRolePermissionTree: AddRolePermissionTree},operInfo:{operType:'org_update'}});
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

treeReducerMap[actionTypes.EMP_LIST_UPDATE] = function (state, action) {
    console.log("readucer用户的列表:" + JSON.stringify(action.payload.extension));
    return Object.assign({}, state, { empList: action.payload.extension });
}

export default handleActions(treeReducerMap, initState)