import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    ext:[],
    dataSource:[],
    searchCondition:{}
};
let finaReducerMap = {};

finaReducerMap[actionTypes.FINA_QUERYPPFT_SUCCESS] = function (state, action) {
    console.log("readucer人员分摊表查询成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERYPPFT_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_YFTCB_SUCCESS] = function (state, action) {
    console.log("readucer应发提成表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_YFTCB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_SFTCB_SUCCESS] = function (state, action) {
    console.log("readucer实发提成表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_SFTCB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_TCCBB_SUCCESS] = function (state, action) {
    console.log("readucer提成成本表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_TCCBB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_YFTCCJB_SUCCESS] = function (state, action) {
    console.log("readucer提成冲减表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_YFTCCJB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_LZRYYJQRB_SUCCESS] = function (state, action) {
    console.log("readucer离职人员业绩确认表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_LZRYYJQRB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_SFKJQRB_SUCCESS] = function (state, action) {
    console.log("readucer实发扣减确认表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_SFKJQRB_SUCCESS'}});
}
finaReducerMap[actionTypes.FINA_QUERY_FYXQB_SUCCESS] = function (state, action) {
    console.log("readucer分佣详情表" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERY_FYXQB_SUCCESS'}});
}
export default handleActions(finaReducerMap, initState)