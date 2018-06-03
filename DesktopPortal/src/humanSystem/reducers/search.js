import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import moment from 'moment';

const initState = {
    attendanceList: {extension: [{key: "1", time: "tt", name: "tt", idcard: "tta", signed: "today"}], pageIndex: 0, pageSize: 10, totalCount: 1},
    achievementList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 1},
    stationList: [],//选中的部门职位
    blackList: {extension: [{key: '1', idcard: 'tt', name: 'test', reason: "tta"}], pageIndex: 0, pageSize: 10, totalCount: 1},//黑名单结果
    showLoading: false,
    showOrgSelect: false,//部门选择
    navigator: [],//导航记录
    activeOrg: {id: '0', organizationName: '不限'},//当前部门
    activeMenu: 'menu_index',//当前菜单
    keyWord: '',//搜索关键词
    searchCondition: {},//完整搜索条件
    humanType: 0,//0不限 1在职 2离职 3黑名单
    orderRule: 0,//0不排 1升 2降
    ageCondition: 0,//0不限 1 20以上 2 30以上 3 40以上
    expandSearchBox: true,
    pageIndex: 0,
    pageSize: 10,
    searchResult: {extension: [{key: '1', id: 'tt', username: 'test', idcard: 'hhee'}], pageIndex: 0, pageSize: 10, totalCount: 1},//搜索结果
};
let reducerMap = {};

//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, {showLoading: action.payload});
}

//个人所在部门数据获取完成
reducerMap[actionTypes.DIC_GET_ORG_DETAIL_COMPLETE] = function (state, action) {
    let activeOrg = {...state.activeOrg};
    if (action.payload.id) {
        activeOrg = action.payload;
    }
    return Object.assign({}, state, {activeOrg: activeOrg});
}
//打开部门选择
reducerMap[actionTypes.OPEN_ORG_SELECT] = function (state, action) {
    return Object.assign({}, state, {showOrgSelect: true});
}
//关闭部门选择
reducerMap[actionTypes.CLOSE_ORG_SELECT] = function (state, action) {
    return Object.assign({}, state, {showOrgSelect: false});
}


//切换部门
reducerMap[actionTypes.CHAGNE_ACTIVE_ORG] = function (state, action) {
    let activeOrg = {...state.activeOrg}
    if (action.payload) {
        activeOrg = action.payload;
    }
    return Object.assign({}, state, {
        activeOrg: activeOrg,
        navigator: [],
        searchResult: []
    });
}
//菜单切换
reducerMap[actionTypes.CHANGE_MENU] = function (state, action) {
    return Object.assign({}, state, {
        activeMenu: action.payload,
        searchKeyWord: '',
        auditList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},
        navigator: [],
        searchResult: [],
        showLoading: false,
        showOrgSelect: false,
        showAuditDetail: false
    });
}
//搜索关键字改变
reducerMap[actionTypes.CHANGE_KEYWORD] = function (state, action) {
    return Object.assign({}, state, {searchKeyWord: action.payload});
}
//搜索完成
reducerMap[actionTypes.SEARCH_CUSTOMER_COMPLETE] = function (state, action) {
    let list = action.payload.extension;
    let result = action.payload;
    if (!action.payload) {
        result = {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0};
    }
    result.extension.map(c => {
        if (c.createTime) {
            c.createTime = moment(c.createTime).format("YYYY-MM-DD HH:mm:ss");
        }
    });
    return Object.assign({}, state, {searchResult: result});
}

reducerMap[actionTypes.UPDATE_STATIONLIST] = function (state, action) {
    let f = action.payload.map(function(v, i) {
        return {key: i+"", stationname: v.positionName, isnew: false, positionType:v.positionType, id: v.id};
    });
    return Object.assign({}, state, {stationList: f, showLoading: false});
}

//保存查询条件
reducerMap[actionTypes.SAVE_SEARCH_CONDITION] = function (state, action) {
    return Object.assign({}, state, {searchCondition: action.payload});
}

reducerMap[actionTypes.SEARCH_BOX_EXPAND] = function(state, action) {
    return Object.assign({}, state, {expandSearchBox: !state.expandSearchBox});
}

reducerMap[actionTypes.SEARCH_HUMANTYPE] = function(state, action) {
    return Object.assign({}, state, {humanType:action.payload} );
}

reducerMap[actionTypes.SEARCH_AGETYPE] = function(state, action) {
    return Object.assign({}, state, {ageCondition:action.payload} );
}

reducerMap[actionTypes.SEARCH_ORDERTYPE] = function(state, action) {
    return Object.assign({}, state, {orderRule:action.payload} );
}

reducerMap[actionTypes.UPDATE_ALLHUMANINFO] = function(state, action) {
    return Object.assign({}, state,{searchResult: action.payload} );
}

reducerMap[actionTypes.UPDATE_BLACKLST] = function(state, action) {
    return Object.assign({}, state, {blackList: action.payload} );
}

reducerMap[actionTypes.UPDATE_SALARYINFO] = function(state, action) {
    return Object.assign({}, state, {achievementList: action.payload} );
}

export default handleActions(reducerMap, initState);