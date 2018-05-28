import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//保存报告基础信息
export function* saveRpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告交易合同失败！' };
    let url = WebApiConfig.rp.rpAdd;
    try {
        console.log(url)
        console.log('saveRpDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_RP_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告交易合同接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告交易合同信息失败!',
            duration: 3
        });
    }
}
//保存物业
export function* saveRpWyDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告物业信息失败！' };
    let url = WebApiConfig.rp.rpWyAdd;
    try {
        console.log(url)
        console.log('saveRpWyDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_WY_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告物业接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告物业信息失败',
            duration: 3
        });
    }
}
//保存业主
export function* saveRpYzDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告业主信息失败！' };
    let url = WebApiConfig.rp.rpYzAdd;
    try {
        console.log(url)
        console.log('saveRpYzDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告业主接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告业主信息失败!',
            duration: 3
        });
    }
}
//保存业主
export function* saveRpKhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告客户信息失败！' };
    let url = WebApiConfig.rp.rpKhAdd;
    try {
        console.log(url)
        console.log('saveRpKhDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告客户接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告客户信息失败!',
            duration: 3
        });
    }
}
//保存过户
export function* saveRpGhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告过户信息失败！' };
    let url = WebApiConfig.rp.rpGhAdd;
    try {
        console.log(url)
        console.log('saveRpGhDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_GH_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告过户接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告过户信息失败!',
            duration: 3
        });
    }
}
//保存业绩分配
export function* saveRpFpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告业绩分配信息失败！' };
    let url = WebApiConfig.rp.rpFpAdd;
    try {
        console.log(url)
        console.log('saveRpFpDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FP_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告业绩分配接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告业绩分配信息失败!',
            duration: 3
        });
    }
}
export default function* watchAllRpAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_RP_SAVE), saveRpDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_WY_SAVE), saveRpWyDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVE), saveRpYzDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_KH_SAVE), saveRpKhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_GH_SAVE), saveRpGhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FP_SAVE), saveRpFpDataAsync);
}