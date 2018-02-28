import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import { appListGet, privilegeGetList, privilegeDialogClose } from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//权限列表获取
export function* getPrivilegeListAsync(state) {
    let result = { isOk: false, extension: [], msg: '权限列表获取异常!' };
    let appID = null;
    if (!state.payload.appid) {
        let appUrl = WebApiConfig.application.List;
        try {
            let appResult = yield call(ApiClient.post, appUrl, { applicationTypes: [] });
            console.log(appResult);
            getApiResult(appResult, result);
            if (result.isOk) {
                appID = result.extension[0].id;
                yield put({ type: actionUtils.getActionType(actionTypes.APP_LIST_UPDATE), payload: result.extension });
            }
        } catch (e) {

        }
    }
    let url = WebApiConfig.privilege.List + (state.payload.appid || appID);
    console.log(`获取工具列表：${url}`);
    try {
        const privilegeResult = yield call(ApiClient.get, url);
        console.log(privilegeResult);
        getApiResult(privilegeResult, result);
    } catch (e) {
        result.msg = '权限列表接口调用失败!';
    }
    // result.extension = [{ id: '1cab1', applicationId: '1', groups: '分组1', name: '权限1' + state.payload },
    // { id: 'abd22', applicationId: '1', groups: '分组2', name: '权限2' + state.payload },
    // { id: '3ef3', applicationId: '1', groups: '分组1', name: '权限11' + state.payload }];
    if (result.isOk) {
        yield put({ type: actionUtils.getActionType(actionTypes.PRIVILEGE_LIST_UPDATE), payload: result.extension });
        yield put({ type: actionUtils.getActionType(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_UPDATE), payload: { type: 'privilege', extension: result.extension } });
    } else {
        notification.error({
            message: '权限',
            description: result.msg,
            duration: 3
        });
    }
}

export function* watchPrivilegeGet() {
    yield takeLatest(actionUtils.getActionType(actionTypes.PRIVILEGE_GET_LIST), getPrivilegeListAsync);
}
//权限项保存
export function* savePrivilegeAsync(state) {
    let result = { isOk: false, msg: '权限项保存失败!', requestInfo: state.payload.data };
    let url = WebApiConfig.privilege.Base;
    let method = state.payload.method;
    if (method == 'PUT') {
        url = url + "/" + state.payload.data.id;
    }
    console.log(`权限项保存：url:${state.payload.data}提交内容：${url}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.data, null, method);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '权限项保存成功！';
            yield put(actionUtils.action(privilegeGetList, { appid: state.payload.searchAppid }));
            yield put(actionUtils.action(privilegeDialogClose, {}));
        }
    } catch (e) {
        result.msg = '权限项保存接口调用异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '权限',
        description: result.msg,
        duration: 3
    });

}

export function* watchPrivilegeSave() {
    yield takeLatest(actionUtils.getActionType(actionTypes.PRIVILEGE_SAVE), savePrivilegeAsync);
}

export function* deletePrivilegeAsync(state) {
    let result = { isOk: false, msg: '权限项删除失败！' };
    let url = WebApiConfig.privilege.Delete;
    console.log(`url:${url},删除权限项：${state.payload.delKeys}`);
    try {
        const deleteResult = yield call(ApiClient.post, url, state.payload.delKeys, null, 'DELETE');
        getApiResult(deleteResult, result);
        if (result.isOk) {
            result.msg = '权限项删除成功!';
            yield put(actionUtils.action(privilegeGetList, { appid: state.payload.searchAppid }));
        }
    } catch (e) {
        result.msg = '删除权限项接口调用异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '权限',
        description: result.msg,
        duration: 3
    });
}

export function* watchPrivilegeDelete() {
    yield takeLatest(actionUtils.getActionType(actionTypes.PRIVILEGE_DELETE), deletePrivilegeAsync);
}


