import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* postHumanInfoAsync(state) {
    let urlhuman = WebApiConfig.server.PostHumaninfo;

    let humanResult = { isOk: false, msg: '人事信息提交失败！' };

    try {
        humanResult = yield call(ApiClient.post, urlhuman, state.payload.humaninfo, state.payload.fileinfo);

        //弹消息，返回
        if (humanResult.data.code == 0) {
            humanResult.isOk = true;
            humanResult.message = '人事信息提交成功';

            yield put({ type: actionUtils.getActionType(actionTypes.CLOSE_USER_BREAD), payload: {} });
        }
    } catch (e) {
        humanResult.msg = "部门用户获取接口调用异常!";
    }
    
    notification[humanResult.isOk ? 'success' : 'error']({
        message: humanResult.msg,
        duration: 3
    });
}

export function* getWorkNumber(state) {
    let url = WebApiConfig.server.GetWorkNumber;
    let huResult = { isOk: false, msg: '获取工号失败！' };
    try {
        huResult = yield call(ApiClient.get, url);
        //弹消息，返回
        if (huResult.isOk) {
            huResult.message = '人事信息提交成功';

            yield put({ type: actionUtils.getActionType(actionTypes.SET_HUMANINFONUMBER), payload: {worknumber:huResult} });
        }
    } catch (e) {
        huResult.msg = "部门用户获取接口调用异常!";
    }
    
    if (!huResult.isOk) {
        notification.error({
            message: huResult.msg,
            duration: 3
        });
    }
}

export function* recoverMonth(state) {
    let url = WebApiConfig.server.RecoverMonth;
    let huResult = { isOk: false, msg: '恢复月结失败！' };
    try {
        huResult = yield call(ApiClient.post, url, state.payload.last);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '恢复月结成功';

            yield put({ type: actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), payload: state.payload.result});
        }
    } catch (e) {
        huResult.data.message = "恢复月结接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* createMonth(state) {
    let url = WebApiConfig.server.CreateMonth;
    let huResult = { isOk: false, msg: '创建月结失败！' };
    try {
        huResult = yield call(ApiClient.post, url, state.payload.last);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '创建月结成功';

            yield put({ type: actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST) , payload: state.payload.result});
        }
    } catch (e) {
        huResult.data.message = "创建月结接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setBlackLst(state) {
    let url = WebApiConfig.server.SetBlacklst;
    let huResult = { isOk: false, msg: '创建黑名单失败！' };
    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '创建黑名单成功';
            //yield put({ type: actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), payload: state.payload.result});
        }
    } catch (e) {
        huResult.data.message = "创建黑名单接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* createStation(state) {
    let url = WebApiConfig.dic.permissionOrg;
    let huResult = { isOk: false, msg: '创建职位失败！' };

    try {
        
    } catch (e) {
        huResult.data.message = "创建职位接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export default function* watchDicAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_HUMANINFO), postHumanInfoAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_HUMANINFONUMBER), getWorkNumber);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_RECOVER), recoverMonth);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_CREATE), createMonth);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_ADDBLACKLST), setBlackLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_CRATESTATION), createStation);
}