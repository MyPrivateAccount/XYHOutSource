import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import SearchCondition from '../constants/searchCondition';
import { empListGet, orgDialogClose, empDialogClose } from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

//获取用户列表
export function* getEmpListAsync(state) {
    let result = { isOk: false, extension: [], msg: '用户列表获取失败' };
    let url = WebApiConfig.user.List;
    console.log(`url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const empResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(empResult, result);
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '用户列表获取接口调用失败！';
    }
    if (result.isOk) {
        if (!state.payload.isSearch) {
            yield put({ type: actionUtils.getActionType(actionTypes.EMP_LIST_UPDATE), payload: result });
        } else {
            yield put({ type: actionUtils.getActionType(actionTypes.EMP_SERACH_COMPLETE), payload: result });
        }
    } else {
        notification.error({
            message: '员工',
            description: result.msg,
            duration: 3
        });
    }

}

export function* saveEmpAsync(state) {
    let result = { isOk: false, msg: '用户保存失败!' };
    let url = WebApiConfig.user.Base;
    if (state.payload.method == 'PUT') {
        url += '/' + state.payload.empInfo.id;
    }
    console.log(`用户保存提交：url:${url},method:${state.payload.method},body:${JSON.stringify(state.payload.empInfo)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.empInfo, null, state.payload.method);
        console.log("保存结果：", saveResult);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '用户保存成功!';
            yield put(actionUtils.action(empListGet, SearchCondition.empListCondition));
            yield put(actionUtils.action(empDialogClose, {}));
        }
    } catch (e) {
        result.msg = '用户保存接口调用失败！';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '员工',
        description: result.msg,
        duration: 3
    });
}

export function* deleteEmpAsync(state) {
    let result = { isOk: false, msg: '用户删除失败!' };
    let url = WebApiConfig.user.Delete;
    console.log(`用户删除提交：url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const delResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(delResult, result);
        if (result.isOk) {
            result.msg = '用户删除成功!';
            yield put(actionUtils.action(empListGet, SearchCondition.empListCondition));
        }
    } catch (e) {
        result.msg = '用户删除接口调用失败!';
    }
    //result = { isOk true, msg: '用户删除成功!' };
    notification[result.isOk ? 'success' : 'error']({
        message: '员工',
        description: result.msg,
        duration: 3
    });
}

//获取用户职级列表
export function* getUserPrivListAsync() {
    let result = { isOk: false, extension: [], msg: '职级列表获取失败！' };
    let url = WebApiConfig.user.Priv;
    try {
        const getResult = yield call(ApiClient.get, url);
        getApiResult(getResult, result);
        //console.log(`获取用户职级列表：url${url},restul:${JSON.stringify(result)}`);
    } catch (e) {
        result.msg = '接口调用失败！';
    }
    // result.isOk = true;
    // result.extension = [{ id: '49', name: '区域总经理' }, { id: '50', name: '高级区域经理' }];
    yield put({ type: actionUtils.getActionType(actionTypes.EMP_PRIV_LIST_UPDATE), payload: result });
}

export function* saveEmpRoleAsync(state) {
    let result = { isOk: false, extension: [], msg: '用户角色保存失败！' };
    let addRoles = state.payload.addRoles, removeRoles = state.payload.removeRoles;
    let userRoleAddUrl = WebApiConfig.user.UserRoleAdd + state.payload.userName;
    let userRoleRemoveUrl = WebApiConfig.user.UserRoleRemove + state.payload.userName;
    try {
        if (removeRoles.length > 0) {
            const roleRemoveResult = yield call(ApiClient.post, userRoleRemoveUrl, removeRoles);
            getApiResult(roleRemoveResult, result);
            console.log("用户所属角色删除结果：", roleRemoveResult);
        }
        if (addRoles.length > 0) {
            const roleSaveResult = yield call(ApiClient.post, userRoleAddUrl, addRoles);
            getApiResult(roleSaveResult, result);
            console.log('用户角色新增结果：', roleSaveResult);
        }
        if (result.isOk) {
            result.msg = '员工角色保存成功!';
            yield put(actionUtils.action(empDialogClose, {}));
            yield put(actionUtils.action(empListGet, SearchCondition.empListCondition));
        }
    } catch (e) {
        result.msg = '员工角色保存接口调用失败！';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '员工角色保存',
        description: result.msg,
        duration: 3
    });
}
//密码一键重置
export function* resetEmpPwdAsync(state) {
    let result = { isOk: false, msg: '密码重置失败!' };
    let url = WebApiConfig.user.ResetPwd;
    try {
        const delResult = yield call(ApiClient.post, url, { userName: state.payload, password: '123456' });
        console.log(`密码重置提交：url:${url},body:${JSON.stringify({ userName: state.payload, password: '123456' })},result:${JSON.stringify(delResult)}`);
        getApiResult(delResult, result);
        if (result.isOk) {
            result.msg = '密码重置成功!';
        }
    } catch (e) {
        result.msg = '密码重置接口调用失败!';
    }
    notification[result.isOk ? 'success' : 'error']({
        description: result.msg,
        duration: 3
    });
}

export function* watchEmpAll() {
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_GET_LIST), getEmpListAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.EMP_SEARCH), getEmpListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_SAVE), saveEmpAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_DELETE), deleteEmpAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_GET_PRIV_LIST), getUserPrivListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_ROLE_SAVE), saveEmpRoleAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.EMP_RESET_PWD), resetEmpPwdAsync);
}
