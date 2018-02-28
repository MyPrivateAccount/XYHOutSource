import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import SearchCondition from '../constants/searchCondition';

//获取待审核列表
export function* getAuditListAsync(state) {
    let result = { isOk: false, extension: [], msg: '待审核列表查询失败！' };
    let url = WebApiConfig.audit.myAudit.getWaitAuditList;
    let body = state.payload;
    try {
        if (body) {
            if (body.listType === "myAudit_audited") {
                url = WebApiConfig.audit.myAudit.getAuditedList;
            } else if (body.listType === "mySubmit") {
                url = WebApiConfig.audit.mySubmit;
            } else if (body.listType === "copyToMe") {
                url = WebApiConfig.audit.coypToMe;
            }
        }
        let res = yield call(ApiClient.post, url, state.payload)
        console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({
                type: actionTypes.GET_AUDIT_LIST_COMPLETE, payload: {
                    extension: result,
                    listType: body.listType,
                    examineStatus: body.examineStatus || body.status
                }
            });
        }
        yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
    } catch (e) {
        result.msg = "待审核列接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//审核操作
export function* auditAsync(state) {
    let result = { isOk: false, extension: [], msg: '审核保存失败！' };
    let entity = state.payload;
    let url = WebApiConfig.audit.passAudit + entity.recordId;
    try {
        let res;
        if (entity.auditStatus) {
            res = yield call(ApiClient.post, url, entity.desc, null, "PUT");
        } else {
            url = WebApiConfig.audit.rejectAudit + entity.recordId;
            res = yield call(ApiClient.post, url, entity.desc, null, 'PUT');
        }
        console.log(`审核保存:url:${url},body:${JSON.stringify(state.payload.desc)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '审核保存成功！';
            yield put({ type: actionTypes.GET_AUDIT_LIST, payload: SearchCondition.myAudit.waitAuditListCondition })
        }
        yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
    } catch (e) {
        result.msg = "审核保存接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}
//获取审核历史详细
export function* getAuditHistoryDetailAsync(state) {
    let result = { isOk: false, extension: {}, msg: '获取核列详细失败！' };
    let url = WebApiConfig.audit.getAuditHistory + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        console.log(`url:${url},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({ type: actionTypes.GET_AUDIT_HISTORY_COMPLETE, payload: result.extension });
            if (result.extension.submitDefineId) {
                yield put({ type: actionTypes.GET_UPDATE_RECORD_DETAIL, payload: result.extension.submitDefineId });
            }
        }
        yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
    } catch (e) {
        result.msg = "获取核列详细接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//知会未读总数
export function* getNoReadCountAsync(state) {
    let result = { isOk: false, extension: 0, msg: '获取知会未读总数失败！' };
    let url = WebApiConfig.audit.getNoReadCount;
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        console.log(`url:${url},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({ type: actionTypes.GET_NO_READ_COUNT_COMPLETE, payload: result.extension });
        }
        yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
    } catch (e) {
        result.msg = "知会未读总数接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//获取房源动态审核详细
export function* getUpdateRecordDetailAsync(state) {
    let result = { isOk: false, extension: 0, msg: '获取房源动态详细失败！' };
    let url = WebApiConfig.audit.getUpdateRecordDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        console.log(`url:${url},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({ type: actionTypes.GET_UPDATE_RECORD_DETAIL_COMPLETE, payload: result.extension });
        }
        yield put({ type: actionTypes.SET_SEARCH_LOADING, payload: false });
    } catch (e) {
        result.msg = "获取房源动态详细接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export default function* watchAllSearchAsync() {
    yield takeLatest(actionTypes.GET_AUDIT_LIST, getAuditListAsync);
    yield takeLatest(actionTypes.SAVE_AUDIT, auditAsync);
    yield takeLatest(actionTypes.GET_AUDIT_HISTORY, getAuditHistoryDetailAsync);
    yield takeLatest(actionTypes.GET_NO_READ_COUNT, getNoReadCountAsync);
    yield takeLatest(actionTypes.GET_UPDATE_RECORD_DETAIL, getUpdateRecordDetailAsync);
}

