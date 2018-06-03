import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//获取交易合同
export function* searchPPFtDataAsync(state){
    let result = { isOk: false, extension: [], msg: '查询人员分摊表信息成功' };
    let url = WebApiConfig.fina.searchPPFt;
    try {
        console.log(url)
        console.log('searchPPFtDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchPPFtDataAsync返回成功:',result)
            yield put({ type: actionUtils.getActionType(actionTypes.FINA_QUERYPPFT_SUCCESS), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "查询人员分摊表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '查询人员分摊表信息失败!',
            duration: 3
        });
    }
}
export default function* watchAllFinaAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.FINA_QUERYPPFT), searchPPFtDataAsync);
}