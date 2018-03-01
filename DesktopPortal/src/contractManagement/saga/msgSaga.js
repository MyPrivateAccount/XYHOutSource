import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

//发送楼盘消息/通知
export function* sendBuildingMsgAsync(state) {
    let result = { isOk: false, msg: '楼盘消息保存失败！' };
    let url = WebApiConfig.msg.sendMsg;
    let body = state.payload;
    try {
        const saveResult = yield call(ApiClient.post, url, body);
        getApiResult(saveResult, result);
        // console.log(`楼盘消息保存url:${url},body:${JSON.stringify(body)}`);
        if (result.isOk) {
            result.msg = "楼盘消息发送成功";
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_MSG_LOADING), payload: false });
    } catch (e) {
        result.msg = "楼盘消息保存接口调用异常!";
    }
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}


export function* watchMsgAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEND_BUILDING_MSG), sendBuildingMsgAsync);
}