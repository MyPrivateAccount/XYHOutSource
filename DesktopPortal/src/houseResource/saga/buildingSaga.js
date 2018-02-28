import { takeEvery, takeLatest, delay } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import { gotoThisBuildStart, gotoThisBuildFinish, changeShowGroup,
    gotoChangeMyAdd, submitBuildInfoFinish,
    submitBuildInfoStart, uploadPicFinish ,
    basicLoadingEnd, batchBuildLoadingEnd,
    supportLoadingEnd, relShopLoadingEnd,
    projectLoadingEnd, ruleLoadingEnd,
    ruleTemplateLoadingEnd, commissionLoadingEnd,
    attchLoadingEnd
} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);
//楼盘基础信息保存
export function* saveBuildingBasicAsync(state) {
    let result = { isOk: false, msg: '楼盘基础信息保存失败！' };
    let url = WebApiConfig.buildingBasic.Base + "/" + state.payload.entity.id;
    let body = state.payload.entity;
    let method = state.payload.method;
    let city = state.payload.ownCity; // 自己所在城市
    try {
        // console.log(`楼盘基础信息保存url:${url},body:${JSON.stringify(body)}`);
        const saveResult = yield call(ApiClient.post, url, body, null, "PUT");
        getApiResult(saveResult, result);
        // console.log("保存结果", saveResult);
        if (result.isOk) {
            result.msg = "楼盘基础信息保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_BASIC_VIEW), payload: result.extension || body });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        yield put(actionUtils.action(basicLoadingEnd))
    } catch (e) {
        result.msg = "楼盘基础信息保存接口调用异常!";
    }
    
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}


//保存配套信息
export function* saveSupportInfoAsync(state) {
    let result = { isOk: false, msg: '楼盘配套信息保存失败！' };
    let url = WebApiConfig.buildingSupport.Base + "/" + state.payload.entity.id;
    let body = state.payload.entity;
    let method = state.payload.method;
    let city = state.payload.ownCity; // 自己所在城市
    try {
        const saveResult = yield call(ApiClient.post, url, body, null, "PUT");
        // console.log(`url:${url},body:${body},楼盘配套信息保存结果：${JSON.stringify(saveResult)}`)
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = "楼盘配套信息保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_SUPPORT_VIEW), payload: result.extension || state.payload.entity });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        yield put(actionUtils.action(supportLoadingEnd))
    } catch (e) {
        result.msg = "楼盘配套信息保存接口调用异常!";
    }
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

//楼盘商铺信息保存
export function* saveRelshopsAsync(state) {
    let result = { isOk: false, msg: '楼盘商铺整体概况保存失败！' };
    let url = WebApiConfig.buildingRelshop.Base + "/" + state.payload.entity.id;
    let body = state.payload.entity;
    let method = state.payload.method;
    let city = state.payload.ownCity; // 自己所在城市
    try {
        const saveResult = yield call(ApiClient.post, url, body, null, 'PUT');
        // console.log(`url:${url},body:${body},楼盘整体概况保存结果：${JSON.stringify(saveResult)}`);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = "楼盘商铺整体概况保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_RELSHOP_VIEW), payload: result.extension || state.payload.entity });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        yield put(actionUtils.action(relShopLoadingEnd))
    } catch (e) {
        result.msg = "楼盘商铺整体概况保存接口调用异常!";
    }
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

//楼盘简介信息保存
export function* saveProjectAsync(state) {
    let result = { isOk: false, msg: '楼盘简介保存失败！' };
    let url = WebApiConfig.buildingProject.Base + "/" + state.payload.entity.id;
    let body = state.payload.entity;
    let method = state.payload.method;
    let city = state.payload.ownCity; // 自己所在城市
    try {
        const saveResult = yield call(ApiClient.post, url, body, null, "PUT");
        // console.log(`url:${url},body:${body},楼盘简介保存结果：${JSON.stringify(saveResult)}`);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = "楼盘简介保存成功";
            yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_PROJECT_VIEW), payload: result.extension || state.payload.entity });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
        }
        yield put(actionUtils.action(projectLoadingEnd))
    } catch (e) {
        result.msg = "楼盘简介保存接口调用异常!";
    }
    notification[result.isOk ? 'success' : 'error']({
        message: result.msg,
        duration: 3
    });
}

// 楼盘报备规则信息保存
export function* saveRulesInfo(action) {
    let type = action.payload.rulesOperType;
    let url = WebApiConfig.buildingrule.Base;
    let body = action.payload.entity;
    let city = action.payload.ownCity; // 自己所在城市
    let res;
    console.log(type, url, JSON.stringify(body), '楼盘报备规则信息保存请求体')
    try {
        res = yield call(ApiClient.post, url, body, null, 'PUT');
        console.log(res, 'ww', body)
        if (body.reportTime) {
            body.reportTime = moment(body.reportTime).format('YYYY-MM-DD')
        }
        body.liberatingStart = moment(body.liberatingStart).format('HH:mm')
        body.liberatingEnd = moment(body.liberatingEnd).format('HH:mm')
        body.template = action.payload.template
        // console.log(body, '需要传到reducer里的body')
        if (res.data.code === '0') {
            yield put({ type: actionUtils.getActionType(actionTypes.RULES_VIEW), payload:  {body: res.data.extension, template: action.payload.template} });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
            notification.success({
                message: '保存成功',
                duration: 3
            });
        }
        yield put(actionUtils.action(ruleLoadingEnd))
    } catch (e) {
        notification.error({
            message: '保存失败',
            duration: 3
        });
    }
}

// 楼盘佣金保存
export function* saveCommissionInfo(action) {
    let type = action.payload.commissionOperType;
    let url = WebApiConfig.buildings.Commission + action.payload.id;
    let body = action.payload.entity;
    let city = action.payload.ownCity; // 自己所在城市
    let res;
    // console.log(type, url, JSON.stringify(body), '请求体')
    try {
        res = yield call(ApiClient.post, url, body, null, 'PUT');
        // console.log(res, 'ww')
        if (res.data.code === '0') {
            yield put({ type: actionUtils.getActionType(actionTypes.COMMISSION_VIEW), payload: body });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) , payload: city });
            notification.success({
                message: '保存成功',
                duration: 3
            });
        }
        yield put(actionUtils.action(commissionLoadingEnd))
    } catch (e) {
        notification.error({
            message: '保存失败',
            duration: 3
        });
    }
}

// 编辑楼盘
export function* gotoThisBuild(action) {
    // console.log(action, '点击楼盘')
    let url = WebApiConfig.buildings.GetThisBuildings + action.payload.id;
    let res;
    yield put(actionUtils.action(gotoThisBuildStart))
    try {
        res = yield call(ApiClient.get, url);
        yield put(actionUtils.action(gotoThisBuildFinish, res));
        if (!action.payload.type) { // type==>dynamic 说明是在动态房源哪里掉的接口，不能进行页面跳转步骤
            yield put(actionUtils.action(gotoChangeMyAdd));
            yield put(actionUtils.action(changeShowGroup, {type: 2}));
        } else {
            yield put(actionUtils.action(changeShowGroup, {type: 1}));
        }
    } catch (e) {
        yield put(actionUtils.action(gotoThisBuildFinish, { code: '1', message: '失败' }));
    }
}

// 提交楼盘
export function* submitBuildInfo(action) {
    let body = action.payload.entity;
    let url = WebApiConfig.buildInfo.Base + body.id;
    let res;
    // console.log(url, 'aaaaaaaaaaa')
    yield put(actionUtils.action(submitBuildInfoStart));
    try {
        res = yield call(ApiClient.post, url);
        // console.log(res, '提交楼盘')
        if (res.data.code === '0') {
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) });
        }
        notification[res.data.code === "0" ? 'success' : 'error']({
            message: res.data.code === "0" ? '提交成功，请等待审核' : '提交失败',
            duration: 3
        })
        yield put(actionUtils.action(submitBuildInfoFinish));
    } catch (e) {
        yield put(actionUtils.action(submitBuildInfoFinish, { code: '1', message: '提交失败', entity: body }));
    }
}

// 保存楼栋批次
export function* saveBatchBuildingAsync(action) {
    // console.log(action.payload.entity, 'action')
    let body = action.payload.entity;
    let city = action.payload.ownCity; // 自己所在城市
    body.forEach((v, i) => {
        v.openDate = v.openDate ? moment(v.openDate).format('YYYY-MM-DD') : null
        v.deliveryDate = moment(v.deliveryDate).format('YYYY-MM-DD')
    })
    let type = action.payload.batchBuildOperType;
    let url = WebApiConfig.batchBuilding.Base + action.payload.id;
    let res;
    // console.log(type, url, JSON.stringify(body), '请求体')
    try {
        res = yield call(ApiClient.post, url, body, null, 'PUT');
        // console.log(res, '保存楼栋批次')
        if (res.data.code === '0') {
            yield put({ type: actionUtils.getActionType(actionTypes.BATCH_BUILDING_VIEW), payload: res.data.extension || body });
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
            notification.success({
                message: '保存成功',
                duration: 3
            });
        }
        yield put(actionUtils.action(batchBuildLoadingEnd))
    } catch (e) {
        // console.log(e)
        notification.error({
            message: '保存失败',
            duration: 3
        });
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
            if (action.payload.parentPage === "building") {
                yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_PIC_VIEW), payload: { filelist: action.payload.completeFileList, type: 'add' } });
            } else {
                yield put({ type: actionUtils.getActionType(actionTypes.SHOP_PIC_VIEW), payload: { filelist: action.payload.completeFileList, type: 'add' } });
            }
            yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
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
        if (action.payload.parentPage === "building") {
            url = WebApiConfig.buildingAttachInfo.PicDelete + action.payload.id
            res = yield call(ApiClient.post, url, body)
            if (res.data.code === '0') {
                yield put({ type: actionUtils.getActionType(actionTypes.BUILDING_PIC_VIEW), payload: { filelist: action.payload.deletePicList, type: 'delete' } });
                notification.success({
                    message: '图片删除成功！',
                    duration: 3
                });
                yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
            }
            yield put(actionUtils.action(attchLoadingEnd))
        } else {
            url = WebApiConfig.shopsAttachInfo.PicDelete + action.payload.id
            res = yield call(ApiClient.post, url, body)
            if (res.data.code === '0') {
                yield put({ type: actionUtils.getActionType(actionTypes.SHOP_PIC_VIEW), payload: { filelist: action.payload.deletePicList, type: 'delete' } });
                notification.success({
                    message: '图片删除成功！',
                    duration: 3
                });
                yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), payload: city  });
            }
            yield put(actionUtils.action(attchLoadingEnd))
        }
    } catch (e) {
        notification.error({
            message: '图片删除失败！',
            duration: 3
        });
    }
}



export function* watchBuildingAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_BASIC_SAVE), saveBuildingBasicAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_SUPPORT_SAVE), saveSupportInfoAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_RELSHOP_SAVE), saveRelshopsAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PROJECT_SAVE), saveProjectAsync);
    // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_PICTURE_SAVE), savePictureAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_THIS_BUILD), gotoThisBuild);
    yield takeLatest(actionUtils.getActionType(actionTypes.BUILD_INFO_SUBMIT), submitBuildInfo);
    yield takeLatest(actionUtils.getActionType(actionTypes.BATCH_BUILDING_SAVE_ASYNC), saveBatchBuildingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_PICTURE_ASYNC), savePictureAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_PICTURE_ASYNC), deletePicAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.COMMISSION_SAVE), saveCommissionInfo);
    yield takeLatest(actionUtils.getActionType(actionTypes.RULES_SAVE), saveRulesInfo);
}