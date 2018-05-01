import {takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants'
import {} from '../actions'
import appUtils from '../../utils/appUtils'
import WebApiConfig from '../constants/webapiConfig';
import {notification} from 'antd';
import getApiResult from '../../utils/sagaUtil';

const actionUtils = appUtils(actionTypes.APPNAME)

//获取区域列表
export function* getPlanListAsync(action) {
    let result = {isOk: false, extension: [], msg: '业态规划列表获取失败！'};
    let {level, parentId} = action.payload; //获取规划级别: 1,2,3
    let url = WebApiConfig.planning.getPlanList + parentId
    try {
        let res = yield call(ApiClient.get, url);
        getApiResult(res, result);
        result.level = level;
        result.parent = parentId;
        if (result.isOk) {
            console.log("业态规划列表:", res, url);
            yield put({type: actionUtils.getActionType(actionTypes.PLANNING_GET_LIST_END), payload: result});
        }
    } catch (e) {
        result.msg = '业态规划列表接口异常';
    }
    if (!result.isOk) {
        notification.error({
            description: '业态规划列表获取失败',
            duration: 3
        });
    }
}

// 删除城市
export function* delPlanningAsync(action) {
    let result = {isOk: false, extension: [], msg: '业态规划数据删除失败！'};
    let entity = action.payload;
    let url = WebApiConfig.planning.removePlan + entity.id;
    try {
        let res = yield call(ApiClient.post, url, null, null, 'DELETE');
        getApiResult(res, result);
        if (result.isOk) {
            result.msg = "业态规划数据删除成功";
            yield put({type: actionUtils.getActionType(actionTypes.PLANNING_REMOVE_END), payload: entity});
        }
    } catch (e) {
        result.msg = "业态规划数据删除接口异常";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

// 保存城市
export function* savePlanningAsync(action) {
    let result = {isOk: false, extension: [], msg: '业态规划数据保存失败！'};
    let {entity, op} = action.payload; // op:1 添加 2 修改
    let res, method;
    let url = WebApiConfig.planning.savePlan;
    try {
        if (op === 1) {
            method = 'POST';
        } else {
            method = 'PUT';
            url += `/${entity.id}`;
        }
        res = yield call(ApiClient.post, url, entity, null, method);
        getApiResult(res, result);
        entity.op = op;
        console.log("业态规划保存:", url, entity, method);
        if (result.isOk) {
            result.msg = "业态规划数据保存成功";
            yield put({type: actionUtils.getActionType(actionTypes.PLANNING_SAVE_END), payload: entity});
        }
    } catch (e) {
        result.msg = "业态规划数据保存接口异常";
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

export default function* run() {
    yield takeLatest(actionUtils.getActionType(actionTypes.PLANNING_GET_LIST), getPlanListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.PLANNING_REMOVE), delPlanningAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.PLANNING_SAVE), savePlanningAsync);
}