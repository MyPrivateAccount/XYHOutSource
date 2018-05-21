import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import { isNull } from 'util';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

function dealCondition(body){
    let newBody = {};
    for(let key in body){
        if(key === 'keyWord' || (body[key] !== '' && body[key] !== null)){
            newBody[key] = body[key];
        }
    }
    return newBody;
}
export function* getContractListAsync(state) {
    let result = { isOk: true, extension: [], msg: '合同查询失败！' };
    console.log('getContractListAsync:.......')
    let url = WebApiConfig.search.getContractList;
    
    let body = state.payload;
    let newBody = dealCondition(body);
    //newBody = {"keyWord": "","pageIndex":0,"pageSize":10};
    try {
        let res = yield call(ApiClient.post, url, newBody);
        console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)},newBody:${JSON.stringify(newBody)}`);
       getApiResult(res, result);
       if (result.isOk) {
            if (res.data.validityContractCount) {
                    result.msg = "合同查询成功！";
                    result.validityContractCount = res.data.validityContractCount;
            }
            if(state.payload.type === 'dialog'){
            
                // if(result.extension || result.extension.length === 0)
                // {
                //     notification.error({
                //         message: '没有甲方信息，请先在甲方管理中设置',
                //         description: result.msg,
                //         duration: 3
                //     });
                // }
          
                yield put({type: actionUtils.getActionType(actionTypes.OPEN_CONTRACT_CHOOSE), payload: result});
                
               
            }
            else{
                yield put({ type: actionUtils.getActionType(actionTypes.SEARCH_COMPLETE), payload: result });
            }
            
     }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = "合同查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}



export function* getAllExportDataAsync(action){
    let result = { isOk: true, extension: [], msg: '导出失败!' };
    let url = WebApiConfig.search.getContractList;
    
    
    let body = action.payload;
    let newBody = dealCondition(body);
  
    try {
        let res = yield call(ApiClient.post, url, newBody);
        console.log(`查询所有合同url:${url},result:${JSON.stringify(res)},newBody:${JSON.stringify(newBody)}`);
       getApiResult(res, result);
       yield put({ type: actionUtils.getActionType(actionTypes.BEGIN_EXPORT_ALL_DATA), payload: result });
    } catch (e) {
        result.msg = "合同查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}
export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_START), getContractListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ALL_EXPORT_DATA), getAllExportDataAsync);

}

