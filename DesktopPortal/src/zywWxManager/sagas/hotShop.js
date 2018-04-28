import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants'
import {notification} from 'antd';
import appUtils from '../../utils/appUtils'
import WebApiConfig from '../constants/webapiConfig';
import getApiResult from '../../utils/sagaUtil';

const actionUtils = appUtils(actionTypes.APPNAME)

//获取区域列表
export function* searchBuildingListAsync(action) {
    let result = {isOk: false, extension: [], msg: '楼盘列表获取失败！'};
    let url = WebApiConfig.hotShops.searchBuilding;
    try {
        let res = yield call(ApiClient.post, url, action.payload);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.SEARCH_BUILDING_END), payload: result});
        }
    } catch (e) {
        result.msg = '查询楼盘列表接口异常';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export default function* run() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_BUILDING), searchBuildingListAsync);
}