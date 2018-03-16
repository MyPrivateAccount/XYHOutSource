import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import { isNull } from 'util';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

function dealCondition(body){
    let newBody = {};
    for(let key in body){
        if(body[key] !== '' && body[key] !== null){
            newBody[key] = body[key];
        }
    }
    return newBody;
}
export function* getContractListAsync(state) {
    let result = { isOk: true, extension: [], msg: '合同查询失败！' };
    console.log('getContractListAsync:.......')
    let url = WebApiConfig.search.getContractList;
    
    let body = state.payload;
    let newBody = dealCondition(body);

    try {
        let res = yield call(ApiClient.post, url, {});
        console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)},newBody:${JSON.stringify(newBody)}`);
       getApiResult(res, result);
       if (result.isOk) {
       if (res.data.validityCustomerCount) {
               result.validityCustomerCount = res.data.validityCustomerCount;
       }
            yield put({ type: actionUtils.getActionType(actionTypes.SEARCH_COMPLETE), payload: result });
     }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = "合同查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//审核操作
export function* getContractDetailAsync(state) {
    let result = { isOk: false, extension: {}, msg: '加载客户详情失败！' };
    let url = WebApiConfig.search.getCustomerDetail + state.payload.id;
    try {
        let res = yield call(ApiClient.get, url);
        // console.log(`加载客户详情:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '加载合同详情成功！';
            if (result.extension) {
                if (result.extension.customerDealResponse === null) {
                    result.extension.customerDealResponse = state.payload.customerDealResponse;
                }
                if (result.extension.customerLossResponse === null) {
                    result.extension.customerLossResponse = state.payload.customerLossResponse;
                }
            }
            yield put({ type: actionUtils.getActionType(actionTypes.GET_CONTRACT_DETAIL_COMPLETE), payload: result.extension });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = "加载合同详情接口调用异常！";
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
    let result = { isOk: false, extension: {}, msg: '调客失败！' };
    let url = WebApiConfig.search.adjustCustomer;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`加载客户详情:url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '调客成功！';
            if (state.payload.searchCondition) {
                yield put({ type: actionUtils.getActionType(actionTypes.SEARCH_START), payload: state.payload.searchCondition });
            }
            yield put({ type: actionUtils.getActionType(actionTypes.CLOSE_ADJUST_CUSTOMER), payload: null });
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
    let result = { isOk: false, extension: {}, msg: '获取客户电话失败！' };
    let url = WebApiConfig.search.getCustomerAllPhone + state.payload;
    try {
        let res = yield call(ApiClient.get, url);
        // console.log(`加载客户电话:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '客户电话获取成功！';
            yield put({ type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_ALL_PHONE_COMPLETE), payload: result.extension });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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
    let result = { isOk: false, extension: [], msg: '获取调客审核列表失败！' };
    let url = WebApiConfig.search.getAuditList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`加载调客审核列表:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '调客审核列表获取成功！';
            yield put({ type: actionUtils.getActionType(actionTypes.GET_AUDIT_LIST_COMPLETE), payload: result });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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
    let result = { isOk: false, extension: null, msg: '获取重客判断信息失败！' };
    let url = WebApiConfig.search.getRepeatJudgeInfo + state.payload;
    try {
        let res = yield call(ApiClient.get, url);
        getApiResult(res, result);
        // console.log(`获取重客判断信息:url:${url},result:${JSON.stringify(result)}`);
        if (result.isOk) {
            result.msg = '获取重客判断信息成功！';
            yield put({ type: actionUtils.getActionType(actionTypes.GET_REPEAT_JUDGE_INFO_COMPLETE), payload: result.extension });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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
    let result = { isOk: false, extension: [], msg: '获取客户列表失败！' };
    let url = WebApiConfig.search.getCustomerByUserID + state.payload.userID;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`获取客户列表:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        //result.type = state.payload.type;
        // console.log("state.payload", state.payload);
        if (result.isOk) {
            result.msg = '获取客户列表成功！';
            yield put({ type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_OF_USERID_COMPLETE), payload: result });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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
    let result = { isOk: false, extension: {}, msg: '获取核列详细失败！' };
    let url = WebApiConfig.search.getAuditHistory + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        console.log(`url:${url},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.GET_AUDIT_HISTORY_COMPLETE), payload: result.extension });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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


export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_START), getContractListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.OPEN_CONTRACT_DETAIL), getContractDetailAsync);

    
    yield takeLatest(actionUtils.getActionType(actionTypes.ADJUST_CUSTOMER), adjustCustomerAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_ALL_PHONE), getCustomerAllPhoneAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_AUDIT_LIST), getWaitAuditListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_REPEAT_JUDGE_INFO), getRepeatJudgeInfoAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_OF_USERID), getCustomerListByUserIDAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_AUDIT_HISTORY), getAuditHistoryDetailAsync);
}

