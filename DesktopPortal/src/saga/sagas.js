import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../utils/apiClient'
import * as actionTypes from '../constants/actionTypes';
import { notification } from 'antd';
import WebApiConfig from '../utils/webapiConfig';
export const delay = ms => new Promise(resolve => setTimeout(resolve, ms))

//获取收到的消息列表
export function* getReceiveListAsync(state) {
    let result = { isOk: false, extension: [], msg: '系统消息列表获取失败！' };
    let url = WebApiConfig.message.receiveList;
    try {
        let appResult = yield call(ApiClient.post, url, state.payload);
        console.log(`系统消息列表：${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(appResult)}`);
        if (appResult && appResult.data.code == '0') {
            result.isOk = true;
            //result.extension = appResult.data.extension;
            yield put({ type: actionTypes.GET_RECEIVE_LIST_COMPLETE, payload: appResult.data });
        }
    } catch (e) {
        result.msg = '应用获取接口访问异常！';
    }
    if (!result.isOk) {
        notification.error({
            message: '应用',
            description: result.msg,
            duration: 3
        });
    }
    yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
}

//获取未读消息总数
export function* getUnReadCountAsync(state) {
    let result = { isOk: false, extension: 0, msg: '未读消息数量获取失败！' };
    let url = WebApiConfig.message.unReadCount;
    try {
        let appResult = yield call(ApiClient.get, url);
        console.log(`未读消息数量：${url},result:${JSON.stringify(appResult)}`);
        if (appResult && appResult.data.code == '0') {
            result.isOk = true;
            result.extension = appResult.data.extension;
            yield put({ type: actionTypes.GET_UNREAD_COUNT_COMPLETE, payload: result.extension });
        }
    } catch (e) {
        result.msg = '获取未读消息数量接口访问异常！';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
    yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
}


//获取消息详细
export function* getMsgDetailAsync(state) {
    let result = { isOk: false, extension: 0, msg: '获取消息详细失败！' };
    let url = WebApiConfig.message.getMsgDetail + state.payload;
    try {
        let appResult = yield call(ApiClient.get, url);
        console.log(`消息详细：${url},result:${JSON.stringify(appResult)}`);
        if (appResult && appResult.data.code == '0') {
            result.isOk = true;
            result.extension = appResult.data.extension;
            yield put({ type: actionTypes.GET_SYS_MSG_DETAIL_COMPLETE, payload: result.extension });
        }
    } catch (e) {
        result.msg = '获取消息详细接口访问异常！';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
    yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
}

//权限判断
export function* judgePermissionAsync(state) {
    let result = { isOk: false, extension: [], msg: '权限判断接口失败！' };
    let url = WebApiConfig.permission.judgePermssion;
    try {
        let appResult = yield call(ApiClient.post, url, state.payload);
        console.log(`权限判断接口：${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(appResult)}`);
        if (appResult && appResult.data.code == '0') {
            result.isOk = true;
            yield put({ type: actionTypes.JUDGE_PERMISSION_COMPLETE, payload: appResult.data.extension || [] });
        }
    } catch (e) {
        result.msg = '权限判断接口访问异常！';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* watchMsgAsync() {
    yield takeLatest(actionTypes.GET_RECEIVE_LIST, getReceiveListAsync);
    yield takeLatest(actionTypes.GET_UNREAD_COUNT, getUnReadCountAsync);
    yield takeLatest(actionTypes.GET_SYS_MSG_DETAIL, getMsgDetailAsync);
    yield takeEvery(actionTypes.JUDGE_PERMISSION, judgePermissionAsync);
}