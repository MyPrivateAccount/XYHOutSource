import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webApiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

///获取字典列表
export function* getParListAsync(state) {
    let result = { isOk: false, extension: [], msg: '系统参数获取失败！' };
    let url = WebApiConfig.dic.ParList;
    try {
        console.log('getParListAsync:', state);
        let res = yield call(ApiClient.post, url, state.payload);
       
        //console.log(res, '获取参数列表');
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




export default function* watchDicAllAsync() {
    //这个只是获取最新的一个，使用时切忌在组件嵌套的情况且需要同时加载的时不要在componentWillMount中同时获取这样会导致前一次的执行被取消
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_PARLIST), getParListAsync)
    //yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_AREA), getAllAreaAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_LIST), getAllChildOrgsAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ALL_ORG_LIST), getAllOrgsAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_ORG_DETAIL), getUserOrgAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.GET_ORG_USERLIST), getOrgUserListAsync);
}

