import {BaseApiUrl, basicDataBaseApiUrl} from '../../constants/baseConfig';
import contract from '../../contractManagement/reducers/contract';

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
        getZYWUpdateRecordDetail: basicDataBaseApiUrl + 'zyw/updaterecord/',//获取租壹屋房源动态详细
        getDealInfo: basicDataBaseApiUrl + 'customerdeal/',//获取成交详细信息
        getZYWDealInfo: basicDataBaseApiUrl + 'zyw/customerdeal/',//获取租壹屋成交详细信息
    },
    houseActive: {
        buildingDetail: basicDataBaseApiUrl + "buildings/",//楼盘详情
        buildingShops: basicDataBaseApiUrl + "Shops/search",//"Shops/list",//楼盘商铺获取
        zywBuildingDetail: basicDataBaseApiUrl + "zyw/buildings/",//楼盘详情
        zywBuildingShops: basicDataBaseApiUrl + "zyw/Shops/search",//"Shops/list",//楼盘商铺获取
        getShopDetail: basicDataBaseApiUrl + 'Shops/',//获取商铺详情
        getZYWShopDetail: basicDataBaseApiUrl + 'zyw/Shops/',//获取商铺详情
    },
    contract: {
        modifyDetail: basicDataBaseApiUrl + 'contractinfo/getmodifyinfobyid/', //获取合同内容的部分详情
        getContractDetail: basicDataBaseApiUrl + 'contractinfo/',
    }
}
export default WebApiConfig;