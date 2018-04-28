import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants'
import appUtils from '../../utils/appUtils'
import WebApiConfig from '../constants/webapiConfig';
import {notification} from 'antd';
import getApiResult from '../../utils/sagaUtil';

const actionUtils = appUtils(actionTypes.APPNAME)


//获取足迹列表
export function* getFootPrintListAsync(state) {
    let result = {isOk: false, extension: [], msg: '客户足迹列表获取失败！'};
    let url = WebApiConfig.footPrint.getList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({type: actionUtils.getActionType(actionTypes.FPRINT_GET_LIST_END), payload: result});
        }
    } catch (e) {
        result.msg = '获取客户足迹列表接口异常';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}



export default function* run() {
    yield takeLatest(actionUtils.getActionType(actionTypes.FPRINT_GET_LIST), getFootPrintListAsync);
}