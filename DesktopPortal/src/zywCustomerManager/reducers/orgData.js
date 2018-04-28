import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const initState = {
    saleStatus: [],
    saleModel: [],
    shopsTypes: [],
    tradePlannings: [],//业态规划
    customerSource: [],//客户来源
    businessTypes: [],//商业类型
    customerLevels: [],//客户等级
    requirementLevels: [],//需求等级
    invalidResions: [],//失效原因
    followUpTypes: [],//跟进方式
    areaList: [],
    orgInfo: {orgList: [], levelCount: 0},
    userList: [],//部门用户
    sourceUserList: [],//调客业务员
    targetUserList: [],//收客业务员
    rateProgress: [],//商机阶段
    requirementType: [],
    treeData: []
};
let reducerMap = {};

//部门数据
reducerMap[actionTypes.DIC_GET_ORG_LIST_COMPLETE] = function (state, action) {
    let orgInfo = {...state.orgInfo};
    if (action.payload.extension) {
        action.payload.extension.map(org => {
            org.key = org.id;
            org.label = org.organizationName;
            org.value = org.id;
        });
        // console.log(action.payload.extension, "???/s")
        orgInfo = formatOrgData(action.payload.extension, action.payload.parentId, 0);
    }
    // console.log("格式化后的部门:", orgInfo.orgList);
    return Object.assign({}, state, {orgInfo: {orgList: orgInfo.orgList || [], levelCount: orgInfo.levelCount || 0}});
}
//部门用户获取完成
reducerMap[actionTypes.GET_ORG_USERLIST_COMPLETE] = function (state, action) {
    let sourceUserList = [...state.sourceUserList]; //调客业务员
    let targetUserList = [...state.targetUserList];//收客业务员
    let userList = [...state.userList];
    if (action.payload.type) {
        if (action.payload.type === "source") {
            sourceUserList = action.payload.extension || [];
        } else {
            targetUserList = action.payload.extension || [];
        }
    } else {
        userList = action.payload.extension || [];
    }
    return Object.assign({}, state, {userList: userList, targetUserList: targetUserList, sourceUserList: sourceUserList});
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

reducerMap[actionTypes.CHANGE_SOURCE_ORG] = function (state, action) {
    return Object.assign({}, state, {sourceUserList: []});
}
reducerMap[actionTypes.CHANGE_TARGET_ORG] = function (state, action) {
    return Object.assign({}, state, {targetUserList: []});
}


reducerMap[actionTypes.GET_TREE_LIST_COMPLETE] = function (state, action) {
    let treeData = []
    treeData = action.payload || []
    treeData.map((x, index) => {
        x.value = x.id
        x.label = x.businessName
        let arr = x.children||[]
        arr.map((child, idx) => {
            child.value = child.id
            child.label = child.businessName
            return child
        }) 
        return x
    })
    return Object.assign({}, state, {treeData: treeData});
}

export default handleActions(reducerMap, initState);