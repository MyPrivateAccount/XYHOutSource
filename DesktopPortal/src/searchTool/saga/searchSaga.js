import {takeEvery, takeLatest, delay} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import {notification} from 'antd';
import {getActiveListStart, getActiveListEnd} from '../actions/actionCreator'

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* searchXYHBuildingAsync(state) {
    let result = {isOk: false, extension: [], msg: '房源查询失败！'};
    let url = WebApiConfig.search.xyhBuildingSearch;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        // console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.SEARCH_XYH_BUILDING_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "房源查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getXYHBuildingDetailAsync(state) {
    let result = {isOk: false, extension: [], msg: '楼盘详情查询失败！'};
    let url = WebApiConfig.search.xyhBuildingDetail + state.payload;
    try {
        let res = yield call(ApiClient.get, url)
        //console.log(`楼盘详细:url:${url},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.XYH_GET_BUILDING_COMPLETE), payload: result.extension});
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

//推荐楼盘
export function* recommendBuildingAsync(state) {
    let result = {isOk: false, extension: [], msg: '推荐楼盘失败！'};
    let url = WebApiConfig.search.buildingRecommend;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        //console.log(`推荐楼盘:url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = '推荐楼盘成功！';
            yield put({type: actionUtils.getActionType(actionTypes.CLOSE_RECOMMEND_DIALOG), payload: null});
            yield put({type: actionUtils.getActionType(actionTypes.GET_RECOMMEND_LIST), payload: {pageIndex: 0, pageSize: 10}});
        }
    } catch (e) {
        result.msg = "推荐楼盘接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
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
//判断是否有推荐权限
export function* hasRecommendPermissionAsync(state) {
    let result = {isOk: false, extension: false, msg: '权限判断失败！'};
    let url = WebApiConfig.search.xyhHasPermission;
    try {
        const areaResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(areaResult, result);
        console.log(`权限判断：${url},result:${JSON.stringify(areaResult)}`);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_PERMISSION_COMPLETE), payload: {extension: result.extension, permission: state.payload}});
        }
    } catch (e) {
        result.msg = "权限判断接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//查询推荐列表
export function* searchXYHRecommendListAsync(state) {
    let result = {isOk: false, extension: [], msg: '推荐房源查询失败！'};
    let url = WebApiConfig.search.getRecommendList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_RECOMMEND_LIST_COMPLETE), payload: result.extension});
        }
        //yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = "推荐房源查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
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
            delay(200);
            yield put({type: actionUtils.getActionType(actionTypes.GET_RECOMMEND_LIST), payload: {pageIndex: 0, pageSize: 10}});
        }
    } catch (e) {
        result.msg = "取消推荐接口调用异常！";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}


// 点击查看成交信息
export function* getCustomerDeal(action) {
    let result = {isOk: false, extension: {}, msg: '获取成交信息失败！'};
    let url = WebApiConfig.search.GetCustomerDeal + action.payload;
    try {
        let res = yield call(ApiClient.get, url);
        getApiResult(res, result);
        console.log(`url:${url}成交信息:${JSON.stringify(result)}`)
        yield put({type: actionUtils.getActionType(actionTypes.GET_CUSTOMER_DEAL_INFO_COMPLETE), payload: result.extension});
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取成交信息接口异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//获取消息列表
export function* getMsgListAsync(state) {
    let result = {isOk: false, extension: [], msg: '消息列表获取失败！'};
    let url = WebApiConfig.msg.getMsgList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        console.log(`获取消息列表url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.GET_MSG_LIST_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取消息列表接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//获取消息详细
export function* getMsgDetailAsync(action) {
    let result = {isOk: false, extension: {}, msg: '获取消息详细失败！'};
    let url = WebApiConfig.msg.getMsgDetail + action.payload;
    try {
        let res = yield call(ApiClient.get, url);
        getApiResult(res, result);
        console.log(`获取消息详细url:${url},result:${JSON.stringify(result)}`)
        yield put({type: actionUtils.getActionType(actionTypes.GET_MSG_DETAIL_COMPLETE), payload: result.extension});
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "获取消息详细接口异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

// 获取房源动态列表
export function* getActiveList(action) {
    let dataList = []
    let url = WebApiConfig.dynamicProject.List;
    let body = {
        updateTypes: [1,2],
        examineStatus: [8],
        pageIndex: action.payload.pageIndex,
        pageSize: 10,
        contentIds: [action.payload.buildingId]
    }
    let res;
    console.log(url, JSON.stringify(body), '获取房源动态列表地址')
    yield put(actionUtils.action(getActiveListStart))
    try {
      let res = yield call(ApiClient.post, url, body)
      console.log(res, '获取房源动态列表res')
      yield put(actionUtils.action(getActiveListEnd, res))
    } catch (e) {
      yield put(actionUtils.action(getActiveListEnd, { code: '1', message: '获取房源动态列表失败'}))
    }
}

export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_XYH_BUILDING), searchXYHBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.XYH_GET_BUILDING_DETAIL), getXYHBuildingDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.RESULT_NEXT), getXYHBuildingDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.RESULT_PREV), getXYHBuildingDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS), getBuildingShopsAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.RESULT_BUILDING_RECOMMEND), recommendBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_SHOPS_DETAIL), getShopDetailAsync);
    yield takeEvery(actionUtils.getActionType(actionTypes.GET_RECOMMEND_PERMISSION), hasRecommendPermissionAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_RECOMMEND_LIST), searchXYHRecommendListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.CANCEL_RECOMMEND), cancelXYHRecommendAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_DEAL_INFO), getCustomerDeal);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_MSG_LIST), getMsgListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_MSG_DETAIL), getMsgDetailAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ACTIVE_LIST), getActiveList);
}

