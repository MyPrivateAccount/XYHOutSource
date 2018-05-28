import {takeEvery, takeLatest} from 'redux-saga';
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import searchCondition from '../../contractManagement/pages/searchCondition';
import { postReciept } from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

///上传文件
export function* uploadFileAsync(state) {
    let result = { isOk: false, extension: [], msg: '文件上传失败！' };
    let url = WebApiConfig.server.uploadImg + state.payload.receiptID + '/';

    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        // if (result.isOk) {
        //     yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_PARLIST_COMPLETE), payload: result.extension });
        // }
    } catch (e) {
        result.msg = "文件上传接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '文件上传',
            description: result.msg,
            duration: 3
        });
    }
}

export function* postChargeAsync(state) {
    let result = { isOk: false, extension: [], msg: '费用添加失败！' };
    let url = WebApiConfig.server.addCharge;

    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.SET_USER_BREADINDEX), payload: 1 });
        }
    } catch (e) {
        result.msg = "费用添加接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '费用添加参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* postsearchCondition(state) {
    let result = { isOk: false, extension: [], msg: '查询失败！' };
    let url = WebApiConfig.server.searchCharge;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_SEARCHCONDITION), payload: result });
        }
    } catch (e) {
        result.msg = "查询接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '查询参数',
            description: result.msg,
            duration: 3
        });
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    }
}

export function* getRecieptInfo(state) {
    let result = { isOk: false, extension: [], msg: '查询失败！' };
    let url = WebApiConfig.server.getRecieptInfo + state.payload;

    try {
        
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_RECIPTINFO), payload: result.extension });
        }
    } catch (e) {
        result.msg = "查询接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '查询参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* postPaymentCharge(state) {
    let result = { isOk: false, extension: [], msg: '查询失败！' };
    let url = WebApiConfig.server.updatePostTime + state.payload.chargeid + "/" + state.payload.department;

    try {
        let res = yield call(ApiClient.post, url)
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.SET_USER_BREADINDEX), payload: 1 });
        }
    } catch (e) {
        result.msg = "查询接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '查询参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* postRecieptInfo(state) {
    let result = { isOk: false, extension: [], msg: '查询失败！' };
    let url = WebApiConfig.server.postreciept;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.SET_USER_BREADINDEX), payload: 1 });
        }
    } catch (e) {
        result.msg = "查询接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '查询参数',
            description: result.msg,
            duration: 3
        });
    }
}


export default function* watchServerInterface() {
    yield takeLatest(actionUtils.getActionType(actionTypes.UPLOAD_CHARGEFILE), uploadFileAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_CHARGEINFO), postChargeAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_SEARCHCONDITION), postsearchCondition);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_RECIEPTBYID), getRecieptInfo);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_PAYMENTCHARGE), postPaymentCharge);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_RECIEPTINFO), postRecieptInfo);
}