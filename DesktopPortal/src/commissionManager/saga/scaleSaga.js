import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

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
    let result = { isOk: false, msg: '保存提成比例数据失败!' };
    //等待数据接口
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