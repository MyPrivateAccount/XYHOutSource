import {notification} from 'antd';
import ApiClient from '../../utils/apiClient'
import WebApiConfig from '../constants/webapiConfig';

//获取薪酬列表
export function getSalaryList(condition) {
    let result = {isOk: false, extension: {}, msg: '薪酬列表查询失败！'};
    let url = WebApiConfig.search.getSalaryList;
    return ApiClient.post(url, condition).then(res => {
        if (res.data.code == 0) {
            result.isOk = true;
            result.msg = '薪酬列表查询成功';
            result.extension = {
                extension: res.data.extension || [],
                pageIndex: res.data.pageIndex,
                pageSize: res.data.pageSize,
                totalCount: res.data.totalCount
            };
        } else {
            result.msg = '薪酬列表查询失败:' + res.data.message;
        }
    }).catch(e => {
        result.msg = '薪酬列表接口调用异常:' + e.message;
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

//新增薪酬信息
export function addSalary(entity) {
    let url = WebApiConfig.server.setSalary;
    let huResult = {isOk: false, msg: '保存薪酬失败！'};
    return ApiClient.post(url, entity).then(res => {
        console.log("保存薪酬:", res);
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '保存薪酬成功';
            huResult.extension = res.data.extension || {};
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "保存薪酬接口调用异常:" + e.message;
    }).then(res => {
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}
//编辑薪酬信息
export function editSalary(entity) {
    let url = WebApiConfig.server.editSalary + entity.id;
    let huResult = {isOk: false, msg: '保存薪酬失败！'};
    return ApiClient.post(url, entity, null, 'PUT').then(res => {
        console.log("保存薪酬:", res);
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '保存薪酬成功';
            huResult.extension = res.data.extension || {};
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "保存薪酬接口调用异常:" + e.message;
    }).then(res => {
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}