import {BaseApiUrl, basicDataBaseApiUrl} from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list",//区域数据
        // OrgList: BaseApiUrl + "Organization/allsons/",//获取子部门  ===>>>> 2018.1.26更改接口
        OrgList: BaseApiUrl + "Organization/sonsandmy/",//获取子部门
        OrgDetail: BaseApiUrl + "Organization/",//获取部门详细
        GetOrgUserList: BaseApiUrl + "user/list",//获取部门下的用户
        GetTreeList: basicDataBaseApiUrl + "businessplanning/treelist",//获取业态意向
    },
    search: {
        //getSaleManCustomerList: basicDataBaseApiUrl + 'customerInfo/listsaleman',// 业务员客户查询
        getSaleManCustomerList: basicDataBaseApiUrl + 'zyw/customerInfo/listsaleman/source',// 业务员客户查询
        getPoolCustomerList: basicDataBaseApiUrl + 'zyw/customerInfo/listpool',//公共池客户
        getDealCustomerList: basicDataBaseApiUrl + 'zyw/customerInfo/listdeal',//成交客户
        getLoosCustomerList: basicDataBaseApiUrl + 'zyw/customerInfo/listloss',//失效客户
        //getCustomerDetail: basicDataBaseApiUrl + 'customerInfo/',//加载客户详情
        getCustomerDetail: basicDataBaseApiUrl + 'zyw/customerInfo/retrieve/',//加载客户详情
        adjustCustomer: basicDataBaseApiUrl + 'zyw/customerInfo/transfercustomer',//调客
        getCustomerAllPhone: basicDataBaseApiUrl + 'zyw/customerInfo/GetUseridCustomerInfoPhone/',//获取所有电话号码
        getAuditList: basicDataBaseApiUrl + 'examines/submitlist',//提交的调客审核列表
        getRepeatJudgeInfo: basicDataBaseApiUrl + 'zyw/customerInfo/customerheavy',//获取重客判断信息
        getCustomerByUserID: basicDataBaseApiUrl + 'zyw/customerInfo/salesmancustomer/',//根据用户ID获取客户列表
        getAuditHistory: basicDataBaseApiUrl + "examines/",//获取审核历史详细
        customerloss: basicDataBaseApiUrl + "zyw/customerloss/customerloss",//拉无效客户post
        customerlossActive: basicDataBaseApiUrl + "zyw/customerloss/",//激活失效客户get
        getRepeatGroup: basicDataBaseApiUrl + 'zyw/customerInfo/listsalemanrepeat',//获取重客分组
        getCustomerByGroup: basicDataBaseApiUrl + 'zyw/customerInfo/listsalemanbyphone/',//根据分组获取重客列表
        getRepeatCount: basicDataBaseApiUrl + 'zyw/customerInfo/listsaleman/repetitioncount',//获取重客总量
    }
}
export default WebApiConfig;