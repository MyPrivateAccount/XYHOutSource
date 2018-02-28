import {takeEvery, takeLatest, delay} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import {notification} from 'antd';


const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* searchMyBuildingAsync(state) {
    let result = {isOk: false, extension: [], msg: '负责房源查询失败！'};
    let url = WebApiConfig.search.responsibleSearch;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        console.log("负责房源:", result.extension.length);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.SEARCH_XYH_BUILDING_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "负责房源查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

//推荐楼盘
export function* recommendBuildingAsync(state) {
    let result = {isOk: false, extension: [], msg: '推荐楼盘失败！'};
    let url = WebApiConfig.search.buildingRecommend;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        // console.log(`推荐楼盘:url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '推荐楼盘成功！';
            yield put({type: actionUtils.getActionType(actionTypes.CLOSE_RECOMMEND_DIALOG), payload: null});
            // delay(500);
            // yield put({type: actionUtils.getActionType(actionTypes.GET_MYCOMMEND_BUILDING), payload: {pageIndex: 0, pageSize: 1000}});
        }
    } catch (e) {
        result.msg = "推荐楼盘接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

//取消推荐
export function* cancelXYHRecommendAsync(state) {
    let result = {isOk: false, extension: [], msg: '取消推荐失败！'};
    let url = WebApiConfig.search.cancelRecommend;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '已成功取消推荐！';
            // delay(500);
            // yield put({type: actionUtils.getActionType(actionTypes.GET_MYCOMMEND_BUILDING), payload: {pageIndex: 0, pageSize: 1000}});
        }
    } catch (e) {
        result.msg = "取消推荐接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

//获取我推荐的楼盘
export function* getMyRecommendBuildingAsync(state) {
    let result = {isOk: false, extension: [], msg: '获取我推荐的楼盘失败！'};
    let url = WebApiConfig.search.getMyRecommendList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        console.log(`我推荐的楼盘url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        console.log("我推荐的楼盘:", result.extension.length);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_MYCOMMEND_BUILDING_COMPLETE), payload: result.extension});
        }
        // yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取我推荐的楼盘接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export function* getBuildingDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '楼盘详情查询失败！'};
    let url = WebApiConfig.search.xyhBuildingDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        console.log(`楼盘详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_BUILDING_DETAIL_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
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
    let url = WebApiConfig.search.xyhBuildingShops;
    try {
        let body = {buildingIds: [state.payload.buildingIds], saleStatus: state.payload.saleStatus, pageIndex: 0, pageSize: 10000};
        let res = yield call(ApiClient.post, url, body);
        console.log(`获取楼盘商铺:url:${url},body:${JSON.stringify(body)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            if (result.extension.length == 0) {
                notification.info({
                    description: (state.payload.saleStatus || []).length > 0 ? "该分类下没有商铺!" : "该楼盘下没有商铺！",
                    duration: 3
                });
            }
            yield put({type: actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS_COMPLETE), payload: result.extension || []});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
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

export function* getShopDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '商铺详情获取失败！'};
    let url = WebApiConfig.search.xyhBuildingShopDetail + state.payload;
    try {
        const areaResult = yield call(ApiClient.get, url);
        getApiResult(areaResult, result);
        //console.log(`获取商铺详情：${url},result:${JSON.stringify(areaResult)}`);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS_DETAIL_COMPLETE), payload: result.extension});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "商铺详情接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_XYH_BUILDING), searchMyBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.RESULT_BUILDING_RECOMMEND), recommendBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.CANCEL_RECOMMEND), cancelXYHRecommendAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_MYCOMMEND_BUILDING), getMyRecommendBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_DETAIL), getBuildingDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS),getBuildingShopsAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS_DETAIL), getShopDetailAsync);
}

