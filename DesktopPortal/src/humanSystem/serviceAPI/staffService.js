import {notification} from 'antd';
import ApiClient from '../../utils/apiClient'
import WebApiConfig from '../constants/webapiConfig';

//获取员工列表
export function getHumanList(entity) {
    let result = {isOk: false, extension: [], msg: '员工查询失败！'};
    let url = WebApiConfig.search.searchHumanList;
    if (entity.staffStatuses && entity.staffStatuses.includes('0')) {
        entity.staffStatuses = [];
    } else {
        entity.staffStatuses = [entity.staffStatuses]
    }
    return ApiClient.post(url, entity).then(res => {
        if (res.data.code == 0) {
            result.isOk = true;
            result.extension = res.data.extension;
            result.pageIndex = res.data.pageIndex;
            result.pageSize = res.data.pageSize;
            result.totalCount = res.data.totalCount;
        } else {
            result.msg = res.data.message;
        }
    }).catch(e => {
        result.msg = '员工查询接口调用异常!';
    }).then(res => {
        if (!result.isOk) {
            notification.error({
                description: result.msg,
                duration: 3
            });
        }
        return result;
    });
}
//获取员工详情
export function getHumanDetail(humenId) {
    let result = {isOk: false, extension: {}, msg: '获取员工详情失败!'};
    let url = WebApiConfig.search.getHumanDetail + humenId;
    return ApiClient.get(url).then(res => {
        if (res.data.code == 0) {
            result.isOk = true;
            result.msg = "获取员工详情成功";
            result.extension = res.data.extension;
            console.log("员工详情获取结果:", res);
        } else {
            result.msg = res.data.message;
        }
    }).catch(e => {
        result.msg = '获取员工详情接口异常!';
    }).then(res => {
        if (!result.isOk) {
            notification.error({
                description: result.msg,
                duration: 3
            });
        }
        return result;
    });
}
//移动调薪保存接口
export function adjustHuman(entity) {
    let url = WebApiConfig.server.adjustHuman;
    let huResult = {isOk: false, msg: '异动调薪失败！'};
    return ApiClient.post(url, entity).then(res => {
        console.log("移动调薪结果:", res);
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '异动调薪保存成功';
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "异动调薪接口调用异常!";
    }).then(res => {
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}
//保存人事信息
export function postHumanInfo(entity) {
    let urlhuman = WebApiConfig.server.PostHumaninfo;
    let humanResult = {isOk: false, msg: '人事信息提交失败！'};
    return ApiClient.post(urlhuman, entity, null, 'PUT').then(res => {
        console.log("人事信息提交结果:", res);
        if (res.data.code == 0) {
            humanResult.isOk = true;
            humanResult.msg = '人事信息提交成功';
        } else {
            humanResult.msg = res.data.message;
        }
    }).catch(e => {
        humanResult.msg = "人事信息提交接口调用异常!";
    }).then(res => {
        notification[humanResult.isOk ? 'success' : 'error']({
            message: humanResult.msg,
            duration: 3
        });
        return humanResult;
    });
}
//获取职位列表
export function getPosition(departmentId) {
    let url = WebApiConfig.search.getStationList + "/" + departmentId;
    let huResult = {isOk: false, extension: [], msg: '获取职位失败！'};
    return ApiClient.get(url).then(res => {
        console.log("请求结果:", res);
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '获取职位成功';
            huResult.extension = res.data.extension || [];
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "获取职位接口调用异常!";
    }).then(res => {
        if (!huResult.isOk) {
            notification.error({
                message: huResult.msg,
                duration: 3
            });
        }
        return huResult;
    });
}
//离职操作
export function leavePosition(entity) {
    let url = WebApiConfig.server.leavePositon;
    let huResult = {isOk: false, msg: '离职失败！'};
    return ApiClient.post(url, entity).then(res => {
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '离职成功';
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "离职接口调用异常:" + e.message;
    }).then(res => {
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}

//保存兼职信息
export function savePartTimeJob(entity) {
    let url = WebApiConfig.server.savePartTimeJob;
    let huResult = {isOk: false, msg: '保存兼职信息失败!'};
    return ApiClient.post(url, entity).then(res => {
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '保存兼职信息成功';
            huResult.extension = res.data.extension;
        }
        else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "保存兼职信息接口调用异常!";
    }).then(res => {
        notification[huResult.isOk ? "success" : "error"]({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}

//查询兼职信息
export function getPartTimeJobById(userId) {
    let url = WebApiConfig.server.getPartTimeJobList + userId;
    let huResult = {isOk: false, msg: '获取兼职列表失败!'};
    return ApiClient.get(url).then(res => {
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '获取兼职列表成功';
            huResult.extension = res.data.extension;
        }
        else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "获取兼职列表接口调用异常:" + e.message;
    }).then(res => {
        notification[huResult.isOk ? "success" : "error"]({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}

//删除兼职信息
export function removePartTimeJob(jobId) {
    let url = WebApiConfig.server.removePartTimeJob + jobId;
    let huResult = {isOk: false, msg: '删除兼职信息失败!'};
    return ApiClient.post(url, null, null, 'DELETE').then(res => {
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '删除兼职信息成功';
        }
        else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "删除兼职信息接口调用异常!";
    }).then(res => {
        notification[huResult.isOk ? "success" : "error"]({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}