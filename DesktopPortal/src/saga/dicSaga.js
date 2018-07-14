import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../utils/apiClient'
import * as actionTypes from '../constants/actionTypes';
import WebApiConfig from '../utils/webapiConfig';
import getApiResult from '../utils/sagaUtil';
import {notification} from 'antd';
import {globalAction} from 'redux-subspace';

///获取字典列表
export function* getParListAsync(state) {
    let result = {isOk: false, extension: [], msg: '系统参数获取失败！'};
    let url = WebApiConfig.dic.ParList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        console.log('最外层获取到的参数列表:', res);
        getApiResult(res, result);
        if (result.isOk) {
            yield put(globalAction({type: actionTypes.DIC_GET_PARLIST_COMPLETE, payload: result.extension}));
        }
    } catch (e) {
        result.msg = "系统字典参数接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            message: '系统参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* getAllAreaAsync(state) {
    let result = {isOk: false, extension: [], msg: '区域数据获取失败！'};
    let url = WebApiConfig.dic.AreaList;
    try {
        const areaResult = yield call(ApiClient.post, url, {levels: [1, 2, 3], codes: []});
        getApiResult(areaResult, result);
        if (result.isOk) {
            yield put({type: actionTypes.DIC_GET_AREA_COMPLETE, payload: result.extension});
        }
    } catch (e) {
        result.msg = "区域数据获取接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            message: '系统参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* watchDicAllAsync() {
    yield takeEvery(actionTypes.DIC_GET_PARLIST, getParListAsync)
    yield takeLatest(actionTypes.DIC_GET_AREA, getAllAreaAsync);
}

