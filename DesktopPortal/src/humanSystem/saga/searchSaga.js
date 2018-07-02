import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import {notification} from 'antd';
import {getHumanDetailEnd, setSearchLoadingVisible} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)
const PositionStatus = ["未入职", "离职", "入职", "转正"];
export function* getCustomerListAsync(state) {
    let result = {isOk: false, extension: [], msg: '客源查询失败！'};
    let url = WebApiConfig.search.getSaleManCustomerList;//默认为业务员客户查询
    let body = state.payload;
    if (body) {
        if (body.searchSourceType === "2") {//已成交客户列表
            url = WebApiConfig.search.getDealCustomerList;
        }
        else if (body.searchSourceType === "3") {//失效客户列表
            url = WebApiConfig.search.getLoosCustomerList;
        }
        else if (body.searchSourceType === "4") {//公客池客户列表
            url = WebApiConfig.search.getPoolCustomerList;
        }
    }
    try {
        let res = yield call(ApiClient.post, url, state.payload)
        // console.log(`url:${url},body:${JSON.stringify(state.payload)},result:${JSON.stringify(res)}`);
        getApiResult(res, result);
        if (result.isOk) {
            if (res.data.validityCustomerCount) {
                result.validityCustomerCount = res.data.validityCustomerCount;
            }
            yield put({type: actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER_COMPLETE), payload: result});
        }
        yield put({type: actionUtils.getActionType(actionTypes.SET_SEARCH_LOADING), payload: false});
    } catch (e) {
        result.msg = "客源查询接口调用异常！";
    }
    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getSearchConditionAsync(state) {
    let result = {isOk: false, extension: {}, msg: '检索条件失败！'};
    let url = WebApiConfig.search.searchHumanList;
    let entity = {...state.payload};
    try {
        if (entity.staffStatuses && entity.staffStatuses.includes('0')) {
            entity.staffStatuses = null;
        } else {
            entity.staffStatuses = [entity.staffStatuses]
        }
        let res = yield call(ApiClient.post, url, entity);
        console.log("查询结果:", res);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            let data = lv.map(function (v, k) {
                let sn = "", fn = "";
                (v.sex == 1) && (sn = "男");
                (v.sex == 2) && (sn = "女");
                fn = v.staffStatus ? PositionStatus[v.staffStatus] : "未入职"
                return {
                    key: k, sexname: sn, staffStatus: fn, ...v,
                    entryTime: v.entryTime ? v.entryTime.replace("T", " ") : "", becomeTime: v.becomeTime ? v.becomeTime.replace("T", " ") : "",
                    socialInsurance: v.IsSocialInsurance ? "是" : "否", contract: v.contract ? "是" : "否",
                };
            });
            let re = {
                extension: data,
                pageIndex: res.data.pageIndex,
                pageSize: res.data.pageSize,
                totalCount: res.data.totalCount
            };

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ALLHUMANINFO), payload: re});
        }
    } catch (e) {
        result.msg = '检索接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getHumanListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '检索关键字失败！'};
    let url = WebApiConfig.search.searchHumanList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            let data = lv.map(function (v, k) {
                let sn = "", fn = "";
                (v.sex == 1) && (sn = "男");
                (v.sex == 2) && (sn = "女");
                fn = v.staffStatus ? PositionStatus[v.staffStatus] : "未入职";
                return {
                    key: k, sexname: sn, staffStatus: fn, ...v,
                    entryTime: v.entryTime ? v.entryTime.replace("T", " ") : "", becomeTime: v.becomeTime ? v.becomeTime.replace("T", " ") : "",
                    socialInsurance: v.IsSocialInsurance ? "是" : "否", contract: v.contract ? "是" : "否",
                };
            });

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ALLHUMANINFO), payload: data});
        }
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getMonthListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取月结列表失败！'};
    let url = WebApiConfig.search.getAllMonthList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            let data = lv.extension.map(function (v, k) {
                let last = new Date(v.settleTime);
                last.setMonth(last.getMonth() - 1);
                let v1 = last.getFullYear() + '.' + (last.getMonth() + 1);

                last.setMonth(last.getMonth() + 1);
                let v2 = last.getFullYear() + '.' + (last.getMonth() + 1);
                return {key: k, last: v1, monthtime: v2, operater: v.operName};
            });

            yield put({
                type: actionUtils.getActionType(actionTypes.MONTH_UPDATEMONTHLIST),
                payload: {extension: data, pageIndex: lv.pageIndex, pageSize: lv.pageSize, totalCount: lv.totalCount, lastTime: lv.lastTime}
            });
        }
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getHumanDetailAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取员工详情失败!'};
    let url = WebApiConfig.search.getHumanDetail + state.payload;
    yield put(actionUtils.action(setSearchLoadingVisible(true)));
    try {
        let res = yield call(ApiClient.get, url);
        if (res.data.code == 0) {
            result.isOk = true;
            result.msg = "获取员工详情成功";
            let detailInfo = res.data.extension;
            yield put(actionUtils.action(getHumanDetailEnd(detailInfo)));
            yield put(actionUtils.action(setSearchLoadingVisible(false)));
        }
    } catch (e) {
        result.msg = '获取员工详情接口异常!';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getBlackListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取黑名单列表失败！'};
    let url = WebApiConfig.search.getBlackList;
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            yield put({
                type: actionUtils.getActionType(actionTypes.UPDATE_BLACKLST),
                payload: {
                    extension: lv.extension.map(function (v, i) {
                        return Object.assign({key: i}, v);
                    }), pageIndex: lv.pageIndex, pageSize: lv.pageSize, totalCount: lv.totalCount, lastTime: lv.lastTime
                }
            });
        }
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getSalaryListAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取薪酬列表失败！'};
    let url = WebApiConfig.search.getSalaryList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;

            let lv = res.data.extension;
            let data = lv.extension.map(function (v, k) {
                return {key: k, ...v};
            });
            let re = {
                extension: data,
                pageIndex: lv.pageIndex,
                pageSize: lv.pageSize,
                totalCount: lv.totalCount
            };

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_SALARYINFO), payload: re});
        }
    } catch (e) {
        result.msg = '检索关键字接口调用异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getSalaryItemAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取单一薪酬失败！'};
    let url = WebApiConfig.search.getSalaryItem + '/' + state.payload;

    try {
        let res = yield call(ApiClient.get, url);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_SALARYITEM), payload: res.data.extension});
        }
    } catch (e) {
        result.msg = '获取单一薪酬异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getAttendenceSettingAsync(state) {
    let result = {isOk: false, extension: {}, msg: '获取考勤金额设置信息失败'};
    let url = WebApiConfig.search.getAttendenceSettingList;

    try {
        let res = yield call(ApiClient.get, url);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ATTENDANCESETTINGLST), payload: res.data.extension});
        }
    } catch (e) {
        result.msg = '获取考勤金额设置信息失败';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* postAttendeceSettingAsync(state) {
    let result = {isOk: false, extension: {}, msg: '设置考勤金额设置信息失败！'};
    let url = WebApiConfig.server.postAttendenceSettingList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ATTENDANCESETTINGLST), payload: res.data.extension});

            notification.success({
                description: "设置考勤金额设置信息成功",
                duration: 3
            });
        }
    } catch (e) {
        result.msg = '设置考勤金额设置信息异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* getHumanlistByorgid(state) {
    let result = {isOk: false, extension: {}, msg: '获取组织下员工信息失败！'};
    let url = WebApiConfig.server.getHumanlistByorg;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_REWARDPUNISHHUMANLIST), payload: res.data.extension});

        }
    } catch (e) {
        result.msg = '获取组织下员工信息异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* searchRewardPunishmentLst(state) {
    let result = {isOk: false, extension: {}, msg: '查询奖惩信息失败！'};
    let url = WebApiConfig.search.getRPInfoList;

    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            result.isOk = true;
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_REWARDPUNISHMENTLIST), payload: res.data.extension});

        }
    } catch (e) {
        result.msg = '查询奖惩信息异常';
    }

    if (!result.isOk) {
        notification.error({
            description: result.msg,
            duration: 3
        });
    }
}

export function* searchtAttendenceLst(state) {
    let url = WebApiConfig.search.getAttendenceList
    let huResult = {isOk: false, msg: '查询考勤列表失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '查询考勤列表成功';

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ATTENDANCELST), payload: huResult.data.extension});
            
            return;
        }
    } catch (e) {
        huResult.data.message = "查询考勤列表接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}
export default function* watchAllSearchAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_CUSTOMER), getCustomerListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_CONDITION), getSearchConditionAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ALLHUMANINFO), getHumanListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), getMonthListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_BLACKLST), getBlackListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_SALARYLIST), getSalaryListAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_SALARYITEM), getSalaryItemAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_ATTENDANCELST), searchtAttendenceLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_ATTENDANCESETTINGLST), getAttendenceSettingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_ATTENDANCESETTINGLST), postAttendeceSettingAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GETSELHUMANLIST_BYORGID), getHumanlistByorgid);

    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_REWARDPUNISHMENT), searchRewardPunishmentLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.HUMAN_GET_DETAIL), getHumanDetailAsync);
}