import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//人员分摊表
export function* searchPPFtDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询人员分摊表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchPPFtDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchPPFtDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERYPPFT_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询人员分摊表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询人员分摊表信息失败!',
            duration: 3
        });
    }
}
//应发提成表
export function* searchYftcbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询应发提成表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchYftcbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchYftcbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_YFTCB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询应发提成表信息信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询应发提成表信息信息失败!',
            duration: 3
        });
    }
}
//实发提成表
export function* searchSftcbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询实发提成表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchSftcbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchSftcbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_SFTCB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询实发提成表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询实发提成表信息失败!',
            duration: 3
        });
    }
}
//提成成本表
export function* searchTccbbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询提成成本表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchTccbbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchTccbbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_TCCBB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询提成成本表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询提成成本表信息失败!',
            duration: 3
        });
    }
}
//应发提成冲减表
export function* searchYftccjbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询应发提成冲减表成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchYftccjbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchYftccjbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_YFTCCJB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询应发提成冲减表异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询应发提成冲减表失败!',
            duration: 3
        });
    }
}
//离职人员业绩确认表
export function* searchLzryyjqrbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询离职人员业绩确认表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchLzryyjqrbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchLzryyjqrbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_LZRYYJQRB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询离职人员业绩确认表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询离职人员业绩确认表信息失败!',
            duration: 3
        });
    }
}
//实发扣减确认表
export function* searchSfkjqrbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询实发扣减业绩确认表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchSfkjqrbDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchSfkjqrbDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERY_SFKJQRB_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询实发扣减业绩确认表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询实发扣减业绩确认表信息失败!',
            duration: 3
        });
    }
}
export default function* watchAllFinaAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERYPPFT), searchPPFtDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_YFTCB), searchYftcbDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_SFTCB), searchSftcbDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_TCCBB), searchTccbbDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_YFTCCJB), searchYftccjbDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_LZRYYJQRB), searchLzryyjqrbDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERY_SFKJQRB), searchSfkjqrbDataAsync);
}