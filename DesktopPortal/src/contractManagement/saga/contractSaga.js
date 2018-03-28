import { takeEvery, takeLatest, delay } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction , { NewGuid }from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import { 
    basicLoadingEnd, gotoThisContractStart, gotoThisContractFinish,
    submitContractStart,submitContractFinish,attchLoadingStart, attchLoadingEnd,
    openAttachMentStart, openAttachMentFinish,
} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//合同基础信息保存
export function* saveContractBasicAsync(state) {
    console.log('state.payload.entity:', state.payload.entity);
    let result = { isOk: false, msg: '合同基础信息提交失败！' };
    let method = state.payload.method;
    let url = method === 'POST' ?  WebApiConfig.contractBasic.Base : WebApiConfig.contractBasic.Modify;
     
    let body = state.payload.entity;
     //body.follow = "1";
     body.relation = 1;
    let baseInfo = Object.assign({}, {baseInfo:body /*, modifyInfo:[{iD:modifyId, contractID:body.id}]*/});
    try {
        console.log(`合同基础信息提交url:${url},baseInfo:${JSON.stringify(baseInfo)}`);
        const saveResult = yield call(ApiClient.post, url, baseInfo, null, "POST");
        getApiResult(saveResult, result);
         console.log("保存结果", result);
        if (result.isOk) {
            result.msg = "合同基础信息提交成功";
           // yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_BASIC_VIEW), payload: {baseInfo: result.extension || baseInfo} });
            yield put({ type: actionUtils.getActionType(actionTypes.BASIC_SUBMIT_END) });
            yield put({ type: actionUtils.getActionType(actionTypes.CLOSE_RECORD)});
        }
        //yield put(actionUtils.action(basicLoadingEnd));
    } catch (e) {
        result.msg = "合同基础信息提交接口调用异常!";
        //yield put(actionUtils.action(basicLoadingEnd));
    }
    
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

//获取合同详情
export function* gotoThisContract(action) {

    let result = { isOk: false, msg: '获取合同详情失败！' };
    let id  = action.payload.record.id;
    let url = WebApiConfig.contract.GetContractInfo + id;
    let res;
    //
    try {
        res = yield call(ApiClient.get, url);
        getApiResult(res, result);
       if (result.isOk) {
            result.msg = "获取合同详情成功";
            
            yield put({ type: actionUtils.getActionType(actionTypes.GOTO_THIS_CONTRACT_START), payload: {id:0}});
            console.log('获取合同详情结果:', JSON.stringify(res));
            yield put(actionUtils.action(gotoThisContractFinish, res));
       }

    } catch (e) {
       // yield put(actionUtils.action(gotoThisContractFinish, { code: '1', message: '失败' }));
    }
    console.log("resultppp:", result);
    if(result.isOk === false)
    {
        notification[ 'error']({
            message: result.msg,
            duration: 3
        });
    }

}

export function* openAttachUpload(action){
    let url = WebApiConfig.contract.GetContractInfo + action.payload.record.id;
    let baseInfo = action.payload.record;
    let res;
    yield put(actionUtils.action(openAttachMentStart))
    try {
       
        res = yield call(ApiClient.get, url);
        let fileList = [];
        if (res.data.code === '0') {
            // yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) });
            if(res.data.extension && res.data.extension.fileList)
            {
                fileList = res.data.extension;
            }
         }
        
        yield put(actionUtils.action(openAttachMentFinish, {baseInfo: res.data.extension.baseInfo, fileList:fileList, code: '0'}));

    } catch (e) {
        yield put(actionUtils.action(openAttachMentFinish, { code: '1', message: '失败' }));
    }
}

// 提交楼盘
export function* submitContractInfo(action) {
    let body = action.payload.entity;
    let url = WebApiConfig.contractBasic.Submit;
    console.log("submit action:" , action);
    let submitBody = {ContractID:body.id, ModifyId:body.modifyInfo[0].iD, CheckName:"sssss", Action:"TEST" };
    let res;
    // console.log(url, 'aaaaaaaaaaa')
    console.log(`合同提交url:${url},submitBody:${JSON.stringify(submitBody)}`);
    yield put(actionUtils.action(submitContractStart));
    try {
        res = yield call(ApiClient.post, url, submitBody);
        console.log("提交合同:", res);
        if (res.data.code === '0') {
           // yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) });
        }
        notification[res.data.code === "0" ? 'success' : 'error']({
            message: res.data.code === "0" ? '提交成功，请等待审核' : '提交失败',
            duration: 3
        })
        yield put(actionUtils.action(submitContractFinish));
    } catch (e) {
        yield put(actionUtils.action(submitContractFinish, { code: '1', message: '提交失败', entity: body }));
    }
}

// 上传图片
export function* savePictureAsync(action) {
    let result = { isOk: false, msg: '图片提交失败！' };
    //console.log(action, '上传图片')
    let id = action.payload.id;
    
    let url = WebApiConfig.attach.savePicUrl+ id;
   
    try {
        let body = action.payload.fileInfo;
        //console.log(`上传图片url:${url},body:${JSON.stringify(body)}`);
        // yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_PIC_VIEW), payload: { filelist: action.payload.completeFileList, type: 'add' } });
        //     yield put(actionUtils.action(attchLoadingEnd));
        // return;
        let res = yield call(ApiClient.post, url, body);
        getApiResult(res, result);
        console.log(`上传图片url:${url},body:${JSON.stringify(body)}，result：${res}`);
        console.log(res, body, 'res')
        if (result.isOk) {
            result.msg = '图片提交成功！';
           // yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_PIC_VIEW), payload: { filelist: action.payload.completeFileList, type: 'save' } });
            yield put({ type: actionUtils.getActionType(actionTypes.ATTACH_SUBMIT_END) });
            yield put({ type: actionUtils.getActionType(actionTypes.CLOSE_RECORD)});
            //这个地方关联合同信息使其可以成为一个完整的合同信息
            //yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        //yield put(actionUtils.action(attchLoadingEnd))
    } catch (e) {
        result.msg = '图片提交失败！';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

// 图片删除
export function* deletePicAsync(action) {
    let url, res,
        body = action.payload.fileInfo;
    let city = action.payload.ownCity; // 自己所在城市
    try {

            url = WebApiConfig.shopsAttachInfo.PicDelete + action.payload.id
            res = yield call(ApiClient.post, url, body)
            if (res.data.code === '0') {
                yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_PIC_VIEW), payload: { filelist: action.payload.deletePicList, type: 'delete' } });
                notification.success({
                    message: '图片删除成功！',
                    duration: 3
                });
                //yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
            }
            yield put(actionUtils.action(attchLoadingEnd))
        
    } catch (e) {
        notification.error({
            message: '图片删除失败！',
            duration: 3
        });
    }
}

export function* watchContractAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.CONTRACT_BASIC_SAVE), saveContractBasicAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.OPEN_ATTACHMENT), openAttachUpload);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_SUPPORT_SAVE), saveSupportInfoAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_RELSHOP_SAVE), saveRelshopsAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PROJECT_SAVE), saveProjectAsync);
    // // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PICTURE_SAVE), savePictureAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_THIS_CONTRACT), gotoThisContract);
     yield takeLatest(actionUtils.getActionType(actionTypes.CONTRACT_INFO_SUBMIT), submitContractInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BATCH_BUILDING_SAVE_ASYNC), saveBatchBuildingAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.CONTRACT_SAVE_PICTURE_ASYNC), savePictureAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_PICTURE_ASYNC), deletePicAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.COMMISSION_SAVE), saveCommissionInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.RULES_SAVE), saveRulesInfo);
}