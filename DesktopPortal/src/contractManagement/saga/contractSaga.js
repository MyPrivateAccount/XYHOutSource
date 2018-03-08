import { takeEvery, takeLatest, delay } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import { 
    basicLoadingEnd, gotoThisContractStart, gotoThisContractFinish
} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//合同基础信息保存
export function* saveContractBasicAsync(state) {
    console.log('state.payload.entity:', state.payload.entity);
    let result = { isOk: false, msg: '合同基础信息保存失败！' };
    // let url = WebApiConfig.buildingBasic.Base + "/" + state.payload.entity.id;
     let body = state.payload.entity;
    // let method = state.payload.method;
    // let city = state.payload.ownCity; // 自己所在城市
    try {
        // console.log(`楼盘基础信息保存url:${url},body:${JSON.stringify(body)}`);
        //const saveResult = yield call(ApiClient.post, url, body, null, "PUT");
        //getApiResult(saveResult, result);
        // console.log("保存结果", saveResult);
        //if (result.isOk) {
            result.msg = "合同基础信息保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.CONTRACT_BASIC_VIEW), /*payload: result.extension ||*/ body });
            //yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
       // }
        yield put(actionUtils.action(basicLoadingEnd))
    } catch (e) {
        result.msg = "合同基础信息保存接口调用异常!";
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


export function* watchContractAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.CONTRACT_BASIC_SAVE), saveContractBasicAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_SUPPORT_SAVE), saveSupportInfoAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_RELSHOP_SAVE), saveRelshopsAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PROJECT_SAVE), saveProjectAsync);
    // // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PICTURE_SAVE), savePictureAsync);
     yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_THIS_CONTRACT), gotoThisContract);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILD_INFO_SUBMIT), submitBuildInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BATCH_BUILDING_SAVE_ASYNC), saveBatchBuildingAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_PICTURE_ASYNC), savePictureAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_PICTURE_ASYNC), deletePicAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.COMMISSION_SAVE), saveCommissionInfo);
    // yield takeLatest(actionUtils.getActionType(actionTypes.RULES_SAVE), saveRulesInfo);
}