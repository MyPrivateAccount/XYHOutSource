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

            yield put({ type: actionUtils.getActionType(actionTypes.MINUS_USER_BREAD), payload: {} });
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

export function* getcreateStation(state) {
    let url = WebApiConfig.search.getStationList+"/"+state.payload;
    let huResult = { isOk: false, msg: '获取职位失败！' };

    try {
        huResult = yield call(ApiClient.get, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '获取职位成功';
            yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        }
    } catch (e) {
        huResult.data.message = "获取职位接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setStation(state) {
    let url = WebApiConfig.server.SetStation;
    let huResult = { isOk: false, msg: '设置职位失败！' };

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '设置职位成功';
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            yield put({ type: actionUtils.getActionType(actionTypes.MINUS_USER_BREAD), payload: {} });
            return;
            //yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        }
    } catch (e) {
        huResult.data.message = "设置职位接口调用异常!";
    }
    
    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* deleteStation(state) {
    let url = WebApiConfig.server.DeleteStation;
    let huResult = { isOk: false, msg: '删除职位失败！' };

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除职位成功';
            //yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        }
    } catch (e) {
        huResult.data.message = "删除职位接口调用异常!";
    }
    
    if (huResult.data.code !== 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setSalary(state) {
    let url = WebApiConfig.server.setSalary;
    let huResult = { isOk: false, msg: '设置薪酬失败！' };

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '设置薪酬成功';

            yield put({ type: actionUtils.getActionType(actionTypes.MINUS_USER_BREAD), payload: {} });
            notification.success({
                message: "设置成功",
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "设置薪酬接口调用异常!";
    }
    
    if (huResult.data.code !== 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* deleteSalary(state) {
    let url = WebApiConfig.server.deleteSalary;
    let huResult = { isOk: false, msg: '删除薪酬失败！' };

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除薪酬成功';

            notification.success({
                message: "删除成功",
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除薪酬接口调用异常!";
    }
    
    if (huResult.data.code !== 0) {
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
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CRATESTATION), getcreateStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.SET_STATION), setStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_STATION), deleteStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.SET_SALARYINFO), setSalary);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_SALARYINFO), deleteSalary);
}