import { BaseApiUrl, basicDataBaseApiUrl, UploadUrl } from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    },
    buildings: {
        List: basicDataBaseApiUrl + "buildings/list", // post获取楼盘列表
        buildings: basicDataBaseApiUrl + "buildings", // get 获取自己负责的楼盘
        buildingSearch: basicDataBaseApiUrl + "buildings/PostResponsibleBuildingSreach", // post 楼盘search
        GetThisBuildings: basicDataBaseApiUrl + "buildings/", // get 获取对应的楼盘
        getBuildingSreach: basicDataBaseApiUrl + "buildings/PostResponsibleBuildingSreach", // post 查询楼盘
        Commission: basicDataBaseApiUrl + "buildings/commission/", // put 楼盘佣金
        DeleteBuilding: basicDataBaseApiUrl + "buildings/", // delete 楼盘删除
    },
    buildingBasic: {
        Base: basicDataBaseApiUrl + "buildingbaseinfos",//楼盘基础信息
    },
    buildingSupport: {
        Base: basicDataBaseApiUrl + "buildingfacilities"//楼盘配套信息
    },
    buildingRelshop: {
        Base: basicDataBaseApiUrl + "buildingshopinfos"//楼盘整体商铺
    },
    buildingProject: {
        Base: basicDataBaseApiUrl + "buildings/summary"//楼盘简介
    },
    buildingrule: {
        Base: basicDataBaseApiUrl + "buildingrule",  // put 楼盘报备规则
        Template: basicDataBaseApiUrl + "",// 楼盘报备模板规则
    },
    buildingAttachInfo: {
        PicUpload: basicDataBaseApiUrl + "files/{dest}/uploadmore/",//楼盘图片上传//files/building/uploadmore/
        PicDelete: basicDataBaseApiUrl + "files/deletebuildingfile/"//楼盘图片删除
    },
    shopsAttachInfo: {
        PicUpload: basicDataBaseApiUrl + "files/shops/upload/",//商铺图片上传 post
        PicDelete: basicDataBaseApiUrl + "files/deleteshopsfile/"//商铺图片删除 post
    },
    shopBasic: {
        Base: basicDataBaseApiUrl + "shopsbaseinfos", // get 获取商铺基础信息   (添加商铺基础信息 修改商铺基础信息) ==> put
    },
    shopLease: {
        Base: basicDataBaseApiUrl + "shopsleaseinfos", // get 获取商铺租约信息  (添加商铺租约信息 修改商铺租约信息) ==> put
    },
    shopSupport: {
        Base: basicDataBaseApiUrl + "shopsfacilities", // get 获取商铺配套设施信息 (添加商铺配套设施信息 修改商铺配套设施信息) ==> put
    },
    shopSummary: {
        Base: basicDataBaseApiUrl + "Shops/summary", // (添加商铺简介 修改商铺简介) ==> put
    },
    shopsInfo: {
        Base: basicDataBaseApiUrl + 'Shops/audit/submit/', // post 提交商铺信息
        GetThisShops: basicDataBaseApiUrl + 'Shops/' // get 获取对应的商铺信息
    },
    shopsList: {
        List: basicDataBaseApiUrl + 'Shops/list', // post 获取商铺列表
        DeleteShop: basicDataBaseApiUrl + 'Shops/'// delete 删除
    },
    buildInfo: {
        Base: basicDataBaseApiUrl + 'buildings/audit/submit/', // post 提交楼盘信息
    },
    batchBuilding: {
        Base: basicDataBaseApiUrl + 'buildingNo/', // put 修改楼栋批次信息
    },
    customerTransactions: {
        Search: basicDataBaseApiUrl + 'customertransactions/Search', // POST 获取驻场自己的楼盘下客户的状态信息
        PLComfirmReport: basicDataBaseApiUrl + 'customertransactions/PostListCustomerReportByStatusCon', // 批量确认报备post
        PLReport: basicDataBaseApiUrl + 'customertransactions/PostListCustomerReportByStatusRep', // 批量向开发商报备post
        PLLook: basicDataBaseApiUrl + 'customertransactions/PostListCustomerReportByStatusBel', // 确认带看post
        MakeDeal: basicDataBaseApiUrl + 'customertransactions/PostListCustomerReportByStatusDea', // 确认成交post
        StatusCount: basicDataBaseApiUrl + 'customertransactions/transwaitconfirmcount', // 根据buildingId获取驻场待确认行数post'
        SearchValphone: basicDataBaseApiUrl + 'customertransactions/transactionbyids', // post 根据该楼盘是否需要向开发商展示真实号码来调接口
    },
    customerDeal: {
        CustomerDeal: basicDataBaseApiUrl + 'customerdeal', // post 提交成交信息
        GetCustomerDeal: basicDataBaseApiUrl + 'customerdeal/shopsdeal/', // get 根据楼商铺Id查询成交信息
        GetReportCustomerDeal: basicDataBaseApiUrl + 'customerdeal/transaction/', // get 根据报备流程ID查询成交信息
    },
    xk: {
        Base: basicDataBaseApiUrl + 'Shops/search', // post 获取商铺列表
        UpdateSalestatus: basicDataBaseApiUrl + 'Shops/updateshopssalestatus', // put 锁定和解除锁定
        Salestatistics: basicDataBaseApiUrl + 'Shops/salestatistics/', // get 销售统计
    },
    dynamic: {
        Base: basicDataBaseApiUrl + 'updaterecord', // post 房源动态的保存信息
        Examines: basicDataBaseApiUrl + 'examines/status/', //获取一个审核资源所有审核项目的当前审核状态
        List: basicDataBaseApiUrl + 'updaterecord/list', // post 获取最后一次审核信息
    },
    msg: {
        sendMsg: basicDataBaseApiUrl + 'buildingnotice',//发送房源消息
    },
    zcManagement: {
        siteuserlist: basicDataBaseApiUrl + 'buildings/siteuserlist', // get 获取负责驻场用户列表
        buildingsite: basicDataBaseApiUrl + 'buildings/buildingsite', // get 获取负责楼盘管理驻场用户
        saveonsite: basicDataBaseApiUrl + 'buildings/saveonsite', // put 指派驻场
        ExamineFlow: basicDataBaseApiUrl + 'examines/search', // post 指派驻场审核列表
    },
    attach: {
        uploadUrl: `${UploadUrl}/file/upload/`
    },
    userTypeValue: {
        Base: basicDataBaseApiUrl + 'userTypeValue', // put 更新当前用户历史楼盘
    }

}
export default WebApiConfig;