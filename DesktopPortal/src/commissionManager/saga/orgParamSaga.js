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
export function* getOrgParamDataListByOrgIdAsyncs(state){
    let result = { isOk: false, msg: '获取组织参数设置列表成功!' };
    let url = WebApiConfig.baseset.orgsave+state.payload.branchId;
    try {
        console.log(url)
        console.log('getOrgParamDataListByOrgIdAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAMLIST_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取组织参数设置列表接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取组织参数设置列表信息失败!',
            duration: 3
        });
    }
    //等待数据接口
    //yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAMLIST_UPDATE), payload: result });
}
//保存数据
export function* saveOrgParamDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '保存组织参数设置成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.orgsave;
    try {
        console.log(url)
        console.log('saveOrgParamDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAM_SAVE_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存组织参数设置接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '保存组织参数设置失败!',
            duration: 3
        });
    }
}
//获取列表数据
export function* delOrgParamAsyncs(state){
    let result = { isOk: false, msg: '删除组织参数设置成功!' };
    let url = WebApiConfig.baseset.orgsave+state.payload.branchId+'/'+state.payload.parCode;
    try {
        console.log(url)
        console.log('delOrgParamAsyncs:', state);
        let res = yield call(ApiClient.del, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAM_DEL_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "删除组织参数设置接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '删除组织参数设置信息失败!',
            duration: 3
        });
    }
    //等待数据接口
    //yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAMLIST_UPDATE), payload: result });
}

export default function* watchAllOrgParamAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_PARAMLIST_GET), getOrgParamDataListByOrgIdAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_PARAM_SAVE), saveOrgParamDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_PARAM_DEL), delOrgParamAsyncs);
}