import {takeEvery, takeLatest} from 'redux-saga'
import {put, call} from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import {notification} from 'antd';
import {createMergeHead, createColumData, writeMonthFile, MonthHead, writeHumanFile, HumanHead} from '../constants/export';

const actionUtils = appAction(actionTypes.ACTION_ROUTE)
const PositionStatus = ["未入职", "离职", "入职", "转正"];

export function* postHumanInfoAsync(state) {
    let urlhuman = WebApiConfig.server.PostHumaninfo;
    let humanResult = {isOk: false, msg: '人事信息提交失败！'};
    try {
        humanResult = yield call(ApiClient.post, urlhuman, state.payload, null, 'PUT');
        console.log("人事信息提交结果:", urlhuman, humanResult);
        //弹消息，返回
        if (humanResult.data.code == 0) {
            humanResult.isOk = true;
            humanResult.message = '人事信息提交成功';
            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
        }
    } catch (e) {
        humanResult.msg = "人事信息提交接口调用异常!";
    }

    notification[humanResult.isOk ? 'success' : 'error']({
        message: humanResult.msg,
        duration: 3
    });
}

export function* getWorkNumber(state) {
    let url = WebApiConfig.server.GetWorkNumber;
    let huResult = {isOk: false, msg: '获取工号失败！'};
    try {
        huResult = yield call(ApiClient.get, url);
        //弹消息，返回
        if (huResult.isOk) {
            huResult.message = '人事信息提交成功';

            yield put({type: actionUtils.getActionType(actionTypes.SET_HUMANINFONUMBER), payload: {worknumber: huResult}});
        }
    } catch (e) {
        huResult.msg = "部门用户获取接口调用异常!";
    }

    if (!huResult.isOk) {
        notification.error({
            message: huResult.msg,
            duration: 3
        });
    }
}

export function* recoverMonth(state) {
    let url = WebApiConfig.server.RecoverMonth;
    let huResult = {isOk: false, msg: '恢复月结失败！'};
    try {
        huResult = yield call(ApiClient.post, url, state.payload.last);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '恢复月结成功';

            yield put({type: actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), payload: state.payload.result});
        }
    } catch (e) {
        huResult.data.message = "恢复月结接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* createMonth(state) {
    let url = WebApiConfig.server.CreateMonth;
    let huResult = {isOk: false, msg: '创建月结失败！'};
    try {
        huResult = yield call(ApiClient.post, url, state.payload.last);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '创建月结成功';

            yield put({type: actionUtils.getActionType(actionTypes.MONTH_GETALLMONTHLIST), payload: state.payload.result});
        }
    } catch (e) {
        huResult.data.message = "创建月结接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setBlackLst(state) {
    let url = WebApiConfig.server.SetBlack;
    let huResult = {isOk: false, msg: '创建黑名单失败！'};
    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        //弹消息，返回
        if (huResult.data.code == 0) {
            huResult.data.message = '创建黑名单成功';
            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
        }
    } catch (e) {
        huResult.data.message = "创建黑名单接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* getcreateStation(state) {
    let url = WebApiConfig.search.getStationList + "/" + state.payload;
    let huResult = {isOk: false, msg: '获取职位失败！'};

    try {
        huResult = yield call(ApiClient.get, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '获取职位成功';
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        }
    } catch (e) {
        huResult.data.message = "获取职位接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* getorgcreateStation(state) {
    let url = WebApiConfig.search.getStationList + "/" + state.payload;
    let huResult = {isOk: false, msg: '获取职位失败！'};

    try {
        huResult = yield call(ApiClient.get, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '获取职位成功';
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_ORGSTATIONLIST), payload: huResult.data.extension});
        }
    } catch (e) {
        huResult.data.message = "获取职位接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setStation(state) {
    let url = WebApiConfig.server.SetStation;
    let huResult = {isOk: false, msg: '设置职位失败！'};
    let notifyType = 'success';
    let message = '设置职位成功';
    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data && huResult.data.code == 0) {
            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
            //yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        } else {
            notifyType = 'error';
            message = "设置职位失败";
        }
    } catch (e) {
        huResult.data.message = "设置职位接口调用异常!";
    }
    notification[notifyType]({
        message: message,
        duration: 3
    });
}

export function* deleteStation(state) {
    let url = WebApiConfig.server.DeleteStation;
    let huResult = {isOk: false, msg: '删除职位失败！'};
    let entity = (state.payload || {}).entity;
    let notifyType = 'success';
    let message = '删除职位成功';
    try {
        huResult = yield call(ApiClient.post, url, entity);
        if (huResult && huResult.data && huResult.data.code == 0) {
            // notification.success({
            //     message: '删除职位成功',
            //     duration: 3
            // });
            //yield put({ type: actionUtils.getActionType(actionTypes.UPDATE_STATIONLIST), payload: huResult.data.extension});
        } else {
            notifyType = 'error';
            message = "删除职位失败!";
        }
    } catch (e) {
        notifyType = 'error';
        message = "删除职位接口调用异常!";
    }
    notification[notifyType]({
        description: message,
        duration: 3
    });

}

export function* setSalary(state) {
    let url = WebApiConfig.server.setSalary;
    let huResult = {isOk: false, msg: '设置薪酬失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '设置薪酬成功';

            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
            notification.success({
                message: "设置成功",
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "设置薪酬接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* deleteSalary(state) {
    let url = WebApiConfig.server.deleteSalary;
    let huResult = {isOk: false, msg: '删除薪酬失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除薪酬成功';

            notification.success({
                message: "删除成功",
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除薪酬接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* getHumanImage(state) {
    let url = WebApiConfig.server.getHumanImage + '/' + state.payload;
    let huResult = {isOk: false, msg: '获取图片失败！'};

    try {
        huResult = yield call(ApiClient.get, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '获取图片成功';
            let f = [{uid: -1, name: "", status: 'done', url: huResult.data.extension.original}];
            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_HUMANIMAGE), payload: f});
        }
    } catch (e) {
        huResult.data.message = "获取图片接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* deleteBlackInfo(state) {
    let url = WebApiConfig.server.DeleteBlack;
    let huResult = {isOk: false, msg: '删除黑名单失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除黑名单成功';

            yield put({type: actionUtils.getActionType(actionTypes.DELETE_UPDATEBLACKINFO), payload: state.payload});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除黑名单接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* setSocialInsure(state) {
    let url = WebApiConfig.server.setSocialInsure;
    let huResult = {isOk: false, msg: '转正失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '转正成功';

            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "转正接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* leavePosition(state) {
    let url = WebApiConfig.server.leavePositon;
    let huResult = {isOk: false, msg: '离职失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '离职成功';

            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "离职接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* changeHuman(state) {
    let url = WebApiConfig.server.leavePositon;
    let huResult = {isOk: false, msg: '异动失败！'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '异动成功';

            yield put({type: actionUtils.getActionType(actionTypes.SET_USER_BREADITEMINDEX), payload: 0});

            notification.success({
                message: huResult.data.message,
                duration: 3
            });

            return;
        }
    } catch (e) {
        huResult.data.message = "离职接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* exportMonthForm(state) {
    let url = WebApiConfig.server.monthFormData;
    let huResult = {isOk: false, msg: '获取月结表信息异动失败！'};

    try {
        huResult = yield call(ApiClient.get, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '获取月结表信息异动成功';

            if (huResult.data.extension) {
                MonthHead[0].v = MonthHead[0].v + state.payload + "月结表";
                let f = createMergeHead(MonthHead);
                let ret = createColumData(f, huResult.data.extension);
                writeMonthFile(f, ret, "工资表", "月结.xlsx");
            }

            notification.success({
                message: huResult.data.message,
                duration: 3
            });

            return;
        }
    } catch (e) {
        huResult.data.message = "获取月结表信息接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

function findCascaderLst(id, tree, lst) {
    if (tree) {
        if (tree.id === id) {
            lst.unshift(tree.id);
            return true;
        } else if (tree.children && tree.children.length > 0) {
            if (tree.children.findIndex(org => this.findCascaderLst(id, org, lst)) !== -1) {
                lst.unshift(tree.id);
                return true;
            }
        }
    }
    return false;
}

export function* exportHumanForm(state) {
    let result = {isOk: false, extension: {}, msg: '检索关键字失败！'};
    let url = WebApiConfig.search.searchHumanList;
    try {
        let res = yield call(ApiClient.post, url, state.payload.data);
        if (res.data.code == 0) {
            result.isOk = true;
            let lv = res.data.extension;
            let data = lv.map(function (v, k) {
                let sn = "", fn = "";
                (v.sex == 1) && (sn = "男");
                (v.sex == 2) && (sn = "女");
                fn = v.staffStatus ? PositionStatus[v.staffStatus] : "未入职";

                let lstvalue = [];
                state.payload.tree.findIndex(
                    e => findCascaderLst(v.departmentId, e, lstvalue)
                );
                return {
                    a1: k, a2: v.id, a3: v.name, a4: sn, a5: v.idCard, a6: lstvalue.join('/'),
                    a7: v.positionName, a8: fn, a9: v.entryTime ? v.entryTime.replace("T", " ") : "",
                    a10: v.becomeTime ? v.becomeTime.replace("T", " ") : "",
                    a11: v.BaseSalary, a12: v.IsSocialInsurance ? "是" : "否", a13: v.contract ? "是" : "否"
                };
            });

            let f = createMergeHead(HumanHead);
            let ret = createColumData(f, data);
            writeHumanFile(f, ret, "人事员工表", "人事员工.xlsx");

            notification.success({
                message: "导出成功",
                duration: 3
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

export function* deleteOrgbyid(state) {
    let url = WebApiConfig.auth.deleteOrg + state.payload;
    let huResult = {isOk: false, msg: '删除组织失败!'};

    try {
        huResult = yield call(ApiClient.post, url, "", "", "DELETE");//应该是delete
        if (huResult.data.code == 0) {
            huResult.data.message = '删除组织成功';

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_DELETE_ORGBYID), payload: state.payload});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除组织接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* addOrg(state) {
    let url = WebApiConfig.auth.addupdateOrg;
    let huResult = {isOk: false, msg: '添加组织失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload.Original);
        if (huResult.data.code == 0) {
            huResult.data.message = '添加组织成功';

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_UPDATE_ORG), payload: state.payload});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "添加组织接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* updateOrg(state) {
    let url = WebApiConfig.auth.addupdateOrg + state.payload.id;
    let huResult = {isOk: false, msg: '更新组织失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload.Original);
        if (huResult.data.code == 0) {
            huResult.data.message = '更新组织成功';

            yield put({type: actionUtils.getActionType(actionTypes.UPDATE_UPDATE_ORG), payload: state.payload});
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "更新组织接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* importAttendenceLst(state) {
    let url = WebApiConfig.server.importAttendenceList
    let huResult = {isOk: false, msg: '导入考勤失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '导入考勤成功';

            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "导入考勤接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
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
            notification.success({
                message: huResult.data.message,
                duration: 3
            });
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

export function* deleteAttendenceItem(state) {
    let url = WebApiConfig.server.deleteAttendenceList + "/" + state.payload;
    let huResult = {isOk: false, msg: '删除考勤信息失败!'};

    try {
        huResult = yield call(ApiClient.post, url);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除考勤信息成功';

            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除考勤信息接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* addRewardPunishment(state) {
    let url = WebApiConfig.server.addRPInfo;
    let huResult = {isOk: false, msg: '添加行政奖惩失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '添加行政奖惩成功';

            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "添加行政奖惩接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}

export function* deleteRewardPunishment(state) {
    let url = WebApiConfig.server.addRPInfo + "/" + state.payload;
    let huResult = {isOk: false, msg: '删除行政奖惩失败!'};

    try {
        huResult = yield call(ApiClient.post, url, state.payload);
        if (huResult.data.code == 0) {
            huResult.data.message = '删除行政奖惩成功';

            notification.success({
                message: huResult.data.message,
                duration: 3
            });
            return;
        }
    } catch (e) {
        huResult.data.message = "删除行政奖惩接口调用异常!";
    }

    if (huResult.data.code != 0) {
        notification.error({
            message: huResult.data.message,
            duration: 3
        });
    }
}
//保存兼职信息
export function* savePartTimeJob(state) {
    // let url = WebApiConfig.server.addRPInfo + "/" + state.payload;
    let url = WebApiConfig.server.savePartTimeJob;
    let huResult = {isOk: false, msg: '保存兼职信息失败!'};
    try {
        let res = yield call(ApiClient.post, url, state.payload);
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.message = '保存兼职信息成功';

        }
    } catch (e) {
        huResult.message = "保存兼职信息接口调用异常!";
    }
    notification[huResult.isOk ? "success" : "error"]({
        message: huResult.message,
        duration: 3
    });
}

export default function* watchDicAllAsync() {
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_HUMANINFO), postHumanInfoAsync);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_HUMANINFONUMBER), getWorkNumber);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_RECOVER), recoverMonth);
    yield takeLatest(actionUtils.getActionType(actionTypes.MONTH_CREATE), createMonth);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_ADDBLACKLST), setBlackLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CRATESTATION), getcreateStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_CRATEORGSTATION), getorgcreateStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.SET_STATION), setStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_STATION), deleteStation);
    yield takeLatest(actionUtils.getActionType(actionTypes.SET_SALARYINFO), setSalary);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_SALARYINFO), deleteSalary);
    yield takeLatest(actionUtils.getActionType(actionTypes.GET_HUMANIMAGE), getHumanImage);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_BLACKINFO), deleteBlackInfo);
    yield takeLatest(actionUtils.getActionType(actionTypes.HUMAN_BECOME_STAFF), setSocialInsure);
    yield takeLatest(actionUtils.getActionType(actionTypes.LEAVE_POSITON), leavePosition);
    yield takeLatest(actionUtils.getActionType(actionTypes.POST_CHANGEHUMAN), changeHuman);
    yield takeLatest(actionUtils.getActionType(actionTypes.HUMAN_PARTTIME_JOB_SAVE), savePartTimeJob)
    //导表
    yield takeLatest(actionUtils.getActionType(actionTypes.EXPORT_MONTHFORM), exportMonthForm);
    yield takeLatest(actionUtils.getActionType(actionTypes.EXPORT_HUMANFORM), exportHumanForm);
    //组织
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_ORGBYID), deleteOrgbyid);
    yield takeLatest(actionUtils.getActionType(actionTypes.ADD_ORG), addOrg);
    yield takeLatest(actionUtils.getActionType(actionTypes.UPDATE_ORG), updateOrg);
    //考勤
    yield takeLatest(actionUtils.getActionType(actionTypes.IMPORT_ATTENDANCELST), importAttendenceLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_ATTENDANCELST), searchtAttendenceLst);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_ATTENDANCEITEM), deleteAttendenceItem);
    //行政惩罚
    yield takeLatest(actionUtils.getActionType(actionTypes.ADD_REWARDPUNISHMENT), addRewardPunishment);
    yield takeLatest(actionUtils.getActionType(actionTypes.DELTE_REWARDPUNISHMENT), deleteRewardPunishment);
}