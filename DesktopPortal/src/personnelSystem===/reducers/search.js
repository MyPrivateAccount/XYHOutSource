import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import moment from 'moment';

const initState = {
    showLoading: false,
    showOrgSelect: false,//部门选择
    showAdjustCustomer: false,//调客
    showCustomerDetail: false,//客户详情
    navigator: [],//导航记录
    activeOrg: {id: '0', organizationName: '不限'},//当前部门
    activeMenu: 'menu_index',//当前菜单
    activeCustomers: [],//选中客户信息
    searchKeyWord: '',//搜索关键词
    searchCondition: {},//完整搜索条件
    searchResult: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},//搜索结果
    auditList: {extension: [], pageIndex: 0, pageSize: 10, totalCount: 0},//调客审核列表
    repeatJudgeInfo: {},//客户判重信息
    sourceCustomerList: [],
    targetCustomerList: [],
    showAuditDetail: false,//显示调客审核
    activeAuditHistory: {}//调客审核详细
};
let reducerMap = {};

//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, {showLoading: action.payload});
}

//打开详细页面
reducerMap[actionTypes.OPEN_CUSTOMER_DETAIL] = function (state, action) {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: navigator});
}
//关闭详细页面
reducerMap[actionTypes.CLOSE_CUSTOMER_DETAIL] = function (state, action) {
    return Object.assign({}, state, {navigator: [], showAuditDetail: false});
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

//打开调客对话框
reducerMap[actionTypes.OPEN_ADJUST_CUSTOMER] = function (state, action) {
    let activeCustomers = [...state.activeCustomers];
    if (action.payload) {
        activeCustomers = action.payload;
    }
    return Object.assign({}, state, {showAdjustCustomer: true, activeCustomers: activeCustomers});
}
//关闭调客对话框
reducerMap[actionTypes.CLOSE_ADJUST_CUSTOMER] = function (state, action) {
    return Object.assign({}, state, {showAdjustCustomer: false});
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
reducerMap[actionTypes.GET_CUSTOMER_DETAIL_COMPLETE] = function (state, action) {
    let activeCustomer = action.payload || {};
    if (activeCustomer.mainPhone) {
        activeCustomer.mainPhone = activeCustomer.mainPhone.replace(/([0-9]{3})[0-9]{3}([0-9]{4})/g, '$1***$2');
    }
    return Object.assign({}, state, {
        activeCustomers: [activeCustomer],
        navigator: [{id: action.payload.id, name: '客户详情', type: 'customerDetail'}]
    });
}
//保存查询条件
reducerMap[actionTypes.SAVE_SEARCH_CONDITION] = function (state, action) {
    return Object.assign({}, state, {searchCondition: action.payload});
}
//电话列表
reducerMap[actionTypes.GET_CUSTOMER_ALL_PHONE_COMPLETE] = function (state, action) {
    let activeCustomers = [...state.activeCustomers];
    activeCustomers[0].phoneList = action.payload || [];
    return Object.assign({}, state, {activeCustomers: activeCustomers});
}
//调客审核列表
reducerMap[actionTypes.GET_AUDIT_LIST_COMPLETE] = function (state, action) {
    let auditList = {...state.auditList};
    if (action.payload) {
        auditList = action.payload;
    }
    return Object.assign({}, state, {auditList: auditList});
}
//客户判重信息
reducerMap[actionTypes.GET_REPEAT_JUDGE_INFO_COMPLETE] = function (state, action) {
    return Object.assign({}, state, {repeatJudgeInfo: action.payload});
}
//客户列表
reducerMap[actionTypes.GET_CUSTOMER_OF_USERID_COMPLETE] = function (state, action) {
    let sourceCustomerList = [...state.sourceCustomerList];
    let targetCustomerList = [...state.targetCustomerList];
    if (action.payload.type === "source") {
        sourceCustomerList = action.payload.extension;
    } else {
        targetCustomerList = action.payload.extension;
    }
    return Object.assign({}, state, {sourceCustomerList: sourceCustomerList, targetCustomerList: targetCustomerList});
}
//打开调客审核详细
reducerMap[actionTypes.OPEN_CUSTOMER_AUDIT_INFO] = function (state, action) {
    return Object.assign({}, state, {showAuditDetail: true, navigator: [{id: action.payload.id, name: '审核详情', type: 'auditInfo'}]});
}
//获取当前审核记录的历史
reducerMap[actionTypes.GET_AUDIT_HISTORY_COMPLETE] = function (state, action) {
    return Object.assign({}, state, {activeAuditHistory: action.payload});
}
//移除调客请求中的指定客户
reducerMap[actionTypes.REMOVE_ADJUST_REQUEST_ITEM] = function (state, action) {
    let activeAuditHistory = {...state.activeAuditHistory};
    if (activeAuditHistory.content) {
        try {
            let jsonObj = JSON.parse(activeAuditHistory.content);
            jsonObj.customers = jsonObj.customers || [];
            for (let i = jsonObj.customers.length - 1; i > -1; i--) {
                if (jsonObj.customers[i].id === action.payload) {
                    jsonObj.customers.splice(i, 1);
                    break;
                }
            }
            activeAuditHistory.content = JSON.stringify(jsonObj);
        } catch (e) {}
    }
    return Object.assign({}, state, {activeAuditHistory: activeAuditHistory});
}
export default handleActions(reducerMap, initState);