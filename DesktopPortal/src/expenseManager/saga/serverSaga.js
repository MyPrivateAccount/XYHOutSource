import {takeEvery, takeLatest} from 'redux-saga';
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

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
    let url = WebApiConfig.server.addCharge + state.payload.receiptID + '/';

    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);

        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.CLOSE_USER_BREAD), payload: {} });
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

export default function* watchServerInterface() {
    yield takeLatest(actionUtils.getActionType(actionTypes.UPLOAD_CHARGEFILE), uploadFileAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_CHARGEINFO), postChargeAsync);

}