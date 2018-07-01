import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    result:[],
    startYjResult:[],
    yjqrResult:[],
    yjqrCommitResult:[],
    yjskResult:[],
    yjskCommitResult:[],
    yjRollBackResult:[],
    emps:[]

};
let monthReducerMap = {};

monthReducerMap[actionTypes.YJ_MONTH_GETUPDATE] = function (state, action) {
    console.log("readucer月结月份查询接口返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_GETUPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_START_UPDATE] = function (state, action) {
    console.log("readucer开始月结返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { startYjResult:action.payload ,operInfo:{operType:'YJ_MONTH_START_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_CHECK_UPDATE] = function (state, action) {
    console.log("readucer开始月结返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_CHECK_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_YJQR_QUERY_UPDATE] = function (state, action) {
    console.log("readucer业绩确认返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { yjqrResult:action.payload ,operInfo:{operType:'YJ_MONTH_YJQR_QUERY_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_YJQR_COMMIT_UPDATE] = function (state, action) {
    console.log("readucer业绩确认提交返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_YJQR_COMMIT_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_SKQR_QUERY_UPDATE] = function (state, action) {
    console.log("readucer实扣列表返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { yjskResult:action.payload ,operInfo:{operType:'YJ_MONTH_SKQR_QUERY_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_SKQR_COMMIT_UPDATE] = function (state, action) {
    console.log("readucer实扣提交返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_SKQR_COMMIT_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_ROLLBACK_UPDATE] = function (state, action) {
    console.log("readucer月结回滚返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_ROLLBACK_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_CANCEL_UPDATE] = function (state, action) {
    console.log("readucer月结取消返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_CANCEL_UPDATE'}});
}
monthReducerMap[actionTypes.YJ_MONTH_EMP] = function (state, action) {
    console.log("readucer月结获取业绩确认员工" + JSON.stringify(action.payload));
    return Object.assign({}, state, { emps:action.payload ,operInfo:{operType:'YJ_MONTH_EMP'}});
}
monthReducerMap[actionTypes.YJ_MONTH_SKQR_EMP] = function (state, action) {
    console.log("readucer月结获取实扣员工" + JSON.stringify(action.payload));
    return Object.assign({}, state, { emps:action.payload ,operInfo:{operType:'YJ_MONTH_SKQR_EMP'}});
}
export default handleActions(monthReducerMap, initState)