import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import moment from 'moment';

const initState = {
    showLoading: false,
    showOrgSelect: false,//部门选择
    navigator: [],//导航记录
    activeOrg: {id: '0', organizationName: '不限'},//当前部门
    activeMenu: 'menu_index',//当前菜单
    searchKeyWord: '',//搜索关键词
    searchCondition: {},//完整搜索条件
    searchHumanType: 0,
    searchSortType: 0,//0不排 1升 2降
    agesCondition: 0,
    searchResult: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},//搜索结果
    expandSearchBox: true
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
//加载客户详情完成
// reducerMap[actionTypes.GET_CUSTOMER_DETAIL_COMPLETE] = function (state, action) {
//     let activeCustomer = action.payload || {};
//     if (activeCustomer.mainPhone) {
//         activeCustomer.mainPhone = activeCustomer.mainPhone.replace(/([0-9]{3})[0-9]{3}([0-9]{4})/g, '$1***$2');
//     }
//     return Object.assign({}, state, {
//         activeCustomers: [activeCustomer],
//         navigator: [{id: action.payload.id, name: '客户详情', type: 'customerDetail'}]
//     });
// }
//保存查询条件
reducerMap[actionTypes.SAVE_SEARCH_CONDITION] = function (state, action) {
    return Object.assign({}, state, {searchCondition: action.payload});
}

reducerMap[actionTypes.SEARCH_BOX_EXPAND] = function(state, action) {
    return Object.assign({}, state, {expandSearchBox: !state.expandSearchBox});
}

export default handleActions(reducerMap, initState);