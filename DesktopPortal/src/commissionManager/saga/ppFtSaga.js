import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webApiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getPPFTDataListByOrgIdAsyncs(state){
    let result = { isOk: false, msg: '根据组织id获取分摊项数据列表失败!' };
    console.log(state)
    //等待数据接口
    yield put({ type: actionUtils.getActionType(actionTypes.ORG_FT_PARAMLIST_UPDATE), payload: result });
}
//保存数据
export function* savePPFTDataAsync(state){
    let result = { isOk: false, msg: '保存分摊项数据失败!' };
    //等待数据接口
}
//删除数据
export function* delPPFTDataAsync(state){
    let result = { isOk: false, msg: '删除分摊项数据失败!' };
    //等待数据接口
}
export default function* watchAllPPftAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAMLIST_GET), getPPFTDataListByOrgIdAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAM_SAVE), savePPFTDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAM_DELETE),delPPFTDataAsync);
}