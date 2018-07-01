//月结saga
import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取月结月份接口
export function* getMonthlyDataAsyncs(state){
    let result = { isOk: false, msg: '获取月结月份数据成功!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+state.payload.branchId;
    try {
        console.log(url)
        console.log('getMonthlyDataAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_GETUPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取月结月份数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取月结月份数据失败!'+result.msg,
            duration: 3
        });
    }
}
//开始月结接口
export function* startYjAsyncs(state){
    let result = { isOk: false, msg: '开始月结成功!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('startYjAsyncs:', state);
        let res = yield call(ApiClient.post, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_START_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "开始月结接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '开始月结失败!'+result.msg,
            duration: 3
        });
    }
}
//月结进度检查接口
export function* checkYjAsyncs(state){
    let result = { isOk: false, msg: '月结进度检查!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'progress/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('checkYjAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_CHECK_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "月结进度检查接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '月结进度检查失败!'+result.msg,
            duration: 3
        });
    }
}
//取消月结
export function* cancelYjAsyncs(state){
    let result = { isOk: false, msg: '取消月结!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'cancel/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('cancelYjAsyncs:', state);
        let res = yield call(ApiClient.post, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_CANCEL_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "取消月结接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '取消月结失败!'+result.msg,
            duration: 3
        });
    }
}
//离职人员业绩确认查询
export function* yjQrQueryAsyncs(state){
    let result = { isOk: false, msg: '离职人员业绩确认查询!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'yjqr/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('yjQrQueryAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_YJQR_QUERY_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "离职人员业绩确认查询接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '离职人员业绩确认查询失败!'+result.msg,
            duration: 3
        });
    }
}
//离职人员业绩确认
export function* yjQrCommitAsyncs(state){
    let result = { isOk: false, msg: '离职人员业绩确认提交!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'yjqr/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('yjQrCommitAsyncs:', state);
        let res = yield call(ApiClient.post, url,state.payload.emps);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_YJQR_COMMIT_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "离职人员业绩确认提交接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '离职人员业绩确认提交失败!'+result.msg,
            duration: 3
        });
    }
}
////////////////////////////////////////////////////////////////////////////////////////////
//实扣查询
export function* yjSKQueryAsyncs(state){
    let result = { isOk: false, msg: '实扣查询!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'skqr/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('yjSKQueryAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_SKQR_QUERY_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "实扣查询接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '实扣查询失败!'+result.msg,
            duration: 3
        });
    }
}
//实扣提交
export function* yjSKCommitAsyncs(state){
    let result = { isOk: false, msg: '实扣提交!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'skqr/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('yjSKCommitAsyncs:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_SKQR_COMMIT_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "实扣提交接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '实扣提交失败!'+result.msg,
            duration: 3
        });
    }
}
//月结回滚
export function* yjRollBackAsyncs(state){
    let result = { isOk: false, msg: '月结回滚!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+'rollback/'+state.payload.branchId+'/'+state.payload.yyyymm;
    try {
        console.log(url)
        console.log('yjRollBackAsyncs:', state);
        let res = yield call(ApiClient.post, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_ROLLBACK_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "月结回滚接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '月结回滚失败!'+result.msg,
            duration: 3
        });
    }
}
export default function* watchAllYjAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_GET), getMonthlyDataAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_START), startYjAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_CHECK), checkYjAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_CANCEL), cancelYjAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_YJQR_QUERY), yjQrQueryAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_YJQR_COMMIT), yjQrCommitAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_SKQR_QUERY), yjSKQueryAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_SKQR_COMMIT), yjSKCommitAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_ROLLBACK), yjRollBackAsyncs);
}