import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';
import WebApiConfig from '../constants/webApiConfig'
import getApiResult from './sagaUtil'
import ApiClient from '../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getAcmentDataListAsyncs(state){
    let result = { isOk: false, msg: '获取业绩分摊列表数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.acmentlistget+state.payload.branchId;
    try {
        console.log(url)
        console.log('getAcmentDataListAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取业绩分摊列表数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取业绩分摊列表数据信息失败!',
            duration: 3
        });
    }
    //等待数据接口
    //yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_UPDATE), payload: result });
}
//获取业绩分摊设置详情信息
export function* getAcmentDataById(state){
    let result = { isOk: false, msg: '获取业绩分摊数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.acmentdetail+state.payload;
    try {
        console.log(url)
        console.log('getAcmentDataById:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取业绩分摊数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取业绩分摊数据失败!',
            duration: 3
        });
    }
    //等待数据接口
    //yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_UPDATE), payload: result });
}
//保存数据
export function* saveAcmentDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '保存业绩分摊数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.acmentsave;
    try {
        console.log(url)
        console.log('saveAcmentDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存业绩分摊数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '保存业绩分摊数据信息失败!',
            duration: 3
        });
    }
}
//删除数据
export function* delAcmentDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '删除业绩分摊列表数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.acmentlistget+state.payload.id;
    try {
        console.log(url)
        console.log('delAcmentDataAsync:', state);
        let res = yield call(ApiClient.del, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_DEL_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取业绩分摊列表数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取业绩分摊列表数据信息失败!',
            duration: 3
        });
    }
}

//保存数据
export function* saveAcmentItemDataAsync(state){
    let result = { isOk: false, msg: '新增分摊项保存数据失败!' };
    //等待数据接口
}
export default function* watchAllAcmentAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_GET), getAcmentDataListAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_GET),getAcmentDataById)
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_SAVE), saveAcmentDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_DEL), delAcmentDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_ITEM_SAVE), saveAcmentItemDataAsync);

}