import { BaseApiUrl, basicDataBaseApiUrl, UploadUrl, FlowChartApiUrl } from '../../constants/baseConfig';

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
        Permission: FlowChartApiUrl + 'Permission/retrivepermissionusers/',
        Base: basicDataBaseApiUrl + 'userTypeValue', // put 更新当前用户历史楼盘
    },
    server: {
        uploadImg: basicDataBaseApiUrl + "chargeinfo/charge/uploadmore/",
        addCharge: basicDataBaseApiUrl + "chargeinfo/addcharge/",
        searchCharge: basicDataBaseApiUrl + "chargeinfo/searchchargeinfo/",
        getRecieptInfo: basicDataBaseApiUrl + "chargeinfo/getrecipt/",
        getChargeid: basicDataBaseApiUrl + "chargeinfo/chargeid/",
        updatePostTime: basicDataBaseApiUrl + "chargeinfo/paymentcharge/",
        postreciept: basicDataBaseApiUrl + "chargeinfo/setrecieptinfo/",
        limitChargeHum: basicDataBaseApiUrl + "chargeinfo/limithuman/",
        getChargeDetail: basicDataBaseApiUrl + "chargeinfo/chargedetail/",
    },
}
export default WebApiConfig;