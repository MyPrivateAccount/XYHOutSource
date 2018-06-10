import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';
import WebApiConfig from '../constants/webApiConfig';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import ApiClient from '../../utils/apiClient'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getScaleDataListAsyncs(state){
    let result = { isOk: false, msg: '获取提成比例列表数据失败!' };
    console.log(state)
    //等待数据接口
    yield put({ type: actionUtils.getActionType(actionTypes.INCOME_SCALE_LIST_UPDATE), payload: result });
}
//保存数据
export function* saveScaleDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '保存提成比例设置成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.incomesave;
    try {
        console.log(url)
        console.log('saveScaleDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.INCOME_SCALE_SAVE_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存提成比例设置接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '保存提成比例设置失败!',
            duration: 3
        });
    }
}
//删除数据
export function* delScaleDataAsync(state){
    let result = { isOk: false, msg: '删除提成比例数据失败!' };
    //等待数据接口
}

export default function* watchAllScaleAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_LIST_GET), getScaleDataListAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_SAVE), saveScaleDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.INCOME_SCALE_DEL), delScaleDataAsync);

}