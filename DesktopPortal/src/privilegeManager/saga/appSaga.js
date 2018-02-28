import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import { appListGet, appDialogClose } from '../actions/actionCreator';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

//获取应用列表
export function* getAppListAsync(state) {
    let result = { isOk: false, extension: [], msg: '应用列表获取失败！' };
    let appList = [];
    let url = WebApiConfig.application.List;
    console.log(`获取工具列表：${url}`);
    try {
        let appResult = yield call(ApiClient.post, url, { applicationTypes: state.payload });
        console.log(appResult);
        getApiResult(appResult, result);
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '应用获取接口访问异常！';
    }
    // result.extension = [
    //     { id: '1', clientId: "privilegeManager", displayName: "权限管理", logoutRedirectUri: "http://www.biying.com" },
    //     { id: '2', clientId: "xtwh", displayName: "基础数据维护", logoutRedirectUri: "http://www.biying.com" },
    //     { id: '3', clientId: "xtwh2", displayName: "房源管理", logoutRedirectUri: "http://www.baidu.com" }
    // ];
    if (result.isOk) {
        yield put({ type: actionUtils.getActionType(actionTypes.APP_LIST_UPDATE), payload: result.extension });
        yield put({ type: actionUtils.getActionType(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_UPDATE), payload: { type: 'app', extension: result.extension } });
    } else {
        notification.error({
            message: '应用',
            description: result.msg,
            duration: 3
        });
    }

}
//
export function* watchGetAppList() {
    yield takeLatest(actionUtils.getActionType(actionTypes.APP_LIST_GET), getAppListAsync)
}
//应用删除
export function* deleteAppAsync(state) {
    let result = { isOk: false, msg: '应用删除失败!' };
    let url = WebApiConfig.application.Base + '/' + state.payload;
    console.log("应用删除:", url);
    try {
        const appResult = yield call(ApiClient.post, url, null, null, 'DELETE');
        getApiResult(appResult, result);
        if (result.isOk = true) {
            result.msg = '应用删除成功!';
            yield put(actionUtils.action(appListGet, []));
        }
    } catch (e) {
        result.msg = '应用删除接口访问异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '应用',
        description: result.msg,
        duration: 3
    });
}
export function* watchAppDelete() {
    yield takeLatest(actionUtils.getActionType(actionTypes.APP_DELETE), deleteAppAsync);
}
//应用保存
export function* saveAppAsync(state) {
    let result = { isOk: false, msg: '应用保存失败!' }
    var url = WebApiConfig.application.Base;
    if (state.payload.method == 'PUT') {
        url += '/' + state.payload.data.id;
    }
    console.log(`保存工具：${url},method:${state.payload.method}`);
    console.log(state.payload.data);
    try {
        var appResult = yield call(ApiClient.post, url, state.payload.data, null, state.payload.method);
        console.log(appResult);
        getApiResult(appResult, result);
        if (result.isOk) {
            result.msg = "应用保存成功!";
            yield put(actionUtils.action(appListGet, []));
            yield put(actionUtils.action(appDialogClose, {}));
        }
    } catch (e) {
        result.msg = '应用保存接口调用失败!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '应用',
        description: result.msg,
        duration: 3
    });
}

export function* watchAppSave() {
    yield takeLatest(actionUtils.getActionType(actionTypes.APP_DATA_SAVE), saveAppAsync);
}