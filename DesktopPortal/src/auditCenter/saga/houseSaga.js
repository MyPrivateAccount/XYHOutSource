import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import getApiResult from './sagaUtil';
import {notification} from 'antd';


export function* getXYHBuildingDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '楼盘详情查询失败！'};
    let url = WebApiConfig.houseActive.buildingDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        console.log(`楼盘详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionTypes.GET_BUILDING_DETAIL_COMPLETE, payload: result.extension});
        }
        yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    } catch (e) {
        result.msg = "楼盘详情接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//获取楼盘商铺列表
export function* getBuildingShopsAsync(state) {
    let result = {isOk: false, extension: [], msg: '获取楼盘商铺失败！'};
    let url = WebApiConfig.houseActive.buildingShops;
    try {
        let body = {buildingIds: [state.payload], isContainBaseInfo: true, pageIndex: 0, pageSize: 10000};
        let res = yield call(ApiClient.post, url, body);
        console.log(`获取楼盘商铺:url:${url},body:${JSON.stringify(body)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionTypes.GET_BUILDING_SHOPS_COMPLETE, payload: result.extension});
        }
        yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    } catch (e) {
        result.msg = "获取楼盘商铺接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//获取租壹屋楼盘详情
export function* getZYWBuildingDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '楼盘详情查询失败！'};
    let url = WebApiConfig.houseActive.zywBuildingDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        console.log(`楼盘详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionTypes.GET_BUILDING_DETAIL_COMPLETE, payload: result.extension});
        }
        yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    } catch (e) {
        result.msg = "楼盘详情接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//获取新耀行商铺详情
export function* getXYHShopDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '商铺详情查询失败！'};
    let url = WebApiConfig.houseActive.getShopDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        console.log(`商铺详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionTypes.GET_SHOP_DETAIL_END, payload: result.extension});
        }
        yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    } catch (e) {
        result.msg = "商铺详情接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//获取租壹屋商铺详情
export function* getZYWShopDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '商铺详情查询失败！'};
    let url = WebApiConfig.houseActive.getZYWShopDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        console.log(`商铺详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionTypes.GET_SHOP_DETAIL_END, payload: result.extension});
        }
        yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    } catch (e) {
        result.msg = "商铺详情接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export default function* watchHouseActiveAsync() {
    yield takeLatest(actionTypes.GET_BUILDING_DETAIL, getXYHBuildingDetailAsync);
    yield takeLatest(actionTypes.GET_BUILDING_SHOPS, getBuildingShopsAsync);
    yield takeLatest(actionTypes.GET_ZYW_BUILDING_DETAIL, getZYWBuildingDetailAsync);
    yield takeLatest(actionTypes.GET_SHOP_DETAIL, getXYHShopDetailAsync);
    yield takeLatest(actionTypes.GET_ZYW_SHOP_DETAIL, getZYWShopDetailAsync);
}
