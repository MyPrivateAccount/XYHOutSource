import {BaseApiUrl, basicDataBaseApiUrl} from '../../constants/baseConfig';

const WebApiConfig = {
    dic: {
        ParList: basicDataBaseApiUrl + 'dictionarydefines/list',// 获取字典类型列表 get
        AreaList: basicDataBaseApiUrl + "areadefines/list"//区域数据
    },
    search: {
        getMyRecommendList: basicDataBaseApiUrl + 'zyw/buildingRecommend/GetMyBuildingRecommendList',//获取我推荐的
        responsibleSearch: basicDataBaseApiUrl + 'zyw/buildings/PostResponsibleBuildingSreach',//负责楼盘查询
        //xyhBuildingSearch: basicDataBaseApiUrl + "buildings/list",//楼盘查询
        xyhBuildingDetail: basicDataBaseApiUrl + "zyw/buildings/",//楼盘详情
        xyhBuildingShops: basicDataBaseApiUrl + "zyw/Shops/search",//"Shops/list",//楼盘商铺获取
        xyhBuildingShopDetail: basicDataBaseApiUrl + "zyw/Shops/",//商铺详情
        xyhHasPermission: BaseApiUrl + "Permission",//
        buildingRecommend: basicDataBaseApiUrl + 'zyw/buildingRecommend',//楼盘推荐
        getRecommendList: basicDataBaseApiUrl + 'zyw/buildingRecommend/GetMyBuildingRecommendList',//获取推荐列表
        cancelRecommend: basicDataBaseApiUrl + 'zyw/buildingRecommend/delete',//取消推荐
        GetCustomerDeal: basicDataBaseApiUrl + 'zyw/customerdeal/shopsdeal/'//获取成交信息
    },
    msg: {
        getMsgList: basicDataBaseApiUrl + 'zyw/buildingnotice/list',//获取消息列表
        getMsgDetail: basicDataBaseApiUrl + 'zyw/buildingnotice/',//获取消息详细
    }
}
export default WebApiConfig;