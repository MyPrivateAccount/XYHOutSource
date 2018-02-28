import { BaseApiUrl, basicDataBaseApiUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    audit: {
        myAudit: {
            getWaitAuditList: basicDataBaseApiUrl + 'examines/waiting',//获取待审核列表
            getAuditedList: basicDataBaseApiUrl + "examines/list",//我参与的审核列表
        },
        mySubmit: basicDataBaseApiUrl + 'examines/submitlist',//我发起的审核列表
        coypToMe: basicDataBaseApiUrl + 'examinenotices/list',//抄送给我的
        passAudit: basicDataBaseApiUrl + 'examines/pass/',//通过审核
        rejectAudit: basicDataBaseApiUrl + 'examines/reject/',//驳回审核
        getAuditHistory: basicDataBaseApiUrl + "examines/",//获取审核历史详细
        getNoReadCount: basicDataBaseApiUrl + 'examinenotices',//获取知会未读总数
        getUpdateRecordDetail: basicDataBaseApiUrl + 'updaterecord/',//获取房源动态详细
    },
    houseActive: {
        buildingDetail: basicDataBaseApiUrl + "buildings/",//楼盘详情
        buildingShops: basicDataBaseApiUrl + "Shops/search",//"Shops/list",//楼盘商铺获取
    }
}
export default WebApiConfig;