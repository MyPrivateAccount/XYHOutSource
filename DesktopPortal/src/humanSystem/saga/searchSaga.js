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

export function* getSearchConditionAsync(state) {
    let result = {isOk: false, extension: {}, msg: '检索条件失败！'};
    let url = WebApiConfig.search.searchHumanList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
         if (res.data.code == 0) {
             result.isOk = true;
             let lv = res.data.extension;
             let data = lv.map(function(v, k) {
                 return {key: k, id: v.userID, username: v.name, idcard: v.idCard};
             });
             let re = {extension: data, 
                pageIndex: res.data.pageIndex, 
                pageSize: res.data.pageSize,
                totalCount: res.data.totalCount};

             yield put ({type: actionUtils.getActionType(actionTypes.UPDATE_ALLHUMANINFO), payload: re});
         }
    } catch (e) {
        result.msg = '检索条件调用异常';
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
             let lv = res.data.extension;
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

export function* getMonthListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取月结列表失败！'};
    let url = WebApiConfig.search.getAllMonthList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            let data = lv.extension.map(function(v, k) {
                let last = new Date(v.settleTime);
                last.setMonth(last.getMonth()-1);
                let v1 = last.getFullYear() + '.' + (last.getMonth()+1);

                last.setMonth(last.getMonth()+1);
                let v2 = last.getFullYear() + '.' + (last.getMonth()+1);
                return {key: k, last: v1, monthtime: v2, operater: v.operName};
            });

            yield put ({type: actionUtils.getActionType(actionTypes.MONTH_UPDATEMONTHLIST),
                 payload: {extension:data, pageIndex: lv.pageIndex, pageSize: lv.pageSize, totalCount: lv.totalCount, lastTime: lv.lastTime}});
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

export function* getBlackListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取月结列表失败！'};
    let url = WebApiConfig.search.getBlackList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            lv = lv.extension.map(function(v, k) {
                return {};
            });

            // yield put ({type: actionUtils.getActionType(actionTypes.MONTH_UPDATEMONTHLIST),
            //      payload: {extension: lv, pageIndex: lv.pageIndex, pageSize: lv.pageSize, totalCount: lv.totalCount, lastTime: lv.lastTime}});
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

export function* getSalaryListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取薪酬列表失败！'};
    let url = WebApiConfig.search.getSalaryList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;

            let lv = res.data.extension;
            let data = lv.extension.map(function(v, k) {
                return {key: k, id: v.id, organize: v.organize, position:v.position,
                     positionName: v.positionName, baseSalary: v.baseSalary,
                     subsidy: v.subsidy, clothesBack: v.clothesBack, administrativeBack: v.administrativeBack,
                     portBack: v.portBack};
            });
            let re = {extension: data, 
               pageIndex: lv.pageIndex, 
               pageSize: lv.pageSize,
               totalCount: lv.totalCount};

            yield put ({type: actionUtils.getActionType(actionTypes.UPDATE_SALARYINFO), payload: re});
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

export function* getSalaryItemAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取单一薪酬失败！'};
    let url = WebApiConfig.search.getSalaryItem+'/'+state.payload;

    try {
        let res = yield call(ApiClient.get, url);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put ({type: actionUtils.getActionType(actionTypes.UPDATE_SALARYITEM), payload: res.data.extension});
        }
    } catch (e) {
        result.msg = '获取单一薪酬异常';
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
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_CONDITION), getSearchConditionAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ALLHUMANINFO), getHumanListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), getMonthListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BLACKLST), getBlackListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_SALARYLIST), getSalaryListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_SALARYITEM), getSalaryItemAsync);
}