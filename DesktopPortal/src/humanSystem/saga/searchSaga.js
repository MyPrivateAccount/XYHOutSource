import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* getCustomerListAsync(state) {
    let result = { isOk: false, extension: [], msg: '客源查询失败！' };
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
        // console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            if (res.data.validityCustomerCount) {
                result.validityCustomerCount = res.data.validityCustomerCount;
            }
            yield put({ type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER_COMPLETE), payload: result });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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
// //审核操作
// export function* getCustomerDetailAsync(state) {
//     let result = { isOk: false, extension: {}, msg: '加载客户详情失败！' };
//     let url = WebApiConfig.search.getCustomerDetail + state.payload.id;
//     try {
//         let res = yield call(ApiClient.get, url);
//         // console.log(`加载客户详情:url:${url},result:${JSON.stringify(res)}`);
//         getApiResult(res, result);
//         if (result.isOk) {
//             result.msg = '加载客户详情成功！';
//             if (result.extension) {
//                 if (result.extension.customerDealResponse === null) {
//                     result.extension.customerDealResponse = state.payload.customerDealResponse;
//                 }
//                 if (result.extension.customerLossResponse === null) {
//                     result.extension.customerLossResponse = state.payload.customerLossResponse;
//                 }
//             }
//             yield put({ type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_DETAIL_COMPLETE), payload: result.extension });
//         }
//         yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
//     } catch (e) {
//         result.msg = "加载客户详情接口调用异常！";
//     }
//     if (!result.isOk) {
//         notification.error({
//             description: result.msg,
//             duration: 3
//         });
//     }
// }

export function* getSearchWordListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '检索关键字失败！'};
    let url = WebApiConfig.search.searchWordHumanList;
    try {
        let res = yield call(ApiClient.post, url);
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getHumanListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '检索关键字失败！'};
    let url = WebApiConfig.search.searchHumanList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
         if (res.data.code == 0) {
             result.isOk = true;
             let lv = JSON.parse(res.data.extension);
             let data = lv.map(function(v, k) {
                 return {key: k, id: v.userID, username: v.name, idcard: v.idCard};
             });

             yield put ({type: actionUtils.getActionType(actionTypes.UPDATE_ALLHUMANINFO), payload: data});
         }
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
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
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_KEYWORD), getSearchWordListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ALLHUMANINFO), getHumanListAsync);
}