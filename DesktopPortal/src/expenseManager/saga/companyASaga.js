import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import SearchCondition from '../constants/searchCondition';
import { companyListGet, companyACloseDialog} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)

//获取列表
export function* getCompanyAListAsync(state) {
    //console.log("getCompanyAListAsync:", state);
    let result = { isOk: false, extension: [], msg: '甲方列表获取失败' };
    let url = WebApiConfig.extraInfo.search;
    console.log(`url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const comResult = yield call(ApiClient.post, url, state.payload/*{"isDeleted":true,"isSearch":true,"keyWords":"ff","roleId":"","OrganizationIds":[],"pageIndex":0,"pageSize":10}*/);
        getApiResult(comResult, result);
        console.log(`url:${url},resbody:${JSON.stringify(result)}`);
        if(state.payload.type !== 'dialog')
        {
            yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
        }
            
    } catch (e) {
        result.msg = '甲方列表获取接口调用失败！';
    }
    if (result.isOk) {
        result.msg = '甲方列表获取成功';
        if(state.payload.type === 'dialog')
        {
            // if(result.extension || result.extension.length === 0)
            // {
            //     notification.error({
            //         message: '没有甲方信息，请先在甲方管理中设置',
            //         description: result.msg,
            //         duration: 3
            //     });
            // }
      
            yield put({type: actionUtils.getActionType(actionTypes.OPEN_COMPANYA_DIALOG), payload: result});
            
           
        }
        else
        {
            if (!state.payload.isSearch) {
                yield put({ type: actionUtils.getActionType(actionTypes.COMPANYA_GET_LIST), payload: result });
            } else {
                yield put({ type: actionUtils.getActionType(actionTypes.COMPANYA_SERACH_COMPLETE), payload: result });
            }
    
        }
 
    } else {
        notification.error({
            message: '甲方列表获取失败!',
            description: result.msg,
            duration: 3
        });
    }

}

export function* saveCompayAsync(state) {
    let result = { isOk: false, msg: '甲方保存失败!' };
    let url = WebApiConfig.extraInfo.add;
    if (state.payload.method == 'PUT') {
        url = WebApiConfig.extraInfo.modify;
        url +=  state.payload.companyAInfo.id;
    }
    console.log(`甲方保存提交：url:${url},method:${state.payload.method},body:${JSON.stringify(state.payload.companyAInfo)}`);
    try {
        const saveResult = yield call(ApiClient.post, url, state.payload.companyAInfo, null);
        console.log("甲方结果：", saveResult);
        getApiResult(saveResult, result);
        if (result.isOk) {
            result.msg = '甲方保存成功!';
            yield put(actionUtils.action(companyListGet, SearchCondition.companyASearchCondition));
            yield put(actionUtils.action(companyACloseDialog, {}));
        }
    } catch (e) {
        result.msg = '甲方保存接口调用失败！';
    }
    notification[result.isOk ? 'success' : 'error']({
        message: '甲方',
        description: result.msg,
        duration: 3
    });
}

export function* deleteCompanyAsync(state) {
    let result = { isOk: false, msg: '甲方删除失败!' };
    let url = WebApiConfig.extraInfo.delete;
    console.log(`甲方删除提交：url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const delResult = yield call(ApiClient.post, url, state.payload);
        getApiResult(delResult, result);
        if (result.isOk) {
            result.msg = '甲方删除成功!';
            yield put(actionUtils.action(companyListGet, SearchCondition.companyASearchCondition));
        }
    } catch (e) {
        result.msg = '甲方删除接口调用失败!';
    }
    //result = { isOk true, msg: '用户删除成功!' };
    notification[result.isOk ? 'success' : 'error']({
        message: '甲方',
        description: result.msg,
        duration: 3
    });
}


export function* getAllCompanyADataAsync(state){
    //console.log("getCompanyAListAsync:", state);
    let result = { isOk: false, extension: [], msg: '甲方列表获取失败' };
    let url = WebApiConfig.extraInfo.search;
    console.log(`url:${url},body:${JSON.stringify(state.payload)}`);
    try {
        const comResult = yield call(ApiClient.post, url, state.payload/*{"isDeleted":true,"isSearch":true,"keyWords":"ff","roleId":"","OrganizationIds":[],"pageIndex":0,"pageSize":10}*/);
        getApiResult(comResult, result);
        console.log(`url:${url},resbody:${JSON.stringify(result)}`);
        //yield put({ type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false });
    } catch (e) {
        result.msg = '甲方列表获取接口调用失败！';
    }
    if (result.isOk) {
        result.msg = '甲方列表获取成功';
        if(state.payload.type === 'dialog')
        {
            yield put({type: actionUtils.getActionType(actionTypes.OPEN_COMPANYA_DIALOG), payload: result});  
           
        }
        else
        {
            if (!state.payload.isSearch) {
                yield put({ type: actionUtils.getActionType(actionTypes.COMPANYA_GET_LIST), payload: result });
            } else {
                yield put({ type: actionUtils.getActionType(actionTypes.COMPANYA_SERACH_COMPLETE), payload: result });
            }
    
        }
 
    } else {
        notification.error({
            message: '甲方列表获取失败!',
            description: result.msg,
            duration: 3
        });
    }

}



// //获取用户职级列表
// export function* getUserPrivListAsync() {
//     let result = { isOk: false, extension: [], msg: '职级列表获取失败！' };
//     let url = WebApiConfig.user.Priv;
//     try {
//         const getResult = yield call(ApiClient.get, url);
//         getApiResult(getResult, result);
//         //console.log(`获取用户职级列表：url${url},restul:${JSON.stringify(result)}`);
//     } catch (e) {
//         result.msg = '接口调用失败！';
//     }
//     // result.isOk = true;
//     // result.extension = [{ id: '49', name: '区域总经理' }, { id: '50', name: '高级区域经理' }];
//     yield put({ type: actionUtils.getActionType(actionTypes.EMP_PRIV_LIST_UPDATE), payload: result });
// }

// export function* saveEmpRoleAsync(state) {
//     let result = { isOk: false, extension: [], msg: '用户角色保存失败！' };
//     let addRoles = state.payload.addRoles, removeRoles = state.payload.removeRoles;
//     let userRoleAddUrl = WebApiConfig.user.UserRoleAdd + state.payload.userName;
//     let userRoleRemoveUrl = WebApiConfig.user.UserRoleRemove + state.payload.userName;
//     try {
//         if (removeRoles.length > 0) {
//             const roleRemoveResult = yield call(ApiClient.post, userRoleRemoveUrl, removeRoles);
//             getApiResult(roleRemoveResult, result);
//             console.log("用户所属角色删除结果：", roleRemoveResult);
//         }
//         if (addRoles.length > 0) {
//             const roleSaveResult = yield call(ApiClient.post, userRoleAddUrl, addRoles);
//             getApiResult(roleSaveResult, result);
//             console.log('用户角色新增结果：', roleSaveResult);
//         }
//         if (result.isOk) {
//             result.msg = '员工角色保存成功!';
//             yield put(actionUtils.action(empDialogClose, {}));
//             yield put(actionUtils.action(empListGet, SearchCondition.empListCondition));
//         }
//     } catch (e) {
//         result.msg = '员工角色保存接口调用失败！';
//     }
//     notification[result.isOk ? 'success' : 'error']({
//         message: '员工角色保存',
//         description: result.msg,
//         duration: 3
//     });
// }
// //密码一键重置
// export function* resetEmpPwdAsync(state) {
//     let result = { isOk: false, msg: '密码重置失败!' };
//     let url = WebApiConfig.user.ResetPwd;
//     try {
//         const delResult = yield call(ApiClient.post, url, { userName: state.payload, password: '123456' });
//         console.log(`密码重置提交：url:${url},body:${JSON.stringify({ userName: state.payload, password: '123456' })},result:${JSON.stringify(delResult)}`);
//         getApiResult(delResult, result);
//         if (result.isOk) {
//             result.msg = '密码重置成功!';
//         }
//     } catch (e) {
//         result.msg = '密码重置接口调用失败!';
//     }
//     notification[result.isOk ? 'success' : 'error']({
//         description: result.msg,
//         duration: 3
//     });
// }

export function* watchCompanyAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.COMPANYA_GET_LIST), getCompanyAListAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.EMP_SEARCH), getEmpListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.COMPANYA_SAVE), saveCompayAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.COMPANYA_DELETE), deleteCompanyAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.EMP_GET_PRIV_LIST), getUserPrivListAsync);
   // yield takeLatest(actionUtils.getActionType(actionTypes.COMPANYA), saveEmpRoleAsync);
    //yield takeLatest(actionUtils.getActionType(actionTypes.EMP_RESET_PWD), resetEmpPwdAsync);
}
