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

//获取所有子部门
export function* getAllOrgsAsync(state) {
    let result = {}
    let url = WebApiConfig.dic.permissionOrg + state.payload;
    
    try {
        const orgResult = yield call(ApiClient.get, url);
        getApiResult(orgResult, result);
        if (result.isOk) {
            yield put({ 
                 type: actionUtils.getActionType(actionTypes.DIC_GET_ALL_ORG_LIST_COMPLETE),
                 payload: { extension: result.extension, type: state.payload } 
            });
        } 
    } catch (e) {
        result.msg = "获取所有可操作部门接口调用异常!";
        console.log('getAllOrgsAsync error,url:', url);
    }
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

export function* getWorkNumber(state) {
    let url = WebApiConfig.server.GetWorkNumber;
    let huResult = { isOk: false, msg: '获取工号失败！' };
    try {
        huResult = yield call(ApiClient.get, url);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.message = '人事信息提交成功';

            yield put({ type: actionUtils.getActionType(actionTypes.SET_HUMANINFONUMBER), payload: {worknumber:huResult.data.extension} });
        }
    } catch (e) {
        huResult.msg = "部门用户获取接口调用异常!";
    }
    
    if (!huResult.isOk) {
        notification.error({
            message: huResult.msg,
            duration: 3
        });
    }
}

export default function* watchDicAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_PARLIST), getParListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_LIST), getAllOrgsAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_DETAIL), getUserOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ORG_USERLIST), getOrgUserListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_HUMANINFONUMBER), getWorkNumber);
}

