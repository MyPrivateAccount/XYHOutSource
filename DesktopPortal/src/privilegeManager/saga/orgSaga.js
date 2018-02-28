import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { getOrgDataByID, orgDialogClose } from '../actions/actionCreator';
import { notification } from 'antd';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

export function* getOrgByIDAsync(state) {
    let result = { isOk: false, extension: [], msg: '部门数据获取异常!' };
    let url = WebApiConfig.org.Get + state.payload.id;
    console.log(`获取orgTree数据源：${url}`);
    try {
        const getResult = yield call(ApiClient.get, url);
        getApiResult(getResult, result);
        if (result.code == '404') {//部门404特殊处理
            result.isOk = true;
            result.extension = [];
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        console.log(result.data);
    } catch (e) {
        result.msg = '组织结构获取接口调用失败！';
    }

    // if (state.payload.id == '0') {
    //     result.extension.push({ id: 'root', organizationName: '新耀行', fax: '1241', children: [] });
    // }
    // if (state.payload.id == 'root') {
    //     result.extension.push({ id: '11', organizationName: '重庆', fax: '1241', address: 'xx路', leaderManager: 'tom', phone: '13281209291', children: [] });
    //     result.extension.push({ id: '12', organizationName: '成都', children: [] });
    // }
    // if (state.payload.id == '11') {
    //     result.extension.push({ id: '111', organizationName: '重庆部门1', children: [] });
    //     result.extension.push({ id: '112', organizationName: '重庆部门2', children: [] });
    // }
    // if (state.payload.id == '12') {
    //     result.extension.push({ id: '121', organizationName: '成都部门1', children: [] });
    // }
    if (result.isOk) {
        yield put({ type: actionUtils.getActionType(actionTypes.ORG_DATA_UPDATE), payload: { nodeList: result.extension, parentID: state.payload.id, type: 'add' } })
    }
    else {
        notification.error({
            message: '组织结构',
            description: result.msg,
            duration: 3
        });
    }
}

export function* saveOrgAsync(state) {
    let result = { isOk: false, msg: '部门保存失败!' };
    let url = WebApiConfig.org.Base;
    if (state.payload.method == 'PUT') {
        url += '/' + state.payload.orgInfo.id;
    }
    let body = state.payload.orgInfo;
    console.log(`post url:${url},type:${state.payload.method},body:${JSON.stringify(body)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, body, null, state.payload.method);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '部门保存成功!';
            yield put(actionUtils.action(getOrgDataByID, { id: state.payload.orgInfo.parentId }));
            yield put(actionUtils.action(orgDialogClose, {}));
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '部门保存接口调用异常';
    }
    // result.isOk=true;
    // result.msg='部门保存成功!';
    notification[result.isOk ? 'success' : 'error']({
        message: '组织结构',
        description: result.msg,
        duration: 3
    });
}

export function* deleteOrgAsync(state) {
    let result = { isOk: false, msg: '部门删除失败!' };
    let url = WebApiConfig.org.Base + '/' + state.payload;
    console.log(`部门删除请求：url:${url},id:${state.payload}`);
    try {
        const deleteResult = yield call(ApiClient.post, url, null, null, 'DELETE');
        getApiResult(deleteResult, result);
        if (result.isOk) {
            result.msg = '部门删除成功!';
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_DATA_UPDATE), payload: { removeId: state.payload, type: 'delete' } });
        }
    } catch (e) {
        result.msg = '部门删除接口调用异常!';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '组织结构',
        description: result.msg,
        duration: 3
    });
}
//获取有权限的部门树(添加用户时：UserInfoCreate，添加角色时：RoleCreate，添加公共角色时：PublicRoleOper，角色授权时：AuthorizationPermission)
export function* getPermissionOrgAsync(state) {
    let result = { isOk: false, msg: '根据权限获取部门数据失败!' };
    let url = WebApiConfig.org.permissionOrg + state.payload;
    try {
        const deleteResult = yield call(ApiClient.get, url);
        //console.log(`部门获取请求：url:${url},id:${state.payload},result:${JSON.stringify(deleteResult)}`);
        getApiResult(deleteResult, result);
        if (result.isOk) {
            result.msg = '部门获取成功!';
            yield put({ type: actionUtils.getActionType(actionTypes.ORG_GET_PERMISSION_TREE_UPDATE), payload: { permissionType: state.payload, extension: result.extension } });
        }
        yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '部门按权限获取接口调用异常!';
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getAllCityAsync(state) {
    let result = { isOk: false, extension: [], msg: '城市数据获取失败！' };
    let url = WebApiConfig.dic.AreaList;
    try {
        const areaResult = yield call(ApiClient.post, url, { levels: [1], codes: [] });
        getApiResult(areaResult, result);
        if (result.isOk) {
            yield put({ type: actionUtils.getActionType(actionTypes.DIC_GET_AREA_COMPLETE), payload: result.extension });
        }
    } catch (e) {
        result.msg = "城市数据获取接口调用异常!";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}


export default function* watchAllOrgAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_DATA_GET), getOrgByIDAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_DATA_SAVE), saveOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_DATA_DELETE), deleteOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.ORG_GET_PERMISSION_TREE), getPermissionOrgAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.DIC_GET_AREA), getAllCityAsync);
}