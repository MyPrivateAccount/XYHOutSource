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
    searchResult: {extension: [{key: '1', id: 'tt', chargetype: 'test', organize: 'hhee'}], pageIndex: 0, pageSize: 10, totalCount: 1},//搜索结果
}


let reducerMap = {};

reducerMap[actionTypes.CHNAGE_MENU] = function(state, action) {
    return Object.assign(state, {actionMenu: action.payload, searchKeyWord:'', searchResult:[], navigator:[]});
}

reducerMap[actionTypes.SET_SEARCH_LOADING] = function(state, action) {
    return Object.assign(state, {showLoading:action.payload});
}

reducerMap[actionTypes.UPDATE_SEARCHCONDITION] = function(state, action) {
    return Object.assign(state, {searchResult: action.payload, showLoading: false});
}

reducerMap[actionTypes.UPDATE_SEARCHCHECKSTATU] = function(state, action) {
    return Object.assign(state, {checkStatu: action.payload});
}

reducerMap[actionTypes.UPDATE_SEARCHCHARGEPRICE] = function(state, action) {
    return Object.assign(state, {chargePrice: action.payload});
}

export default handleActions(reducerMap, initState);