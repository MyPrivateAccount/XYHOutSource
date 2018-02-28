import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    pagination: {},
    searchResult: [],//查询结果
    activeBuilding: {},//选中的楼盘
    activeBuildingShops: [],//选中楼盘的商铺
    activeShop: {},//商铺详细
    showLoading: false,
    expandSearchBox: true,//查询条件展开、隐藏
    showResult: {
        showBuildingList: false,
        showBuildingDetal: false,
        showShopDetail: false,
        showMsgList: false,
        showMsgDetail: false,
        navigator: []//详细页面导航
    },
    hasRegionRecommendPermission: false,//大区推荐权限
    hasFilialeRecommendPermission: false,//公司推荐权限
    recommendDialogVisible: false,//推荐对话框显示
    recommendList: [],
    customerDeal: [], // 成交信息
    msgList: [],
    msgDetail: {},
    dynamicData: [], // 房源动态列表
};
let reducerMap = {};
//查询结果
reducerMap[actionTypes.SEARCH_XYH_BUILDING_COMPLETE] = function (state, action) {
    //console.log("reducer", action.payload);
    let searchResult = action.payload;
    if (searchResult.extension.length === 0) {
        searchResult.extension.push({ id: '00000000', name: '没有找到符合要求的数据!' });//
    }
    return Object.assign({}, state, {
        showResult: { showBuildingList: true, showBuildingDetal: false, showShopDetail: false, navigator: [] },
        searchResult: searchResult.extension,
        activeBuilding: {},
        pagination: { pageIndex: action.payload.pageindex, pageSize: action.payload.pageSize, totalCount: action.payload.totalCount }
    });
}
//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}
//查看楼盘详细
reducerMap[actionTypes.XYH_GET_BUILDING_COMPLETE] = function (state, action) {
    let navigator = [];
    if (Object.keys(action.payload).length > 0) {
        navigator = [{ id: action.payload.id, name: action.payload.basicInfo.name, type: 'buildingDetail' }];
    }
    return Object.assign({}, state, {
        showResult: { showBuildingList: false, showBuildingDetal: true, showShopDetail: false, navigator: navigator },
        activeBuilding: action.payload
    });
}
//关闭详情
reducerMap[actionTypes.RESULT_CLOSE] = function (state, action) {
    return Object.assign({}, state, { expandSearchBox: true, showResult: { showBuildingList: true, showBuildingDetal: false, showShopDetail: false, navigator: [] }, activeBuilding: {}, activeBuildingShops: [] });
}
//上一个
reducerMap[actionTypes.RESULT_PREV_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { activeBuilding: action.payload });
}
//下一个
reducerMap[actionTypes.RESULT_NEXT_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { activeBuilding: action.payload });
}
//搜索条件隐藏/展开
reducerMap[actionTypes.SEARCH_BOX_EXPAND] = function (state, action) {

    return Object.assign({}, state, { expandSearchBox: action.payload });
}
//商铺列表加载完成
reducerMap[actionTypes.GET_BUILDING_SHOPS_COMPLETE] = function (state, action) {
    let navigator = [...state.showResult.navigator];
    return Object.assign({}, state, {
        activeBuildingShops: action.payload,
        showResult: { showBuildingList: false, showBuildingDetal: true, showShopDetail: false, navigator: navigator }
    });
}
//商铺详情加载完成
reducerMap[actionTypes.GET_BUILDING_SHOPS_DETAIL_COMPLETE] = function (state, action) {
    let navigator = [...state.showResult.navigator];
    if (Object.keys(action.payload).length > 0) {
        navigator.push({ id: action.payload, name: action.payload.basicInfo.buildingNo, type: 'shopDetail' });
    }
    let showResult = { showBuildingList: false, showBuildingDetal: false, showShopDetail: true, navigator: navigator };
    return Object.assign({}, state, { activeShop: action.payload, showResult: showResult });
}

reducerMap[actionTypes.RESULT_VIEW_BACK] = function (state, action) {
    let showResult = { ...state.showResult, ...action.payload };
    return Object.assign({}, state, { showResult: showResult });
}
//推荐权限获取完成
reducerMap[actionTypes.GET_PERMISSION_COMPLETE] = function (state, action) {
    let hasRegionRecommendPermission = state.hasRegionRecommendPermission;
    let hasFilialeRecommendPermission = state.hasFilialeRecommendPermission;
    if (action.payload.permission) {
        if (action.payload.permission.includes("RECOMMEND_REGION")) {
            hasRegionRecommendPermission = action.payload.extension;
        }
        if (action.payload.permission.includes("RECOMMEND_FILIALE")) {
            hasFilialeRecommendPermission = action.payload.extension;
        }
    }
    return Object.assign({}, state, {
        hasRegionRecommendPermission: hasRegionRecommendPermission,
        hasFilialeRecommendPermission: hasFilialeRecommendPermission
    });
}

reducerMap[actionTypes.OPEN_RECOMMEND_DIALOG] = function (state, action) {
    return Object.assign({}, state, { recommendDialogVisible: true, activeBuilding: action.payload });
}
reducerMap[actionTypes.CLOSE_RECOMMEND_DIALOG] = function (state, action) {
    return Object.assign({}, state, { recommendDialogVisible: false });
}
//推荐列表加载完成
reducerMap[actionTypes.GET_RECOMMEND_LIST_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { recommendList: action.payload });
}
//成交信息获取完成
reducerMap[actionTypes.GET_CUSTOMER_DEAL_INFO_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { customerDeal: action.payload });
}
//打开消息列表页面
reducerMap[actionTypes.OPEN_MSG_LIST] = function (state, action) {
    let showResult = { ...state.showResult };
    showResult.showBuildingList = false;
    showResult.showBuildingDetal = false;
    showResult.showShopDetail = false;
    showResult.showMsgList = true;
    showResult.showMsgDetail = false;
    showResult.navigator.push({ id: "msgList", name: "消息列表", type: "msgList" });
    return Object.assign({}, state, { showResult: showResult });
}
//获取消息列表
reducerMap[actionTypes.GET_MSG_LIST_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { msgList: action.payload });
}
//打开消息详细页面
reducerMap[actionTypes.OPEN_MSG_DETAIL] = function (state, action) {
    let showResult = { ...state.showResult };
    showResult.showBuildingList = false;
    showResult.showBuildingDetal = false;
    showResult.showShopDetail = false;
    showResult.showMsgList = false;
    showResult.showMsgDetail = true;
    if (action.payload) {
        showResult.navigator.push({ id: action.payload.id, name: action.payload.title, type: "msgDetail" });
    }
    return Object.assign({}, state, { showResult: showResult, msgDetail: action.payload });
}
//获取消息详细
reducerMap[actionTypes.GET_MSG_DETAIL_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { msgDetail: action.payload });
}
//导航切换
reducerMap[actionTypes.NAV_CHANGE] = function (state, action) {
    let showResult = { ...state.showResult };
    let curNav = action.payload, navigator = state.showResult.navigator;
    if (curNav) {
        let index = navigator.findIndex(n => n.id === curNav.id);
        if (index > -1 && index != (navigator.length - 1)) {
            //处理显示信息
            if (curNav.type === "buildingDetail") {
                showResult.showBuildingList = false;
                showResult.showBuildingDetal = true;
                showResult.showShopDetail = false;
                showResult.showMsgList = false;
                showResult.showMsgDetail = false;
            } else if (curNav.type === "msgList") {
                showResult.showBuildingList = false;
                showResult.showBuildingDetal = false;
                showResult.showShopDetail = false;
                showResult.showMsgList = true;
                showResult.showMsgDetail = false;
            }
            //处理导航信息
            navigator.splice(index + 1);
            showResult.navigator = navigator;
        }
    }
    return Object.assign({}, state, { showResult: showResult });
}

reducerMap[actionTypes.GET_ACTIVE_LIST_START] = function (state, action) {
    let dynamicData = []
    return Object.assign({}, state, { dynamicData: dynamicData });
}

reducerMap[actionTypes.GET_ACTIVE_LIST_END] = function (state, action) {
    let dynamicData = []
    let res = action.payload.data
    if (res.code === '0') {
        dynamicData = res.extension
        dynamicData.totalCount = res.totalCount
    }
    return Object.assign({}, state, { dynamicData: dynamicData });
}

export default handleActions(reducerMap, initState);