import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import NwfCommand from '../constants/commandDefine';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

export function* excuteNWFCommandAsync(state) {
    let result = { isOk: false, extension: [], msg: '流程操作失败！' };
    let url = WebApiConfig.flowChart.Invoke;
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        //console.log(`执行nwf命令，url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.EXECUTE_NWF_COMMAND_COMPLETE), payload: { command: state.payload.CommandName, extension: res.data } });
            if (state.payload.CommandName === NwfCommand.ImportWorkFLow.CommandName
                || state.payload.CommandName === NwfCommand.DeleteWorkFlow.CommandName
                || state.payload.CommandName === NwfCommand.SaveWorkflow.CommandName) {
                //删除和导入刷新流程列表
                yield put({ type: actionUtils.getActionType(actionTypes.EXECUTE_NWF_COMMAND), payload: NwfCommand.GetWorkflowList });
            }
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_LOADING), payload: false });
    } catch (e) {
        result.msg = "流程接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export default function* watchAllSearchAsync() {
    yield takeEvery(actionUtils.getActionType(actionTypes.EXECUTE_NWF_COMMAND), excuteNWFCommandAsync);
}

