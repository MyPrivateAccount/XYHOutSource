import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getOrgParamDataListByOrgIdAsyncs(state){
    let result = { isOk: false, msg: '根据组织id获取组织参数列表失败!' };
    console.log(state)
    //等待数据接口
    yield put({ type: actionUtils.getActionType(actionTypes.ORG_PARAMLIST_UPDATE), payload: result });
}
//保存数据
export function* saveOrgParamDataAsync(state){
    let result = { isOk: false, msg: '保存组织参数数据失败!' };
    //等待数据接口
}

export default function* watchAllOrgParamAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_PARAMLIST_GET), getOrgParamDataListByOrgIdAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_PARAM_SAVE), saveOrgParamDataAsync);
}