import {notification} from 'antd';
import ApiClient from '../../utils/apiClient'
import WebApiConfig from '../constants/webapiConfig';

//获取指定部门的用户列表
export function getOrgUserList(condition) {
    let result = {isOk: false, extension: [], msg: '部门用户获取失败！'};
    let url = WebApiConfig.dic.GetOrgUserList;
    return ApiClient.post(url, condition).then(res => {
        if (res.data.code == '0') {
            result.isOk = true;
            result.msg = '部门下用户列表获取成功';
            result.extension = res.data.extension || [];
        }else {
            result.msg = res.data.message;
        }
    }).catch(e => {
        result.msg = "部门用户获取接口调用异常!";
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