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
    requirementType: []
};
let reducerMap = {};
//字典数据
reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let saleStatus = [...state.saleStatus], saleModel = [...state.saleModel], shopsTypes = [...state.shopsTypes], tradePlannings = [...state.tradePlannings];
    let customerSource = [...state.customerSource], businessTypes = [...state.businessTypes], customerLevels = [...state.customerLevels];
    let requirementLevels = [...state.requirementLevels], requirementType = [...state.requirementType];
    let invalidResions = [...state.invalidResions], followUpTypes = [...state.followUpTypes], rateProgress = [...state.rateProgress];
    action.payload.map((group) => {
        if (group.groupId === "CUSTOMER_SOURCE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            customerSource = group.dictionaryDefines;
        }
        else if (group.groupId === "BUSINESS_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            businessTypes = group.dictionaryDefines;
        }
        else if (group.groupId === "PROJECT_SALE_STATUS") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            saleStatus = group.dictionaryDefines;
        }
        else if (group.groupId === "SALE_MODE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            saleModel = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_CATEGORY") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            shopsTypes = group.dictionaryDefines;
        }
        else if (group.groupId === "TRADE_MIXPLANNING") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            tradePlannings = group.dictionaryDefines;
        }
        else if (group.groupId === "CUSTOMER_LEVEL") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            customerLevels = group.dictionaryDefines;
        }
        else if (group.groupId === "REQUIREMENT_LEVEL") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            requirementLevels = group.dictionaryDefines;
        }
        else if (group.groupId === "INVALID_REASON") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            invalidResions = group.dictionaryDefines;
        }
        else if (group.groupId === "FOLLOWUP_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            followUpTypes = group.dictionaryDefines;
        }
        else if (group.groupId === "RATE_PROGRESS") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            rateProgress = group.dictionaryDefines;
        }
        else if (group.groupId === "REQUIREMENT_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            requirementType = group.dictionaryDefines;
        }
    });
    return Object.assign({}, state, {
        customerSource: customerSource,
        businessTypes: businessTypes,
        customerLevels: customerLevels,
        requirementLevels: requirementLevels,
        invalidResions: invalidResions,
        followUpTypes: followUpTypes, rateProgress: rateProgress, requirementType: requirementType,
        saleStatus: saleStatus, saleModel: saleModel, shopsTypes: shopsTypes, tradePlannings: tradePlannings
    });
}
//区域数据
reducerMap[actionTypes.DIC_GET_AREA_COMPLETE] = function (state, action) {
    let allAreas = action.payload;
    let areaList = [];
    let firstLevelAreas = allAreas.filter((area) => area.level === "1");
    let secondLevelAreas = allAreas.filter((area) => area.level === "2");
    let thirdLevelAreas = allAreas.filter((area) => area.level === "3");
    firstLevelAreas.map((firstChild) => {
        let firstLevelNode = {value: firstChild.code, label: firstChild.name, children: []};
        //二级地区
        let secondChilds = secondLevelAreas.filter((child) => child.parentId === firstChild.code);
        secondChilds.map((secondChild) => {
            let secondLevelNode = {value: secondChild.code, label: secondChild.name, children: []};
            //三级地区
            let thirdChilds = thirdLevelAreas.filter((child) => child.parentId == secondChild.code);
            thirdChilds.map((thirdChild) => {
                secondLevelNode.children.push({value: thirdChild.code, label: thirdChild.name});
            });
            firstLevelNode.children.push(secondLevelNode);
        });
        areaList.push(firstLevelNode);
    });
    //console.log("地区json：", areaList);
    return Object.assign({}, state, {areaList: areaList});
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
export default handleActions(reducerMap, initState);