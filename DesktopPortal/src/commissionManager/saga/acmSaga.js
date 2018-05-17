import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//获取列表数据
export function* getAcmentDataListAsyncs(state){
    let result = { isOk: false, msg: '获取业绩分摊列表数据失败!' };
    console.log(state)
    //等待数据接口
    yield put({ type: actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_UPDATE), payload: result });
}
//保存数据
export function* saveAcmentDataAsync(state){
    let result = { isOk: false, msg: '保存业绩分摊数据失败!' };
    //等待数据接口
}
//删除数据
export function* delAcmentDataAsync(state){
    let result = { isOk: false, msg: '删除业绩分摊数据失败!' };
    //等待数据接口
}

export default function* watchAllAcmentAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_LIST_GET), getAcmentDataListAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_SAVE), saveAcmentDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ACMENT_PARAM_DEL), delAcmentDataAsync);

}