import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants'
import {
    getAreaListStart, getAreaListFinish,
    delAreaStart, delAreaFinish,
    saveAreaStart, saveAreaFinish
} from '../actions'
import appUtils from '../../utils/appUtils'
import WebApiConfig from '../constants/webapiConfig';

const actionUtils = appUtils(actionTypes.APPNAME)

function delay(interval) {
    return new Promise(function (resolve) {
        setTimeout(resolve, interval);
    });
}


//获取区域列表
export function* getAreaListAsync(action) {
    let level = action.payload.level; //获取级别 1城市 2县区 3商圈
    let url, condition, parentCode;
    if (level !== 1) {
        parentCode = action.payload.parent; //上级区域编码
        url = WebApiConfig.area.ChildList + parentCode
    } else {
        condition = { levels: [level] };
        url = WebApiConfig.area.List;
    }
    yield put(actionUtils.action(getAreaListStart, { level: level }));
    try {
        let res;
        level !== 1 ? res = yield call(ApiClient.get, url) : res = yield call(ApiClient.post, url, condition);
        res.level = level;
        res.parent = parentCode;
        yield put(actionUtils.action(getAreaListFinish, res));
    } catch (e) {
        yield put(actionUtils.action(getAreaListFinish, { code: '1', message: '失败', level: level }));
    }

}

// 删除城市
export function* delAreaAsync(action) {
    let level = action.payload.level; //获取级别 1城市 2县区 3商圈
    let code = action.payload.code; //上级区域编码
    yield put(actionUtils.action(delAreaStart, { level: level, code: code }));
    let url = WebApiConfig.area.Delete;
    try {
        let res = yield call(ApiClient.post, url, [code])
        res.level = level;
        res.areaCode = code;
        yield put(actionUtils.action(delAreaFinish, res));
    } catch (e) {
        yield put(actionUtils.action(delAreaFinish, { code: '1', message: '失败', level: level, areaCode: code }));
    }

}

// 保存城市
export function* saveAreaAsync(action) {
    let areaGroup = action.payload.entity;
    let level = action.payload.level; //获取级别 1城市 2县区 3商圈
    let parent = action.payload.parent; //上级区域编码
    let op = action.payload.op; // 1 添加 2 修改
    let res;
    yield put(actionUtils.action(saveAreaStart, { level: level, entity: areaGroup }));
    let url = WebApiConfig.area.Base;
    try {
        let method = (op === 1 ? 'POST' : 'PUT');
        res = op === 1 ? yield call(ApiClient.post, url, areaGroup) : yield call(ApiClient.post, url + '/' + areaGroup.code, areaGroup, null, 'PUT')
        res.level = level;
        res.entity = areaGroup;
        yield put(actionUtils.action(saveAreaFinish, res));
    } catch (e) {
        yield put(actionUtils.action(saveAreaFinish, { code: '1', message: '失败', level: level }));
    }

}

export default function* run() {
    yield takeLatest(actionUtils.getActionType(actionTypes.AREA_GET_LIST_ASYNC), getAreaListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.AREA_DEL_ASYNC), delAreaAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.AREA_SAVE_ASYNC), saveAreaAsync);
}