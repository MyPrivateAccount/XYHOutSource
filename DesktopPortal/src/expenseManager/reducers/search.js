import * as actionTypes from '../constants/actionType';
import {handleActions} from 'redux-actions'
const initState = {
    showLoading: true,
    navigator:[],
    keyWord: '',
    checkStatu: 0,//0 1 2
    chargePrice: 1,//
    orderRule: 0,//0不排 1升 2降
    pageIndex: 0,
    pageSize: 10,
    searchResult: {extension: [{key: '1', id: 'tt', createtime:"", createname: 'test', organize: 'hhee', ispayed: "否"}], pageIndex: 0, pageSize: 10, totalCount: 1},//搜索结果
    recieptInfoList: [],//后补发票用的
}
// {title: 'ID',dataIndex: 'id',key: 'id',},
// {title: '创建用户',dataIndex: 'createname',key: 'createname'},
// {title: '报销门店',dataIndex: 'organize',key: 'organize',},
// {title: "是否付款", dataIndex: "ispayed", key: "ispayed"},];

let reducerMap = {};

reducerMap[actionTypes.CHNAGE_MENU] = function(state, action) {
    return Object.assign(state, {actionMenu: action.payload, searchKeyWord:'', searchResult:[], navigator:[]});
}

reducerMap[actionTypes.SET_SEARCH_LOADING] = function(state, action) {
    return Object.assign(state, {showLoading:action.payload});
}

reducerMap[actionTypes.UPDATE_SEARCHCONDITION] = function(state, action) {
    if (action.payload.extension.length > 0) {
        action.payload.extension = action.payload.extension.map(function(v, i) {
            return {key: i, id: v.id, createtime: v.createTime?v.createTime.replace("T", " "):'', createname: v.createUserName, organize: v.department, ispayed: v.postTime == null ? "否":"是"};
        });
    }
    return Object.assign(state, {searchResult: action.payload, showLoading: false});
}

reducerMap[actionTypes.UPDATE_SEARCHCHECKSTATU] = function(state, action) {
    return Object.assign(state, {checkStatu: action.payload});
}

reducerMap[actionTypes.UPDATE_SEARCHCHARGEPRICE] = function(state, action) {
    return Object.assign(state, {chargePrice: action.payload});
}

reducerMap[actionTypes.UPDATE_RECIPTINFO] = function(state, action) {
    return Object.assign(state, {recieptInfoList: action.payload});
}

export default handleActions(reducerMap, initState);