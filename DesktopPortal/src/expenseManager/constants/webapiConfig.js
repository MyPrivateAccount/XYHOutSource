import { BaseApiUrl, basicDataBaseApiUrl, UploadUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list",//区域数据
        permissionOrg: BaseApiUrl + 'Permission/'//有权限的部门,
    },
    msg: {
        sendMsg: basicDataBaseApiUrl + 'buildingnotice',//发送房源消息
    },
    attach: {
        uploadUrl: `${UploadUrl}/file/upload/`
    },
    userTypeValue: {
        Base: basicDataBaseApiUrl + 'userTypeValue', // put 更新当前用户历史楼盘
    },
    server: {
        uploadImg: basicDataBaseApiUrl + "chargeinfo/uploadmore/",
        addCharge: basicDataBaseApiUrl + "chargeinfo/addcharge/",
        searchCharge: basicDataBaseApiUrl + "chargeinfo/searchchargeinfo/",
        getRecieptInfo: basicDataBaseApiUrl + "chargeinfo/getrecipt/",
        getChargeid: basicDataBaseApiUrl + "chargeinfo/chargeid/",
        updatePostTime: basicDataBaseApiUrl + "chargeinfo/paymentcharge/",
        postreciept: basicDataBaseApiUrl + "chargeinfo/setrecieptinfo/",
        limitChargeHum: basicDataBaseApiUrl + "chargeinfo/limithuman/",
    },
}
export default WebApiConfig;