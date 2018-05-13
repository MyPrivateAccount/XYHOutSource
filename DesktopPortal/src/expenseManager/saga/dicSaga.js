import {takeEvery, takeLatest} from 'redux-saga';
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
    let result = { isOk: false, extension: [], msg: '字典获取失败！' };
    let url = WebApiConfig.dic.ParList;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_PARLIST_COMPLETE), payload: result.extension });
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

export default function* watchGetAlldic() {
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CHARGEDICINFO), getParListAsync);
}