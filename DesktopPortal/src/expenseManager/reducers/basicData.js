import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    limitHumanlst:[],
    selchargeList: [],
    departmentTree: [],
    chargeCostTypeList: [{value: 0, key: "test"}],
    navigator: [{menuID: 'menu_index', disname: '费用信息', type:'subMenu'}, {menuID: 'home', disname: '费用', type:'item'}]
};

let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let chargeCostTypeList = [...state.chargeCostTypeList];
    action.payload.map((group) => {
        if (group.groupId === "CHARGE_COST_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            chargeCostTypeList = group.dictionaryDefines;
        }
    })

    return Object.assign({}, state, {
        chargeCostTypeList: chargeCostTypeList,
    });
}

reducerMap[actionTypes.CLOSE_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: []});
}

reducerMap[actionTypes.SET_USER_BREADINDEX] = function(state, action) {
    return Object.assign({}, state, {navigator: state.navigator.slice(0, action.payload+1)});
}

reducerMap[actionTypes.SET_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: action.payload});
}

reducerMap[actionTypes.SET_USER_BREADADD] = function(state, action) {
    let bfind = false;
    for (const itm of state.navigator) {
        if (itm.menuID === action.payload.menuID) 
            bfind = true;
    }
    if (!bfind) {
        state.navigator.push(action.payload);
    }
    return Object.assign({}, state, {navigator: state.navigator.slice()});
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
function formatOrgData(originalData, parentId, levelCount) {
    let curLevelOrgs = [];
    let level = levelCount;
    if (originalData) {
        curLevelOrgs = originalData.filter(o => o.parentId === parentId);
        if (curLevelOrgs.length > 0) {
            level++;
            let levels = [];
            curLevelOrgs.map(o => {
                let result = formatOrgData(originalData, o.id, level);
                o.children = result.orgList;
                levels.push(result.levelCount);
            })
            level = Math.max.apply(null, levels);
        }
    }
    return {orgList: curLevelOrgs, levelCount: level};
}
reducerMap[actionTypes.DIC_GET_ALL_ORG_LIST_COMPLETE] = function(state, action) {
    let type = action.payload.type;

    let arrOrg = action.payload.extension;
    let formatNodeList = [];
    for (var i in action.payload.extension) {
        var node = action.payload.extension[i];
        var orgNode = {key: node.id, value: node.id, children: [], Original: node};
        orgNode.name = node.organizationName;
        orgNode.label = node.organizationName;
        orgNode.id = node.id;
        orgNode.organizationName = node.organizationName;
        orgNode.parentId = node.parentId;

        formatNodeList.push(orgNode);
    }
    let orgTreeSource = [];//顶层节点
    let curPeakNode = {}
    formatNodeList.map(node => {
        let parentID = node.Original.parentId;
        let result = formatNodeList.filter(n => n.key === parentID);
        if (result.length == 0) {//找不到父级的部门为顶级部门
            orgTreeSource.push(node);
            curPeakNode = node;
        }
    });
    
    orgTreeSource.map(node => {
        getAllChildrenNode(node, node.key, formatNodeList)
    });
    
    return Object.assign({}, state, {departmentTree: orgTreeSource});
}

reducerMap[actionTypes.SELECTCHARGE] = function(state, action) {
    return Object.assign({}, state, {selchargeList: action.payload});
}

reducerMap[actionTypes.CLEARCHARGE] = function(state, action) {
    return Object.assign({}, state, {selchargeList: []});
}

reducerMap[actionTypes.UPDATE_LIMITCHARGEHUMAN] = function(state, action) {
    return Object.assign({}, state, {limitHumanlst: action.payload});
}


export default handleActions(reducerMap,initState);