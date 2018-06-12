import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';
import { stat } from 'fs';

const initState = {
    selAttendanceList: [],
    selAchievementList: [],
    selBlacklist: [],//选中的黑名单列表
    selSalaryItem: {},
    selHumanList: [],
    searchOrgTree: [],
    stationTypeList: [],
    changeTypeList: [{value: 0, key: "tt"}],
    changeResonList: [{value: 0, key: "dd"}],
    humanImage:[],
    navigator: [{id: 20, menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts'}],//导航记录
    monthresult: {extension: [{key: '1', last: 'tt', monthtime: 'test', operater: 'hhee'}], pageIndex: 0, pageSize: 10, totalCount: 1},
    monthlast: '2018.5',
    headVisible: true,
};
let reducerMap = {};
//字典数据
reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let stationTypeList = [...state.stationTypeList];
    let changeTypeList = [...state.changeTypeList];
    let changeResonList = [...state.changeResonList];

    action.payload.map((group) => {
        if(group.groupId === "POSITION_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            stationTypeList = group.dictionaryDefines;
        } else if(group.groupId === "HUMAN_CHANGE_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            changeTypeList = group.dictionaryDefines;
        } else if(group.groupId === "HUMAN_CHANGEREASON_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            changeResonList = group.dictionaryDefines;
        }

    });
    return Object.assign({}, state, {
        stationTypeList: stationTypeList,
        changeTypeList: changeTypeList,
        changeResonList: changeResonList,
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

reducerMap[actionTypes.DIC_GET_ALL_ORG_LIST_COMPLETE] = function(state, action) {
    
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
    
    let levelCount = 0;

    return Object.assign({}, state, {searchOrgTree: orgTreeSource, levelCount: levelCount});
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

reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}

//{id: 20, menuID: "menu_user_mgr", displayName: "员工信息管理", menuIcon: 'contacts'},
reducerMap[actionTypes.SET_USER_BREADINDEX] = function(state, action) {
    switch (action.payload)
    {
        case 0: {
            state.navigator.push({id: action.payload, displayName: '入职', type: 'item'});
            return Object.assign({}, state, {navigator: state.navigator.slice()});
        } break;
        default: break;
    }
    return state;
}

reducerMap[actionTypes.SET_USER_BREADITEM] = function(state, action) {
    return Object.assign({}, state, {navigator: [action.payload]});
}

reducerMap[actionTypes.CLOSE_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: []});
}

reducerMap[actionTypes.MINUS_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: state.navigator.splice(state.navigator.length-1, 1)});
}

reducerMap[actionTypes.SET_USER_BREADITEMINDEX] = function(state, action) {
    return Object.assign({}, state, {navigator: state.navigator.slice(0, action.payload+1)});
}

reducerMap[actionTypes.ADD_USER_BREAD] = function(state, action) {
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

reducerMap[actionTypes.SET_HUMANINFONUMBER] = function(state, action) {
    let f = {...state.userinfo, worknumber: action.payload}
    return Object.assign({}, state, {userinfo: f});
}

reducerMap[actionTypes.MONTH_UPDATEMONTHLIST] = function(state, action) {
    return Object.assign({}, state, {monthresult: action.payload, showLoading: false, monthlast: action.payload.lastTime});
}

reducerMap[actionTypes.CHANGE_LOADING] = function(state, action) {
    return Object.assign({}, state, {showLoading: action.payload});
}

reducerMap[actionTypes.SEL_BLACKLIST] = function(state, action) {
    return Object.assign({}, state, {selBlacklist: action.payload});
}

reducerMap[actionTypes.SET_SELSALARYLIST] = function(state, action) {
    return Object.assign({}, state, {selAchievementList: action.payload});
}

reducerMap[actionTypes.UPDATE_STATIONTYPELIST] = function(state, action) {
    return Object.assign({}, state, {stationTypeList: action.payload});
}

reducerMap[actionTypes.UPDATE_SALARYINFO] = function(state, action) {
    return Object.assign({}, state, { showLoading: false} );
}

reducerMap[actionTypes.UPDATE_SALARYITEM] = function(state, action) {
    return Object.assign({}, state, {selSalaryItem: action.payload});
}

reducerMap[actionTypes.SET_SELHUMANINFO] = function(state, action) {
    return Object.assign({}, state, {selHumanList: action.payload});
}
reducerMap[actionTypes.UPDATE_HUMANIMAGE] = function(state, action) {
    return Object.assign({}, state, {humanImage: action.payload});
}

reducerMap[actionTypes.UPDATE_BLACKLST] = function(state, action) {
    return Object.assign({}, state, {showLoading: false} );
}
reducerMap[actionTypes.UPDATE_ALLHUMANINFO] = function(state, action) {
    return Object.assign({}, state, { headVisible: true} );
}
reducerMap[actionTypes.SET_VISIBLEHEAD] = function(state, action) {
    return Object.assign({}, state, { headVisible: action.payload.headVisible} );
}

export default handleActions(reducerMap, initState);