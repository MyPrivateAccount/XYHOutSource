import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants'
import {
    getGroupListStart, getGroupListFinish, getParListStart,
    getParListFinish, delDicGroupStart, delDicGroupFinish,
    delDicValueStart, delDicValueFinish,
    saveDicGroupStart, saveDicGroupFinish,
    saveDicValueStart, saveDicValueFinish,
    startDicValueStart, startDicValueEnd,
} from '../actions'
import appUtils from '../../utils/appUtils'
import WebApiConfig from '../constants/webapiConfig';
import { notification } from 'antd';

const actionUtils = appUtils(actionTypes.APPNAME)

function delay(interval) {
    return new Promise(function (resolve) {
        setTimeout(resolve, interval);
    });
}


//获取字典分组列表
export function* getGroupListAsync(state) {
    let groupListResult = [];
    let url = WebApiConfig.dic.GroupList;
    yield put(actionUtils.action(getGroupListStart));
    try {
        groupListResult = yield call(ApiClient.post, url, {});
        yield put(actionUtils.action(getGroupListFinish, groupListResult.data));
    } catch (e) {
        yield put(actionUtils.action(getGroupListFinish, { code: '1', message: '失败' }));
    }
}

//获取参数列表
export function* getParListAsync(state) {
    console.log(state, 777)
    let url = WebApiConfig.dic.ParList + state.payload;
    console.log(url, '地址')
    yield put(actionUtils.action(getParListStart));
    try {
        let res = yield call(ApiClient.get, url)
        console.log(res,  '获取参数列表')
        yield put(actionUtils.action(getParListFinish, res.data));
    } catch (e) {
        yield put(actionUtils.action(getParListFinish, { code: '1', message: '失败' }));
    }
}

//删除参数组
export function* delDicGroupAsync(action) {
    let dicGroup = action.payload;
    let url = WebApiConfig.dic.GroupListDelete + dicGroup.id;
    yield put(actionUtils.action(delDicGroupStart, dicGroup));
    try {
        let res = yield call(ApiClient.post, url, null, null, 'DELETE')
        res.entity = dicGroup;
        yield put(actionUtils.action(delDicGroupFinish, res));
    } catch (e) {
        yield put(actionUtils.action(delDicGroupFinish, { code: '1', message: '删除失败', entity: dicGroup }));
    }

}




//保存字典组
export function* saveDicGroupAsync(action) {
    let dicGroup = action.payload.entity;
    let op = action.payload.op; //1添加 2修改
    let url = WebApiConfig.dic.GroupListSave;
    let res;
    yield put(actionUtils.action(saveDicGroupStart, dicGroup));
    try {
        res = op === 1 ? yield call(ApiClient.post, url, dicGroup) : yield call(ApiClient.post, url + '/' + dicGroup.id, dicGroup, null, 'PUT')
        res.entity = dicGroup;
        yield put(actionUtils.action(saveDicGroupFinish, res));
    } catch (e) {
        yield put(actionUtils.action(saveDicGroupFinish, { code: '1', message: '保存失败', entity: dicGroup }));
    }

}


//保存参数
export function* saveDicValueAsync(action) {
    // console.log(action, '保存参数')
    let dicItem = action.payload.entity;
    let op = action.payload.op; //1添加 2修改
    let url = WebApiConfig.dic.ParListSave;
    // console.log(JSON.stringify(dicItem), url, '修改字典项')
    let res;
    yield put(actionUtils.action(saveDicValueStart, dicItem));
    try {
        res = op === 1 ? yield call(ApiClient.post, url, dicItem) : yield call(ApiClient.post, url + '/' + dicItem.groupId + '/' + dicItem.value, dicItem, null, 'PUT')
        res.entity = dicItem;
        // console.log(op, res, '数据')
        yield put(actionUtils.action(saveDicValueFinish, res));
    } catch (e) {
        yield put(actionUtils.action(saveDicValueFinish, { code: '1', message: '保存失败', entity: dicItem }));
    }
}

//删除参数（禁用字典项定义）
export function* delDicValueAsync(action) {
    let dicValue = action.payload;
    let url = WebApiConfig.dic.ParListDelete;
    let body = [{ groupId: dicValue.groupId, key: dicValue.key }];
    yield put(actionUtils.action(delDicValueStart, dicValue));
    //  console.log(JSON.stringify(body), url, '删除字典项')
    try {
        let res = yield call(ApiClient.post, url, body);
        // console.log(res, '删除字典项res')
        res.entity = dicValue;
        yield put(actionUtils.action(delDicValueFinish, res));
    } catch (e) {
        yield put(actionUtils.action(delDicValueFinish, { code: '1', message: '删除失败', entity: dicValue }));
    }

}

// 启用字典项定义
export function* startDicValueAsync(action) {
    // console.log(action, '启用字典项定义')
    let dicItem = action.payload;
    let url = WebApiConfig.dic.StartDicValue + '/' + action.payload.groupId + '/' + action.payload.value;
    // console.log(JSON.stringify(dicItem), url, '启用字典项定义请求体')
    let res;
    yield put(actionUtils.action(startDicValueStart, dicItem));
    try {
        res = yield call(ApiClient.post, url, null, null, 'PUT')
        res.entity = dicItem;
        // console.log(res, '数据')
        yield put(actionUtils.action(startDicValueEnd, res));
    } catch (e) {
        yield put(actionUtils.action(startDicValueEnd, { code: '1', message: '保存失败', entity: dicItem }));
    }
}

export default function* run() {
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_GROUPLIST_ASYNC), getGroupListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_PARLIST_ASYNC), getParListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_DEL_GROUP_ASYNC), delDicGroupAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_DEL_VALUE_ASYNC), delDicValueAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_SAVE_GROUP_ASYNC), saveDicGroupAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_SAVE_VALUE_ASYNC), saveDicValueAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.START_DIC_VALUE_ASYNC), startDicValueAsync);
}