import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webApiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取有权限的部门树(添加用户时：UserInfoCreate，添加角色时：RoleCreate，添加公共角色时：PublicRoleOper，角色授权时：AuthorizationPermission)
export function* getPermissionOrgAsync(state) {
    let result = { isOk: false, msg: '根据权限获取部门数据失败!' };
    let url = WebApiConfig.org.permissionOrg + state.payload;
    try {
        const deleteResult = yield call(ApiClient.get, url);
        //console.log(`部门获取请求：url:${url},id:${state.payload},result:${JSON.stringify(deleteResult)}`);
        getApiResult(deleteResult, result);
        if (result.isOk) {
            result.msg = '部门获取成功!';
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_GET_PERMISSION_TREE_UPDATE), payload: { permissionType: state.payload, extension: result.extension } });
        }
    } catch (e) {
        result.msg = '部门按权限获取接口调用异常!';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
//获取用户列表
export function* getEmpListAsync(state) {
    let result = { isOk: false, extension: [], msg: '用户列表获取失败' };
    let url = WebApiConfig.user.List;
    console.log(`url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const empResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(empResult, result);
        ///yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '用户列表获取接口调用失败！';
    }
    if (result.isOk) {
            console.log("获取用户列表成功:"+result)
            yield put({ type: actionUtils.getActionType(actionTypes.EMP_LIST_UPDATE), payload: result });
    } else {
        notification.error({
            message: '员工',
            description: result.msg,
            duration: 3
        });
    }

}
//根据分公司的id获取分公司的员工列表
export function* getHumanListAsync(state) {
    let result = { isOk: false, extension: [], msg: '员工列表获取失败' };
    let url = WebApiConfig.human.List;
    console.log(`url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const humanResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(humanResult, result);
        ///yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '员工列表获取接口调用失败！';
    }
    if (result.isOk) {
            console.log("获取员工列表成功:"+result)
            yield put({ type: actionUtils.getActionType(actionTypes.SEARCH_HUMAN_INFO_SUCCESS), payload: result });
    } else {
        notification.error({
            message: '员工',
            description: result.msg,
            duration: 3
        });
    }

}

export default function* watchAllOrgAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_GET_PERMISSION_TREE), getPermissionOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_GET_LIST), getEmpListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_HUMAN_INFO), getHumanListAsync);
}
