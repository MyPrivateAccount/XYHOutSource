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
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
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

export default function* watchAllOrgAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_GET_PERMISSION_TREE), getPermissionOrgAsync);
}
