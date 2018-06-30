//月结saga
import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取月结月份接口
export function* getMonthlyDataAsyncs(state){
    let result = { isOk: false, msg: '获取月结月份数据成功!' };
    console.log(state)
    let url = WebApiConfig.yj.monthlyMonth+state.payload.branchId;
    try {
        console.log(url)
        console.log('getMonthlyDataAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.YJ_MONTH_GETUPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取月结月份数据接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取月结月份数据失败!'+result.msg,
            duration: 3
        });
    }
}
export default function* watchAllYjAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.YJ_MONTH_GET), getMonthlyDataAsyncs);
}