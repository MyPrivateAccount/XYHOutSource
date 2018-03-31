import { BaseApiUrl, basicDataBaseApiUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list",//区域数据
        // OrgList: BaseApiUrl + "Organization/allsons/",//获取子部门  ===>>>> 2018.1.26更改接口
        OrgList: BaseApiUrl + "Organization/sonsandmy/",//获取子部门
        OrgDetail: BaseApiUrl + "Organization/",//获取部门详细
        GetOrgUserList: BaseApiUrl + "user/list",//获取部门下的用户
    },
    search: {
        searchWordHumanList: basicDataBaseApiUrl + 'humaninfo/search',// 业务员客户查询
        getAuditList: basicDataBaseApiUrl + 'examines/submitlist',//提交的调客审核列表
        getRepeatJudgeInfo: basicDataBaseApiUrl + 'customerInfo/customerheavy/',//获取重客判断信息
        getCustomerByUserID: basicDataBaseApiUrl + 'customerInfo/salesmancustomer/',//根据用户ID获取客户列表
        getAuditHistory: basicDataBaseApiUrl + "examines/",//获取审核历史详细
    }
}
export default WebApiConfig;