import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import { roleGetList, privilegeGetList, roleDialogClose } from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//获取角色列表
export function* getRoleListAsync() {
    let result = { isOk: false, extension: [], msg: '获取角色列表失败!' };
    let url = WebApiConfig.role.List;
    console.log(`url:${url}`);
    try {
        const roleListResult = yield call(ApiClient.post, url, { pageIndex: 0, pageSize: 200 }, null);
        getApiResult(roleListResult, result);
    } catch (e) {
        result.msg = '获取角色列表接口调用异常!';
    }
    // let roleTempList = [{ id: '1', name: '测试角色1', OrganizationId: '' }, { id: '2', name: '测试角色2', OrganizationId: '' }];
    // result.isOk = true;
    // result.extension = roleTempList;
    if (result.isOk) {
        yield put({ type: actionUtils.getActionType(actionTypes.ROLE_LIST_UPDATE), payload: result.extension });
    } else {
        notification.error({
            message: '角色',
            description: result.msg,
            duration: 3
        });
    }
}



export function* saveRoleAsync(state) {
    let url = WebApiConfig.role.Base;
    let result = { isOk: false, msg: '角色保存失败!' };
    if (state.payload.method == 'PUT') {
        url += '/' + state.payload.data.id;
    }
    console.log(`url:${url},method:${state.payload.method},body:${JSON.stringify(state.payload.data)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.data, null, state.payload.method);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '角色保存成功!';
            yield put(actionUtils.action(roleGetList, {}));
            yield put(actionUtils.action(roleDialogClose, {}));
        }
    } catch (e) {
        result.msg = '角色保存接口调用异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '角色',
        description: result.msg,
        duration: 3
    });
}


export function* deleteRoleAsync(state) {
    let result = { isOk: false, msg: '角色删除失败!' };
    try {
        let url = WebApiConfig.role.Base + '/' + state.payload;
        console.log(`url:${url}`);
        const deleteResult = yield call(ApiClient.post, url, null, null, 'DELETE');
        getApiResult(deleteResult, result);
        if (result.isOk) {
            result.msg = '角色删除成功！';
            yield put(actionUtils.action(roleGetList, {}));
        }
    } catch (e) {
        result.msg = '角色删除接口调用失败！';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '角色',
        description: result.msg,
        duration: 3
    });
}

//获取角色拥有的权限数据
export function* getPrivilegesByRoleAsync(state) {
    let result = { isOk: false, extension: [], msg: '角色权限获取失败!' };
    let url = WebApiConfig.rolePrivilege.Base + '/' + state.payload;
    console.log(`获取角色权限url:${url}`);
    try {
        const getprivilegeResult = yield call(ApiClient.get, url);
        getApiResult(getprivilegeResult, result);
        console.log("获取到的角色权限结果：", result);
    } catch (e) {
        result.msg = '角色权限获取接口调用异常!';
    }
    if (result.isOk) {
        result.msg = '';
        yield put({ type: actionUtils.getActionType(actionTypes.ROLE_PRIVILEGE_UPDATE), payload: result.extension });
    } else {
        notification.error({
            message: '权限',
            description: result.msg,
            duration: 3
        });
    }
}

//角色对应权限保存
export function* rolePrivilegeSaveAsync(state) {
    let result = { isOk: false, msg: '角色权限保存失败!' };
    let url = WebApiConfig.rolePrivilege.Base + '/' + state.payload.roleId;
    console.log(`角色权限保存:url:${url},method:PUT,body:${JSON.stringify(state.payload.rolePrivilege)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.rolePrivilege, null, 'PUT');
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '角色权限保存成功!';
        }
        console.log("角色权限保存结果：", saveResult);
    } catch (e) {
        result.msg = '角色权限保存接口调用异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '角色权限',
        description: result.msg,
        duration: 3
    });
}

export function* loadAllPrivilegeItemAsync(state) {
    let result = { isOk: false, msg: '工具权限项失败!' };
    let appList = state.payload;
    if (!appList || appList.length == 0) {
        let url = WebApiConfig.application.List;
        try {
            let appResult = yield call(ApiClient.post, url, { applicationTypes: state.payload });
            getApiResult(appResult, result);
            appList = result.extension;
            yield put({ type: actionUtils.getActionType(actionTypes.APP_LIST_UPDATE), payload: result.extension });
            yield put({ type: actionUtils.getActionType(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_UPDATE), payload: { type: 'app', extension: result.extension } });
        } catch (e) {
            result.msg = '应用获取接口访问异常！';
        }
    }
    try {
        for (let i in appList) {
            let app = appList[i];
            // yield put(actionUtils.action(privilegeGetList), app.id);
            let url = WebApiConfig.privilege.List + app.id;
            console.log(`获取工具列表：${url}`);
            const privilegeResult = yield call(ApiClient.get, url);
            getApiResult(privilegeResult, result);
            yield put({ type: actionUtils.getActionType(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_UPDATE), payload: { type: 'privilege', extension: result.extension } });
        };
    } catch (e) {
        notification.error({
            message: '角色权限项',
            description: result.msg,
            duration: 3
        });
    }
}

//角色对应应用保存
export function* saveRoleApplicationAsync(state) {
    let result = { isOk: false, msg: '角色应用保存失败!' };
    let url = WebApiConfig.rolePrivilege.Application + state.payload.roleId;
    console.log(`角色应用保存:url:${url},method:PUT,body:${JSON.stringify(state.payload.entity)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.entity, null, 'PUT');
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '角色应用保存成功!';
        }
        console.log("角色应用保存结果：", saveResult);
    } catch (e) {
        result.msg = '角色应用保存接口调用异常!';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export default function* watchAllRoleAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_GET_LIST), getRoleListAsync)
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_SAVE), saveRoleAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_DELETE), deleteRoleAsync)
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_PRIVILEGE_GET), getPrivilegesByRoleAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_PRIVILEGE_SAVE), rolePrivilegeSaveAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_GET), loadAllPrivilegeItemAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ROLE_APPLICATION_SAVE), saveRoleApplicationAsync);
}