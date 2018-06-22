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
    let result = { isOk: false, msg: '获取人数分摊组织参数列表数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.ppftsave+state.payload.branchId;
    try {
        console.log(url)
        console.log('getPPFTDataListByOrgIdAsyncs:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_FT_PARAMLIST_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取人数分摊组织参数列表接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取人数分摊组织参数列表失败!',
            duration: 3
        });
    }
}
//保存数据
export function* savePPFTDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '保存组织分摊设置成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.ppftsave;
    try {
        console.log(url)
        console.log('savePPFTDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_FT_PARAM_SAVE_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存组织分摊接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '保存组织分摊失败!',
            duration: 3
        });
    }
}
//删除数据
export function* delPPFTDataAsync(state){
    //等待数据接口
    let result = { isOk: false, msg: '删除人数分摊组织列表数据成功!' };
    console.log(state)
    let url = WebApiConfig.baseset.ppftsave+state.payload.branchId+'/'+state.payload.shareId;
    try {
        console.log(url)
        console.log('delAcmentDataAsync:', state);
        let res = yield call(ApiClient.del, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_FT_PARAM_DELETE_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "删除人数分摊组织列表数据异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '删除人数分摊组织列表信息失败!',
            duration: 3
        });
    }
}
export default function* watchAllPPftAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAMLIST_GET), getPPFTDataListByOrgIdAsyncs);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAM_SAVE), savePPFTDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_FT_PARAM_DELETE),delPPFTDataAsync);
}