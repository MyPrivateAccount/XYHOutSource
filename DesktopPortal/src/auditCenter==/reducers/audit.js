import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    navigator: [],//导航记录
    activeMenu: {},//当前选中菜单
    myAudit: {//我审批的
        waitAuditList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },//待我审核
        auditedList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },//已审核列表
    },
    mySubmit: {//我提交的
        auditingList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },//通过列表
        auditedList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },//通过列表
        rejectedList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },//通过列表
    },
    copyToMe: {//抄送给我的
        allList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },
        noReadList: { extension: [], pageIndex: 1, pageSize: 10, totalCount: 0 },
    },
    activeAuditInfo: {},//当前激活审核
    activeAuditHistory: {},//当前审核历史
    noReadCount: 0,//知会未读总数
    buildingOfActiveInfo: {},//房源动态所属的楼盘
    buildingOfShops: []//房源动态所属的楼盘下的商铺列表
};
let reducerMap = {};

//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}

reducerMap[actionTypes.GET_AUDIT_LIST_COMPLETE] = function (state, action) {
    let myAudit = { ...state.myAudit };
    let mySubmit = { ...state.mySubmit };
    let copyToMe = { ...state.copyToMe };
    let result = action.payload;
    if (result.listType === "myAudit_wait") {
        myAudit.waitAuditList = result.extension;
    }
    else if (result.listType === "myAudit_audited") {
        myAudit.auditedList = result.extension;
    }
    else if (result.listType === "mySubmit") {
        if (result) {
            if (result.examineStatus[0] === 1) {
                mySubmit.auditingList = result.extension;
            } else if (result.examineStatus[0] === 2) {
                mySubmit.auditedList = result.extension;
            } else if (result.examineStatus[0] === 3) {
                mySubmit.rejectedList = result.extension;
            }
        }
    }
    else if (result.listType === "copyToMe") {
        if (result) {
            if (result.examineStatus.includes(1)) {
                copyToMe.noReadList = result.extension;
            } else {
                copyToMe.allList = result.extension;
            }
        }
    }
    return Object.assign({}, state, {
        myAudit: myAudit,
        mySubmit: mySubmit,
        copyToMe: copyToMe,
        navigator: []
    });
}
//打开审核详细页面
reducerMap[actionTypes.OPEN_AUDIT_DETAIL] = function (state, action) {
    let navigator = [action.payload];
    return Object.assign({}, state, { navigator: navigator, activeAuditInfo: action.payload });
}
//关闭审核详细页面
reducerMap[actionTypes.CLOSE_AUDIT_DETAIL] = function (state, action) {
    return Object.assign({}, state, { navigator: [] });
}
//菜单切换
reducerMap[actionTypes.CHANGE_MENU] = function (state, action) {
    return Object.assign({}, state, {
        navigator: [], activeAuditInfo: {},
        activeMenu: action.payload
    });
}
//获取当前审核记录的历史
reducerMap[actionTypes.GET_AUDIT_HISTORY_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { activeAuditHistory: action.payload });
}
//未读知会数量获取完成
reducerMap[actionTypes.GET_NO_READ_COUNT_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { noReadCount: action.payload });
}
//房源动态接口获取完成
reducerMap[actionTypes.GET_UPDATE_RECORD_DETAIL_COMPLETE] = function (state, action) {
    let activeAuditHistory = { ...state.activeAuditHistory, updateRecord: action.payload };
    return Object.assign({}, state, { activeAuditHistory: activeAuditHistory });
}

//楼盘详情获取完成
reducerMap[actionTypes.GET_BUILDING_DETAIL_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { buildingOfActiveInfo: action.payload || {} });
}
//楼盘商铺获取完成
reducerMap[actionTypes.GET_BUILDING_SHOPS_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { buildingOfShops: action.payload || [] });
}

export default handleActions(reducerMap, initState);