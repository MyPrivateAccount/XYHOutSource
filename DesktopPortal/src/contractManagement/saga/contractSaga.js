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
} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//合同基础信息保存
export function* saveContractBasicAsync(state) {
    console.log('state.payload.entity:', state.payload.entity);
    let result = { isOk: false, msg: '合同基础信息保存失败！' };
     let url = WebApiConfig.contractBasic.Base
     let method = state.payload.method;
     let body = state.payload.entity;
     //body.follow = "1";
     body.relation = "1";
     const modifyId = NewGuid();
     let baseInfo = Object.assign({}, {baseInfo:body, modifyInfo: [{iD:modifyId, contractID:body.id}]});
    // let city = state.payload.ownCity; // 自己所在城市
    try {
        console.log(`合同基础信息保存url:${url},baseInfo:${JSON.stringify(baseInfo)}`);
        const saveResult = yield call(ApiClient.post, url, baseInfo, null, "POST");
        getApiResult(saveResult, result);
         console.log("保存结果", saveResult);
        if (result.isOk) {
            result.msg = "合同基础信息保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_BASIC_VIEW), payload: /*result.extension */baseInfo });
            //yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_CONTRACT), payload: city  });
        }
        yield put(actionUtils.action(basicLoadingEnd));
    } catch (e) {
        result.msg = "合同基础信息保存接口调用异常!";
        yield put(actionUtils.action(basicLoadingEnd));
    }
    
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

// 编辑楼盘
export function* gotoThisContract(action) {
    // console.log(action, '点击楼盘')
    let url = WebApiConfig.buildings.GetThisBuildings + action.payload.id;
    let res;
    yield put(actionUtils.action(gotoThisContractStart))
    try {
        res = yield call(ApiClient.get, url);
        yield put(actionUtils.action(gotoThisContractFinish, res));
        if (!action.payload.type) { // type==>dynamic 说明是在动态房源哪里掉的接口，不能进行页面跳转步骤
           // yield put(actionUtils.action(gotoChangeMyAdd));
           // yield put(actionUtils.action(changeShowGroup, {type: 2}));
        } else {
            //yield put(actionUtils.action(changeShowGroup, {type: 1}));
        }
    } catch (e) {
        yield put(actionUtils.action(gotoThisContractFinish, { code: '1', message: '失败' }));
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
    let result = { isOk: false, msg: '图片保存失败！' };
    console.log(action, '上传图片')
    let id = action.payload.id;
    let city = action.payload.ownCity; // 自己所在城市
    let url = WebApiConfig.buildingAttachInfo.PicUpload.replace("{dest}", action.payload.type) + id;
    try {
        let body = action.payload.fileInfo;
        let res = yield call(ApiClient.post, url, body);
        getApiResult(res, result);
        console.log(`上传图片url:${url},body:${JSON.stringify(body)}，result：${res}`);
        console.log(res, body, 'res')
        if (result.isOk) {
            result.msg = '图片保存成功！';
            yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_PIC_VIEW), payload: { filelist: action.payload.completeFileList, type: 'add' } });
            //这个地方关联合同信息使其可以成为一个完整的合同信息
            //yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        yield put(actionUtils.action(attchLoadingEnd))
    } catch (e) {
        result.msg = '图片保存失败！';
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
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_SUPPORT_SAVE), saveSupportInfoAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_RELSHOP_SAVE), saveRelshopsAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PROJECT_SAVE), saveProjectAsync);
    // // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PICTURE_SAVE), savePictureAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_THIS_CONTRACT), gotoThisContract);
     yield takeLatest(actionUtils.getActionType(actionTypes.CONTRACT_INFO_SUBMIT), submitContractInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BATCH_BUILDING_SAVE_ASYNC), saveBatchBuildingAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_PICTURE_ASYNC), savePictureAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_PICTURE_ASYNC), deletePicAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.COMMISSION_SAVE), saveCommissionInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.RULES_SAVE), saveRulesInfo);
}