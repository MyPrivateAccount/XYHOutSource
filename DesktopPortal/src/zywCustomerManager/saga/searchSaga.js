import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import {notification} from 'antd';
import {customerlossEnd, customerlossActiveEnd} from '../actions/actionCreator';
const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* getCustomerListAsync(state) {

    let result = {isOk: false, extension: [], msg: '客源查询失败！'};
    let url = WebApiConfig.search.getSaleManCustomerList;//默认为业务员客户查询
    let body = state.payload;
    if (body) {
        if (body.searchSourceType === "2") {//已成交客户列表
            url = WebApiConfig.search.getDealCustomerList;
        }
        else if (body.searchSourceType === "3") {//失效客户列表
            url = WebApiConfig.search.getLoosCustomerList;
        }
        else if (body.searchSourceType === "4") {//公客池客户列表
            url = WebApiConfig.search.getPoolCustomerList;
        }
    }
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        if (result.isOk) {
            if (body.searchSourceType === '1' && body.pageIndex === 0) {
                yield put({type: actionUtils.getActionType(actionTypes.GET_REPEAT_COUNT), payload: body});
            }
            yield put({type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "客源查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//审核操作
export function* getCustomerDetailAsync(state) {
    let result = {isOk: false, extension: {}, msg: '加载客户详情失败！'};
    let url = WebApiConfig.search.getCustomerDetail + state.payload.id;
    try {
        let res = yield call(ApiClient.get, url);
        // console.log(res, '加载客户详情');
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '加载客户详情成功！';
            if (result.extension) {
                if (result.extension.customerDealResponse === null) {
                    result.extension.customerDealResponse = state.payload.customerDealResponse;
                }
                if (result.extension.customerLossResponse === null) {
                    result.extension.customerLossResponse = state.payload.customerLossResponse;
                }
            }
            yield put({type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_DETAIL_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "加载客户详情接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//调客
export function* adjustCustomerAsync(state) {
    let result = {isOk: false, extension: {}, msg: '调客失败！'};
    let url = WebApiConfig.search.adjustCustomer;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`加载客户详情:url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '调客成功！';
            if (state.payload.searchCondition) {
                yield put({type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER), payload: state.payload.searchCondition});
            }
            yield put({type: actionUtils.getActionType(actionTypes.CLOSE_ADJUST_CUSTOMER), payload: null});
        }
    } catch (e) {
        result.msg = "调客接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

//获取客户所有电话号码
export function* getCustomerAllPhoneAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取客户电话失败！'};
    let url = WebApiConfig.search.getCustomerAllPhone + state.payload;
    try {
        let res = yield call(ApiClient.get, url);
        // console.log(`加载客户电话:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '客户电话获取成功！';
            yield put({type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_ALL_PHONE_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "客户电话获取接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//获取审核记录
export function* getWaitAuditListAsync(state) {
    let result = {isOk: false, extension: [], msg: '获取调客审核列表失败！'};
    let url = WebApiConfig.search.getAuditList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`加载调客审核列表:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '调客审核列表获取成功！';
            yield put({type: actionUtils.getActionType(actionTypes.GET_AUDIT_LIST_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "调客审核列表获取接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


//审核操作
export function* getRepeatJudgeInfoAsync(state) {
    let result = {isOk: false, extension: null, msg: '获取重客判断信息失败！'};
    let url = WebApiConfig.search.getRepeatJudgeInfo;
    let body = state.payload;
    try {
        let res = yield call(ApiClient.post, url, body);
        getApiResult(res, result);
        // console.log(`获取重客判断信息:url:${url},result:${JSON.stringify(result)}`);
        if (result.isOk) {
            result.msg = '获取重客判断信息成功！';
            yield put({type: actionUtils.getActionType(actionTypes.GET_REPEAT_JUDGE_INFO_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取重客判断信息接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


//根据用户ID获取客户列表
export function* getCustomerListByUserIDAsync(state) {
    let result = {isOk: false, extension: [], msg: '获取客户列表失败！'};
    let url = WebApiConfig.search.getCustomerByUserID + state.payload.userID;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`获取客户列表:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        //result.type = state.payload.type;
        // console.log("state.payload", state.payload);
        if (result.isOk) {
            result.msg = '获取客户列表成功！';
            yield put({type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_OF_USERID_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取客户列表接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//获取审核历史详细
export function* getAuditHistoryDetailAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取核列详细失败！'};
    let url = WebApiConfig.search.getAuditHistory + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        // console.log(`url:${url},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_AUDIT_HISTORY_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
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

// 拉无效
export function* customerloss(action) {
    let url = WebApiConfig.search.customerloss
    let searchInfo = action.payload.searchInfo
    let type = action.payload.type
    yield put({type: actionUtils.getActionType(actionTypes.CUSTOMER_LOSS_START)});
    try {
        let res = yield call(ApiClient.post, url, action.payload.obj)
        yield put({type: actionUtils.getActionType(actionTypes.CUSTOMER_LOSS_END), payload: res});
        if (action.payload.type === 'list') {
            if (res.data.code === '0') {
                yield put({type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER), payload: searchInfo.searchCondition});
            }
        }
    } catch (e) {
        yield put(actionUtils.action(customerlossEnd, {code: '1', message: '客户拉无效接口调用异常！'}));
    }
}

// 激活客户
export function* customerlossActive(action) {
    let url = WebApiConfig.search.customerlossActive + action.payload.customerId + '/activation/' + action.payload.isDeleteOldData
    let searchInfo = action.payload.searchInfo
    yield put({type: actionUtils.getActionType(actionTypes.CUSTOMER_LOSS_ACTIVE_START)});
    try {
        let res = yield call(ApiClient.get, url)
        console.log(url, res, '87866')
        // yield put({ type: actionUtils.getActionType(actionTypes.CUSTOMER_LOSS_ACTIVE_END), payload: res });
        if (action.payload.type === 'list') {
            if (res.data.code === '0') {
                yield put({type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER), payload: searchInfo.searchCondition});
            }
        }
        if (res.data.code === '0') {
            notification.success({
                message: '激活成功',
                duration: 3
            })
        } else {
            notification.error({
                message: '激活失败',
                duration: 3
            })
        }
    } catch (e) {
        yield put(actionUtils.action(customerlossActiveEnd, {code: '1', message: '激活失效客户接口调用异常！'}));
    }
}
//获取重客分组
export function* getRepeatGroupAsync(state) {
    let result = {isOk: false, extension: [], msg: '重客分组查询失败！'};
    let url = WebApiConfig.search.getRepeatGroup;//默认为业务员客户查询
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        if (result.isOk) {
            if (res.data.validityCustomerCount != null) {
                result.validityCustomerCount = res.data.validityCustomerCount;
            }
            yield put({type: actionUtils.getActionType(actionTypes.GET_REAPEAT_GROUP_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "重客分组查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


//根据分组获取重客里列表
export function* getCustomerByGroupAsync(state) {
    let result = {isOk: false, extension: [], msg: '客源分组查询失败！'};
    let url = WebApiConfig.search.getCustomerByGroup + state.payload;//默认为业务员客户查询
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_BY_GROUP_COMPLETE), payload: result.extension || []});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "客源分组查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


//根据重客数量
export function* getRepeatCountAsync(state) {
    let result = {isOk: false, extension: [], msg: '重客总数查询失败！'};
    let url = WebApiConfig.search.getRepeatCount;//默认为业务员客户查询
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_REPEAT_COUNT_COMPLETE), payload: result.extension || 0});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "重客总数查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER), getCustomerListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_DETAIL), getCustomerDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ADJUST_CUSTOMER), adjustCustomerAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_ALL_PHONE), getCustomerAllPhoneAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_AUDIT_LIST), getWaitAuditListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_REPEAT_JUDGE_INFO), getRepeatJudgeInfoAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_OF_USERID), getCustomerListByUserIDAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_AUDIT_HISTORY), getAuditHistoryDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.CUSTOMER_LOSS), customerloss);
    yield takeLatest(actionUtils.getActionType(actionTypes.CUSTOMER_LOSS_ACTIVE), customerlossActive);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_REAPEAT_GROUP), getRepeatGroupAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_BY_GROUP), getCustomerByGroupAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_REPEAT_COUNT), getRepeatCountAsync);

}

