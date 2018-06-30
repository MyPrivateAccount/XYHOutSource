import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import { sagaMiddleware } from '../..';

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
        AddRolePermissionTree: [],//角色授权时
        BaseSetOrgTree:[],
        RSFTZZOrgTree:[],//人数分摊组织页面的树
        YJOrgTree:[],//月结页面的组织树
    },
    areaList: [],
    humanList:[]
};
let treeReducerMap = {};

//根据权限获取部门树
treeReducerMap[actionTypes.ORG_GET_PERMISSION_TREE_UPDATE] = function (state, action) {
    let AddUserTree = state.permissionOrgTree.AddUserTree, AddNormalRoleTree = state.permissionOrgTree.AddNormalRoleTree;
    let AddPublicRoleTree = state.permissionOrgTree.AddPublicRoleTree, AddRolePermissionTree = state.permissionOrgTree.AddRolePermissionTree;
    let BaseSetOrgTree = state.permissionOrgTree.BaseSetOrgTree
    let RSFTZZOrgTree = state.permissionOrgTree.RSFTZZOrgTree
    let YJOrgTree = state.permissionOrgTree.YJOrgTree
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
    else if(action.payload.permissionType === "YJ_YJFTSZ_CK"||
            action.payload.permissionType === "YJ_RSFTZZSZ_CK"||
            action.payload.permissionType === "YJ_TCBLSZ"||
            action.payload.permissionType === "YJ_ZZCSSZ_CK"){
        BaseSetOrgTree = getContractOrg(orgTreeSource,null)
    }
    else if(action.payload.permissionType === "YJ_SZ_KXFTZZ"){
        RSFTZZOrgTree = orgTreeSource
    }
    else if(action.payload.permissionType === "YJ_CW_YJ"){
        YJOrgTree  = getContractOrg(orgTreeSource,null)
    }
    return Object.assign({}, state, {permissionOrgTree: {AddUserTree: AddUserTree, AddNormalRoleTree: AddNormalRoleTree, AddPublicRoleTree: AddPublicRoleTree, AddRolePermissionTree: AddRolePermissionTree,BaseSetOrgTree:BaseSetOrgTree,RSFTZZOrgTree:RSFTZZOrgTree,YJOrgTree:YJOrgTree},operInfo:{operType:action.payload.permissionType}});
}
function getContractOrg(orgTree,sArray) {
    if(sArray === null){
        sArray = []
    }
    if (orgTree && orgTree.length > 0) {
        for (let i = 0; i < orgTree.length; i++) {
            if (orgTree[i].Original.type == 'Subsidiary') {
                sArray.push(orgTree[i])
                if(orgTree[i].children.length>0){
                    let children = []//拷贝副本
                    for(let k=0;k<orgTree[i].children.length;k++){
                        children.push(orgTree[i].children[k])
                    }
                    orgTree[i].children = []
                    getContractOrg(children,orgTree[i].children)//递归找出子级是否符合条件
                }
            }
            else if(orgTree[i].Original.type == 'Filiale'){
                sArray.push(orgTree[i])
                if(orgTree[i].children.length>0){
                    let children = []//拷贝副本
                    for(let k=0;k<orgTree[i].children.length;k++){
                        children.push(orgTree[i].children[k])
                    }
                    orgTree[i].children = []
                    getContractOrg(children,orgTree[i].children)//递归找出子级是否符合条件
                }
            }
            else if(orgTree[i].children.length>0){
                getContractOrg(orgTree[i].children,sArray)//递归找出子级是否符合条件
            }
        }
    }
    return sArray;
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
    return Object.assign({}, state, { empList: action.payload.extension,operInfo:{operType:'EMP_LIST_UPDATE'}});
}
treeReducerMap[actionTypes.SEARCH_HUMAN_INFO_SUCCESS] = function (state, action) {
    console.log("readucer员工列表:" + JSON.stringify(action.payload.extension));
    return Object.assign({}, state, { humanList: action.payload.extension ,operInfo:{operType:'SEARCH_HUMAN_INFO_SUCCESS'}});
}

export default handleActions(treeReducerMap, initState)