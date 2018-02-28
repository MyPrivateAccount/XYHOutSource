import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';

const initState = {
    navigator: [],
    showLoading: false,
    unReadCount: 0,
    receiveList: { extension: [], pageIndex: 0, pageSize: 10, totalCount: 0 },
    msgDetail: {}
};
let treeReducerMap = {};
//设置遮罩层
treeReducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}
//未读数量
treeReducerMap[actionTypes.GET_UNREAD_COUNT_COMPLETE] = function (state, action) {
    console.log("未读数量:", action.payload);
    return Object.assign({}, state, { unReadCount: action.payload });
}
//接收列表
treeReducerMap[actionTypes.GET_RECEIVE_LIST_COMPLETE] = function (state, action) {
    let receiveList = { ...state.receiveList };
    if (action.payload) {
        receiveList = action.payload;
    }
    console.log("接收列表:", action.payload);
    return Object.assign({}, state, { receiveList: receiveList });
}
//消息详细加载完成
treeReducerMap[actionTypes.GET_SYS_MSG_DETAIL_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { msgDetail: action.payload || {} });
}


export default handleActions(treeReducerMap, initState);