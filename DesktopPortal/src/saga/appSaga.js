import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../utils/apiClient'
import * as actionTypes from '../constants/actionTypes';
import { ApplicationTypes } from '../constants/baseConfig';
import WebApiConfig from '../utils/webapiConfig';
import { notification } from 'antd';


//获取应用列表
export function* getAppListAsync() {
    let result = { isOk: false, extension: [], msg: '应用列表获取失败！' };
    let url = WebApiConfig.application.List;
    console.log(`获取工具列表：${url}`);
    try {
        let appResult = yield call(ApiClient.post, url, { applicationTypes: [ApplicationTypes[0].key] });
        console.log(appResult);
        if (appResult && appResult.data.code == '0') {
            result.isOk = true;
            result.extension = appResult.data.extension;
        }
        if (appResult.data.code == '403') {
            result.msg = '当前用户无权限!';
        }
    } catch (e) {
        result.msg = '应用获取接口访问异常！';
    }
    // result.extension = [
    //     { clientId: "privilegeManager", displayName: "权限管理", LogoutRedirectUri: "http://www.biying.com" },
    //     { clientId: "xtwh", displayName: "基础数据维护", LogoutRedirectUri: "http://www.biying.com" },
    //     { clientId: "houseSource", displayName: "房源管理", LogoutRedirectUri: "http://www.baidu.com" },
    //     { clientId: "searchTool", displayName: "房源查询", LogoutRedirectUri: "http://www.baidu.com" }
    // ];
    if (!result.isOk) {
        notification.error({
            message: '应用',
            description: result.msg,
            duration: 3
        });
    }
    yield put({ type: actionTypes.APP_LIST_UPDATE, payload: result.extension });
}
//
export function* watchGetAppList() {
    yield takeLatest(actionTypes.APP_LIST_GET, getAppListAsync)
}