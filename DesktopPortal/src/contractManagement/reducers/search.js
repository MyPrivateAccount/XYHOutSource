import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import moment from 'moment';

const initState = {
    showLoading: false,
    showContractRecord: false,//合同录入
    showContractInfo:false,//合同详情
    showAttachMent: false,//附件上传
    showComplement: false,
    navigator: [],//导航记录

    activeOrg: {id: '0', organizationName: '不限'},//当前部门
    activeMenu: 'menu_index',//当前菜单
    showContractShow:false,

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

//打开合同附件上传页面
reducerMap[actionTypes.OPEN_ATTACHMENT_START] = function (state, action) {

    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [{id: action.payload.id, name: '附件上传', type: 'attachMent'}], showAttachMent: true});
}

//关闭合同附件上传页面
reducerMap[actionTypes.CLOSE_ATTACHMENT] = function (state, action) {
    return Object.assign({}, state, {navigator: [], showAttachMent: false});
}

reducerMap[actionTypes.UPLOAD_ATTCHMENT_LIST_COMPLETE] = function(state, action){
    return Object.assign({}, state, {navigator: [], showAttachMent: false});
}
//打开合同录入页面
reducerMap[actionTypes.OPEN_RECORD_NAVIGATOR] = function (state, action) {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [{id: action.payload.id, name: '录入', type: 'record'}], showContractRecord: true});
}
//关闭合同录入完成页面
reducerMap[actionTypes.CLOSE_RECORD] = function (state, action) {
    return Object.assign({}, state, {navigator: [], showContractRecord: false});
}

// 打开合同详情
reducerMap[actionTypes.OPEN_CONTRACT_DETAIL] = (state, action) => {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [{id: action.payload.id, name: '合同详情页', type: 'contractDetail'}], showContractInfo: true});
}

// 关闭合同详情
reducerMap[actionTypes.CLOSE_CONTRACT_DETAIL] = (state, action) => {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [], showContractInfo: false});
}

// 打开补充协议
reducerMap[actionTypes.OPEN_COMPLEMENT_START] = (state, action) => {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [{id: action.payload.id, name: '补充协议', type: 'complement'}], showComplement: true});
}

// 关闭补充协议
reducerMap[actionTypes.CLOSE_COMPLEMENT] = (state, action) => {
    let navigator = [action.payload];
    return Object.assign({}, state, {navigator: [], showComplement: false});
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
        //showAuditDetail: false
    });
}
//搜索关键字改变
reducerMap[actionTypes.CHANGE_KEYWORD] = function (state, action) {
    return Object.assign({}, state, {searchKeyWord: action.payload});
}
//搜索完成
reducerMap[actionTypes.SEARCH_COMPLETE] = function (state, action) {
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

//保存查询条件
reducerMap[actionTypes.SAVE_SEARCH_CONDITION] = function (state, action) {
    return Object.assign({}, state, {searchCondition: action.payload});
}



export default handleActions(reducerMap, initState);