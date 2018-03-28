import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

///获取字典列表
export function* getParListAsync(state) {
    let result = { isOk: false, extension: [], msg: '系统参数获取失败！' };
    let url = WebApiConfig.dic.ParList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        // console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_PARLIST_COMPLETE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "系统字典参数接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            message: '系统参数',
            description: result.msg,
            duration: 3
        });
    }
}

export function* getAllAreaAsync(state) {
    let result = { isOk: false, extension: [], msg: '区域数据获取失败！' };
    let url = WebApiConfig.dic.AreaList;
    try {
        const areaResult = yield call(ApiClient.post, url, { levels: [1, 2, 3], codes: [] });
        getApiResult(areaResult, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_AREA_COMPLETE), payload: result.extension });
        }
    } catch (e) {
        result.msg = "区域数据获取接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            message: '系统参数',
            description: result.msg,
            duration: 3
        });
    }
}
//获取所有子部门
export function* getAllChildOrgsAsync(state) {
    // let result = { isOk: false, extension: [], msg: '部门数据获取失败！' };
    let result = {}
    let url = WebApiConfig.dic.OrgList + state.payload;
    try {
        const orgResult = yield call(ApiClient.get, url);
        // console.log(orgResult, '获取所有子部门', state.payload)
        getApiResult(orgResult, result);
        if (result.isOk) {
            yield put({ 
                 type: actionUtils.getActionType(actionTypes.DIC_GET_ORG_LIST_COMPLETE),
                 payload: { extension: result.extension, parentId: state.payload } 
            });
        } 
    } catch (e) {
        result.msg = "部门数据获取接口调用异常!";
    }
    // if (!result.isOk) {
    //     notification.error({
    //         description: result.msg,
    //         duration: 3
    //     });
    // }
}
//获取用户部门数据
export function* getUserOrgAsync(state) {
    // let result = { isOk: false, extension: {}, msg: '部门详细数据获取失败！' };
    let result = {}
    let url = WebApiConfig.dic.OrgDetail + state.payload;
    // console.log('部门详细数据url', url);
    try {
        const orgResult = yield call(ApiClient.get, url);
        // console.log(orgResult, '部门详细数据')
        getApiResult(orgResult, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_ORG_DETAIL_COMPLETE), payload: result.extension });
        }
    } catch (e) {
        result.msg = "部门详细数据获取接口调用异常!";
    }
    // if (!result.isOk) {
    //     notification.error({
    //         description: result.msg,
    //         duration: 3
    //     });
    // }
}
//获取部门用户
export function* getOrgUserListAsync(state) {
    let result = { isOk: false, extension: [], msg: '部门用户获取失败！' };
    let url = WebApiConfig.dic.GetOrgUserList;
    try {
        const userResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(userResult, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ORG_USERLIST_COMPLETE), payload: { extension: result.extension, type: state.payload.type } });
        }
    } catch (e) {
        result.msg = "部门用户获取接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export default function* watchDicAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_PARLIST), getParListAsync)
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_AREA), getAllAreaAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_LIST), getAllChildOrgsAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_DETAIL), getUserOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ORG_USERLIST), getOrgUserListAsync);
}

