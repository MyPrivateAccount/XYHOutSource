import { takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import * as actionTypes from '../../constants/actionType'
import appAction from '../../../utils/appUtils'
import WebApiConfig from '../../constants/webApiConfig'
import getApiResult from '../sagaUtil'
import ApiClient from '../../../utils/apiClient'
import { notification } from 'antd'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

//保存报告基础信息
export function* saveRpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告交易合同失败！' };
    let url = WebApiConfig.rp.rpAdd;
    try {
        console.log(url)
        console.log('saveRpDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_RP_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告交易合同接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告交易合同信息失败!',
            duration: 3
        });
    }
}
//保存物业
export function* saveRpWyDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告物业信息失败！' };
    let url = WebApiConfig.rp.rpWyAdd;
    try {
        console.log(url)
        console.log('saveRpWyDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_WY_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告物业接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告物业信息失败',
            duration: 3
        });
    }
}
//保存业主
export function* saveRpYzDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告业主信息失败！' };
    let url = WebApiConfig.rp.rpYzAdd;
    try {
        console.log(url)
        console.log('saveRpYzDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告业主接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告业主信息失败!',
            duration: 3
        });
    }
}
//保存业主
export function* saveRpKhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告客户信息失败！' };
    let url = WebApiConfig.rp.rpKhAdd;
    try {
        console.log(url)
        console.log('saveRpKhDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告客户接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告客户信息失败!',
            duration: 3
        });
    }
}
//保存过户
export function* saveRpGhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告过户信息失败！' };
    let url = WebApiConfig.rp.rpGhAdd;
    try {
        console.log(url)
        console.log('saveRpGhDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_GH_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告过户接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告过户信息失败!',
            duration: 3
        });
    }
}
//保存业绩分配
export function* saveRpFpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存成交报告业绩分配信息失败！' };
    let url = WebApiConfig.rp.rpFpAdd;
    try {
        console.log(url)
        console.log('saveRpFpDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FP_SAVEUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存成交报告业绩分配接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存成交报告业绩分配信息失败!',
            duration: 3
        });
    }
}
//获取交易合同
export function* getRpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告交易合同信息失败！' };
    let url = WebApiConfig.rp.rpGet+state.payload;
    try {
        console.log(url)
        console.log('getRpDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_RP_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告交易合同接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告交易合同信息失败!',
            duration: 3
        });
    }
}
//获取物业信息
export function* getRpWyDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告物业信息成功！' };
    let url = WebApiConfig.rp.wyGet+state.payload;
    try {
        console.log(url)
        console.log('getRpWyDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_WY_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告物业接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告物业信息失败!',
            duration: 3
        });
    }
}
//获取业主信息
export function* getRpYzDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告业主信息失败！' };
    let url = WebApiConfig.rp.yzGet+state.payload;
    try {
        console.log(url)
        console.log('getRpYzDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_YZ_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告业主接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告业主信息失败!',
            duration: 3
        });
    }
}
//获取客户信息
export function* getRpKhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告客户信息失败！' };
    let url = WebApiConfig.rp.khGet+state.payload;
    try {
        console.log(url)
        console.log('getRpKhDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_KH_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告客户信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告客户信息失败!',
            duration: 3
        });
    }
}
//获取过户信息
export function* getRpGhDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告过户信息失败！' };
    let url = WebApiConfig.rp.ghGet+state.payload;
    try {
        console.log(url)
        console.log('getRpGhDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_GH_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告过户信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告过户信息失败!',
            duration: 3
        });
    }
}
//获取业绩分配信息
export function* getRpFpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报告业绩分配信息失败！' };
    let url = WebApiConfig.rp.fpGet+state.payload;
    try {
        console.log(url)
        console.log('getRpFpDataAsync:', state);
        let res = yield call(ApiClient.get, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FP_GETUPDATE), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报告过户信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报告过户信息失败!',
            duration: 3
        });
    }
}
///上传文件
export function* uploadFileAsync(state) {
    let result = { isOk: false, extension: [], msg: '文件上传失败！' };
    let url = WebApiConfig.server.uploadImg + state.payload.sourceId+'/upload/' + state.payload.fileGuid;

    try {
        let res = yield call(ApiClient.post, url, state.payload)
        getApiResult(res, result);
         if (result.isOk) {
             console.log("uploadFile成功:"+result)
             yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_ATTACT_UPLOAD_COMPLETE), payload: result.extension });
        }
    } catch (e) {
        result.msg = "文件上传接口调用异常！";
    }

    if (!result.isOk) {
        notification.error({
            message: '文件上传',
            description: result.msg,
            duration: 3
        });
    }
}
////
//获取交易合同
export function* getMyRpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取我的成交报告列表信息成功！' };
    let url = WebApiConfig.rp.myrpGet;
    try {
        console.log(url)
        console.log('getMyRpDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('getMyRpDataAsync返回成功:',result)
            let data = result.extension;
            for(let i=0;i<data.length;i++){
                if(data[i].cjrq!==null){
                    var newdt = ''+data[i].cjrq;
                    if(newdt.indexOf('T')!==-1){
                        newdt = newdt.substr(0,newdt.length-9);
                        data[i].cjrq = newdt
                    }
                }
                if(data[i].examineStatus === 0){
                    data[i].examineStatus = '未提交'
                }
                else if(data[i].examineStatus === 1){
                    data[i].examineStatus = '审核中'
                }
                else if(data[i].examineStatus === 8){
                    data[i].examineStatus = '审核通过'
                }
                else if(data[i].examineStatus === 16){
                    data[i].examineStatus = '审核驳回'
                }
                else{
                    data[i].examineStatus = '未知'
                }
            }
            result.extension = data;
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_MYREPORT_GETUPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取我的成交报告列表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取我的成交报告列表信息失败!',
            duration: 3
        });
    }
}
//获取交易合同
export function* searchRpDataAsync(state){
    let result = { isOk: false, extension: [], msg: '搜索成交报告列表信息成功！' };
    let url = WebApiConfig.rp.searchRp;
    try {
        console.log(url)
        console.log('searchRpDataAsync:', state);
        let res = yield call(ApiClient.post, url,state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('searchRpDataAsync返回成功:',result)
            let data = result.extension;
            for(let i=0;i<data.length;i++){
                if(data[i].cjrq!==null){
                    var newdt = ''+data[i].cjrq;
                    if(newdt.indexOf('T')!==-1){
                        newdt = newdt.substr(0,newdt.length-9);
                        data[i].cjrq = newdt
                    }
                }
                if(data[i].examineStatus === 0){
                    data[i].examineStatus = '未提交'
                }
                else if(data[i].examineStatus === 1){
                    data[i].examineStatus = '审核中'
                }
                else if(data[i].examineStatus === 8){
                    data[i].examineStatus = '审核通过'
                }
                else if(data[i].examineStatus === 16){
                    data[i].examineStatus = '审核驳回'
                }
                else{
                    data[i].examineStatus = '未知'
                }
            }
            result.extension = data;
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_REPORT_SEARCH_UPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "搜索成交报告列表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '搜索成交报告列表信息失败!',
            duration: 3
        });
    }
}
//获取成交报备数据表
export function* getCjbbDataAsync(state){
    let result = { isOk: false, extension: [], msg: '获取成交报备列表信息成功！' };
    let url = WebApiConfig.rp.getcjbb;
    try {
        console.log(url)
        console.log('getCjbbDataAsync:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            console.log('getCjbbDataAsync返回成功:',result)
            let data = result.extension;
            result.extension = data;
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_CJBB_LISTUPDATE), payload: result });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "获取成交报备列表信息异常!";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '获取成交报备列表信息失败!',
            duration: 3
        });
    }
}
//根据成交报告编号获取实收付信息
export function* getFactgetDataAsync(state){
    let result = { isOk: false, extension: [], msg: '根据成交报告编号获取实收付信息成功！' };
    let url = WebApiConfig.rp.factget+state.payload;
    try {
        console.log(url)
        console.log('getFactgetDataAsync:', state);
        let res = yield call(ApiClient.get, url);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FACTGET_SUCCESS), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "根据成交报告编号获取实收付信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '提示',
            description: '根据成交报告编号获取实收付信息失败!',
            duration: 3
        });
    }
}
//保存收款信息
export function* saveRpSKDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存收款信息成功！' };
    let url = WebApiConfig.rp.factget;
    try {
        console.log(url)
        console.log('saveRpSKDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FACTGET_GET_SAVE_SUCCESS), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存收款信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存收款信息失败!',
            duration: 3
        });
    }
}
//保存付款信息
export function* saveRpFKDataAsync(state){
    let result = { isOk: false, extension: [], msg: '保存付款信息失败！' };
    let url = WebApiConfig.rp.factget;
    try {
        console.log(url)
        console.log('saveRpFKDataAsync:', state);
        let res = yield call(ApiClient.put, url, state.payload);
       
        //console.log(res, '获取参数列表');
        getApiResult(res, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DEALRP_FACTGET_PAY_SAVE_SUCCESS), payload: result.extension });
            // yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
    } catch (e) {
        result.msg = "保存付款信息接口调用异常！";
    }
    if (!result.isOk) {
        console.log(result.msg)
        notification.error({
            message: '系统参数',
            description: '保存付款信息失败!',
            duration: 3
        });
    }
}
export default function* watchAllRpAsync(){
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_RP_SAVE), saveRpDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_WY_SAVE), saveRpWyDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_YZ_SAVE), saveRpYzDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_KH_SAVE), saveRpKhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_GH_SAVE), saveRpGhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FP_SAVE), saveRpFpDataAsync);

    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_RP_GET), getRpDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_WY_GET), getRpWyDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_YZ_GET), getRpYzDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_KH_GET), getRpKhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_GH_GET), getRpGhDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FP_GET), getRpFpDataAsync);

    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_ATTACT_UPLOADFILE), uploadFileAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_MYREPORT_GET), getMyRpDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_REPORT_SEARCH), searchRpDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_CJBB_GET), getCjbbDataAsync);

    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FACTGET), getFactgetDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FACTGET_GET_SAVE), saveRpSKDataAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DEALRP_FACTGET_PAY_SAVE), saveRpFKDataAsync);
}