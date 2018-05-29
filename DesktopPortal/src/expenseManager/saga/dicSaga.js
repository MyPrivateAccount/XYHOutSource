import {takeEvery, takeLatest} from 'redux-saga';
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

///获取字典列表
export function* getParListAsync(state) {
    let result = { isOk: false, extension: [], msg: '字典获取失败！' };
    let url = WebApiConfig.dic.ParList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_PARLIST_COMPLETE), payload: result.extension });
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

export function* getDepartmentListAsync(state) {
    let result = { isOk: false, extension: [], msg: '获取部门失败！' };
    let url = WebApiConfig.dic.permissionOrg + state.payload;
    
    try {
        const orgResult = yield call(ApiClient.get, url);
        getApiResult(orgResult, result);

        if (result.isOk) {
            yield put({
                 type: actionUtils.getActionType(actionTypes.DIC_GET_ALL_ORG_LIST_COMPLETE),
                 payload: { extension: result.extension, type: state.payload }
            });
        }

    } catch (e) {
        result.msg = "获取所有可操作部门接口调用异常!";
        console.log('getAllOrgsAsync error,url:', url);
    }
}

export function* getLimitChargeHuman(state) {
    let result = {}
    let url = WebApiConfig.userTypeValue.Permission+state.payload;
    
    try {
        const orgResult = yield call(ApiClient.get, url);
        getApiResult(orgResult, result);

        if (result.isOk) {
            yield put({
                 type: actionUtils.getActionType(actionTypes.UPDATE_LIMITCHARGEHUMAN),
                 payload: result.extension
            });
        }

    } catch (e) {
        result.msg = "获取限制名单接口调用异常!";
        console.log('getAllOrgsAsync error,url:', url);
    }
}

export default function* watchGetAlldic() {
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ALLDEPARTMENT), getDepartmentListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CHARGEDICINFO), getParListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_LIMITCHARGEHUMAN), getLimitChargeHuman);
}