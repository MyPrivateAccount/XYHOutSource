import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import getApiResult from './sagaUtil';
import {notification} from 'antd';


export function* getContractDetail(state){
    let result = {isOk: false, extension: null, msg: '获取合同详细信息失败!'};
    let url = WebApiConfig.contract.modifyDetail + state.payload;
    try{
        let res = yield call(ApiClient.get, url)
        getApiResult(res, result);
        console.log(`当前合同的信息url：${url}, result:${JSON.stringify(res)}`);
        if(result.isOk){
            if(result.extension){
                //let info = JSON.parse(result.extension.ext1);
               // result.extension.ext1 = info;
            }
            console.log('当前获取的合同信息为', result.extension);
            yield put({type: actionTypes.GET_CONTRACT_DETAIL_COMPLETE, payload: result.extension});
        }
        else{
            yield put({type: actionTypes.GET_CONTRACT_DETAIL_COMPLETE, payload: null});
        }
       
       yield put({type: actionTypes.SET_SEARCH_LOADING, payload: false});
    }catch(e){
        result.msg = '获取合同详细信息接口异常!';
    }
    if(!result.isOk){
        notification.error({
            description: result.msg,
            duration:3
        });
    }
}

export default function* watchContractAsync() {

    yield takeLatest(actionTypes.GET_CONTRACT_DETAIL, getContractDetail);
}

