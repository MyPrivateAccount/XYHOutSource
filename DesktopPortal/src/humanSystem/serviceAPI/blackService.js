import {notification} from 'antd';
import ApiClient from '../../utils/apiClient'
import WebApiConfig from '../constants/webapiConfig';

//新增黑名单
export function addBlackLst(entity) {
    let url = WebApiConfig.server.SetBlack;
    let huResult = {isOk: false, msg: '创建黑名单失败！'};
    console.log("新增黑名单请求体:", entity);
    return ApiClient.post(url, entity, null, 'PUT').then(res => {
        if (res.data.code == 0) {
            huResult.isOk = true;
            huResult.msg = '创建黑名单成功';
            huResult.extension = res.data.extension || {};
        } else {
            huResult.msg = res.data.message;
        }
    }).catch(e => {
        huResult.msg = "创建黑名单接口调用异常:" + e.message;
    }).then(res => {
        notification[huResult.isOk ? 'success' : 'error']({
            message: huResult.msg,
            duration: 3
        });
        return huResult;
    });
}
