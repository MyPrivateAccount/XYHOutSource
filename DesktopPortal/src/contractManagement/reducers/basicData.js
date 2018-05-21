import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const initState = {
    contractAttachTypes:[],//合同附件分类（字典）
    contractCategories:[],//合同类型分类（字典）
    firstPartyCatogories:[],//甲方类型
    commissionCatogories:[],
    contractProjectCatogories:[],
    settleAccountsCatogories:[],
    orgInfo: {orgList: [], levelCount: 0},
    permissionOrgTree:{
        searchOrgTree:[],
        searchOrgList:[],
        setContractOrgTree:[],
        levelCount: 0,
    },




    requirementType: []
};
let reducerMap = {};
//字典数据
reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {

    let contractAttachTypes = [...state.contractAttachTypes];
    let contractCategories = [...state.contractCategories];
    let firstPartyCatogories = [...state.firstPartyCatogories];
    let commissionCatogories = [...state.commissionCatogories];
    let contractProjectCatogories = [...state.contractProjectCatogories];
    let contractSettleAccountTypes = [...state.settleAccountsCatogories];
    
    console.log('字典数据：', action.payload);
    action.payload.map((group) => {
        if(group.groupId === 'CONTRACT_ATTACHMENT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractAttachTypes = group.dictionaryDefines;
     
        }
        else if(group.groupId === 'CONTRACT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractCategories = group.dictionaryDefines;

        }
        else if(group.groupId === 'FIRST_PARTT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            firstPartyCatogories = group.dictionaryDefines;

        }
        else if(group.groupId === 'COMMISSION_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            commissionCatogories = group.dictionaryDefines;

        }
        else if(group.groupId === "CONTRACT_SETTLEACCOUNTS"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractSettleAccountTypes = group.dictionaryDefines;
        }
        else if(group.groupId === "CONTRACT_PROJECT_CATEGORIES"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractProjectCatogories = group.dictionaryDefines;
        }
        
  

    });
    return Object.assign({}, state, {
        contractAttachTypes:contractAttachTypes,
        contractCategories:contractCategories,
        firstPartyCatogories:firstPartyCatogories,
        commissionCatogories:commissionCatogories,
        contractProjectCatogories:contractProjectCatogories,
        settleAccountsCatogories:contractSettleAccountTypes,
    });
}

//部门数据
reducerMap[actionTypes.DIC_GET_ORG_LIST_COMPLETE] = function (state, action) {
    let orgInfo = {...state.orgInfo};
    if (action.payload.extension) {
        action.payload.extension.map(org => {
            org.key = org.id;
            org.label = org.organizationName;
            org.value = org.id;
        });
        console.log(action.payload.extension, "???/s")
        orgInfo = formatOrgData(action.payload.extension, action.payload.parentId, 0);
    }
    console.log("格式化后的部门:", orgInfo.orgList);
    return Object.assign({}, state, {orgInfo: {orgList: orgInfo.orgList || [], levelCount: orgInfo.levelCount || 0}});
}


reducerMap[actionTypes.DIC_GET_ALL_ORG_LIST_COMPLETE] = function(state, action){
    // let searchOrgTree = action.payload.extension || [];
    // return Object.assign({}, state, {permissionOrgTree: {searchOrgTree: searchOrgTree,}});
    let type = action.payload.type;

    let searchOrgTree = state.permissionOrgTree.searchOrgTree;
    let setContractOrgTree = state.permissionOrgTree.setContractOrgTree;
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
    //let curPeakNode = {}
    formatNodeList.map(node => {
        let parentID = node.Original.parentId;
        let result = formatNodeList.filter(n => n.key === parentID);
        if (result.length == 0) {//找不到父级的部门为顶级部门
            orgTreeSource.push(node);
            //curPeakNode = node;
        }
    });
    
    orgTreeSource.map(node => {
        getAllChildrenNode(node, node.key, formatNodeList)
    });
    
    let levelCount = 0;

    if(type === 'ContractSearchOrg'){

        searchOrgTree = orgTreeSource;
        //let info = formatOrgData(arrOrg, curPeakNode.parentId, 0);
    
        levelCount = getTreeDeepth(searchOrgTree, 0);
      
    }else if(type === 'ContractSetOrg'){
        setContractOrgTree = orgTreeSource;
        levelCount = state.permissionOrgTree.levelCount;
    }
    return Object.assign({}, state, {permissionOrgTree: {searchOrgTree: searchOrgTree, searchOrgList:arrOrg|| [], setContractOrgTree: setContractOrgTree, levelCount: levelCount}});

}

function getTreeDeepth(orgTreeSource, curLevel){
    if(orgTreeSource && orgTreeSource.length > 0){
        curLevel += 1;
      }
      else{
          return curLevel;
      }

      let curll = curLevel;
      for(let i =0; i < orgTreeSource.length; i ++){
        let ch = orgTreeSource[i].children
        let cnt = getTreeDeepth(ch, curLevel);
        if(cnt > curll){
            curll = cnt;
        }
      }
    
      return curll;
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
        // console.log(curLevelOrgs, '00')
        if (curLevelOrgs.length > 0) {
            level++;
            let levels = [];
            curLevelOrgs.map(o => {
                let result = formatOrgData(originalData, o.id, level);
                o.children = result.orgList;
                levels.push(result.levelCount);
            })
            // console.log(curLevelOrgs, '12')
            level = Math.max.apply(null, levels);
        }
    }
    return {orgList: curLevelOrgs, levelCount: level};
}


export default handleActions(reducerMap, initState);