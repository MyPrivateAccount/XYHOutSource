import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';
import WebApiConfig from '../constants/webApiConfig';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import ApiClient from '../../utils/apiClient'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getScaleDataListAsyncs(state){
    //等待数据接口
    let result = { isOk: false, msg: '获取提成比例列表成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.incomesave+state.payload.branchId+'/'+state.payload.code;
    try {
        console.log(url)
        console.log('getScaleDataListAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.INCOME_SCALE_LIST_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取提成比例列表接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取提成比例列表失败!',
            duration: 3
        });
    }
}
//保存数据
export function* saveScaleDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '保存提成比例设置成功!' };
    console.log(state)
    let url = ''
    if(state.payload.mod){
        url = WebApiConfig.baseset.incomesave + state.payload.id
        console.log(url)
    }
    else{
        url = WebApiConfig.baseset.incomesave
        console.log(url)
    }
    try {
        console.log(url)
        console.log('saveScaleDataAsync:', state);
        let res = null
        if(state.payload.mod){
            res = yield call(ApiClient.put, url, state.payload);
        }
        else{
            res = yield call(ApiClient.post, url, state.payload);
        }
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.INCOME_SCALE_SAVE_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存提成比例设置接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '保存提成比例设置失败!',
            duration: 3
        });
    }
}
//删除数据
export function* delScaleDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '删除提成比例设置成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.incomesave+state.payload.id;
    try {
        console.log(url)
        console.log('delScaleDataAsync:', state);
        let res = yield call(ApiClient.del, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.INCOME_SCALE_DEL_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "删除提成比例设置接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '删除提成比例设置失败!',
            duration: 3
        });
    }
}

export default function* watchAllScaleAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_LIST_GET), getScaleDataListAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_SAVE), saveScaleDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_DEL), delScaleDataAsync);

}