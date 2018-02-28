import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    myRecommendList: [],//我推荐的楼盘
    pagination: {},
    searchResult: [],//查询结果
    activeBuilding: {},//选中的楼盘
    showLoading: false,
    showResult: {
        showBuildingList: false,
        navigator: []//详细页面导航
    },
    activeBuildingShops: [],
    activeShop:{},
    recommendDialogVisible: false,//推荐对话框显示
    msgList: [],
    msgDetail: {}
};
let reducerMap = {};
//查询结果
reducerMap[actionTypes.SEARCH_XYH_BUILDING_COMPLETE] = function (state, action) {
    console.log("查询结果:", action.payload);
    let searchResult = action.payload;
    let totalCount = 0;
    if (searchResult.length === 0) {
        searchResult.push({id: '00000000', name: '没有找到符合要求的数据!'});
        totalCount = 0;
    } else {
        for (let i = 0; i < searchResult.length; i++) {
            let build = searchResult[i];
            searchResult[i] = Object.assign({}, build.basicInfo, build.facilitiesInfo, {id: build.id});
        }
        searchResult = filterSearchResult(searchResult, state.myRecommendList);
        totalCount = searchResult.length;
    }
    return Object.assign({}, state, {
        showResult: {showBuildingList: true, showBuildingDetal: false, showShopDetail: false, navigator: []},
        searchResult: searchResult,
        activeBuilding: {},
        pagination: {pageIndex: 1, pageSize: 10, totalCount: totalCount, myRecommendTotalCount: (state.myRecommendList || []).length}
    });
}
//获取我推荐的楼盘完成
reducerMap[actionTypes.GET_MYCOMMEND_BUILDING_COMPLETE] = function (state, action) {
    let myRecommendList = action.payload || [];
    let searchResult = state.searchResult || [];
    let pagination = state.pagination;
    let newMyRecommendList = [];
    myRecommendList = myRecommendList.filter(r => r.isOutDate === false);
    //由于大区推荐和公司推荐会有两条记录,所以这里需要做一下合并
    let recommendBuilds = myRecommendList.map(r => r.buildingId); console.log("推荐的楼盘列表original:", JSON.stringify(recommendBuilds));
    recommendBuilds = Array.from(new Set(recommendBuilds));
    for (let i = 0; i < recommendBuilds.length; i++) {
        let buildId = recommendBuilds[i];
        let result = myRecommendList.filter(buildInfo => buildInfo.buildingId === buildId);
        if (result.length === 1) {
            if (result[0].isRegion) {
                result[0].regionRecommendID = result[0].id;
            } else {
                result[0].filialeRecommendID = result[0].id;
            }
            result[0].isFiliale = !result[0].isRegion;
            newMyRecommendList.push(result[0]);
        } else if (result.length === 2) {
            let regionRecommendID = result[0];
            if (result[0].isRegion) {
                result[0].regionRecommendID = result[0].id;
                result[0].filialeRecommendID = result[1].id;
            } else {
                result[0].regionRecommendID = result[1].id;
                result[0].filialeRecommendID = result[0].id;
            }
            result[0].isRegion = true;
            result[0].isFiliale = true;
            newMyRecommendList.push(result[0]);
        }
    }
    myRecommendList = newMyRecommendList; console.log("格式化后的推荐列表", newMyRecommendList);
    searchResult = filterSearchResult(searchResult, myRecommendList);
    let totalCount = searchResult.length;
    if (searchResult.length > 0 && searchResult[0].id === "00000000") {
        totalCount = 0;
    }
    return Object.assign({}, state, {
        searchResult: searchResult,
        myRecommendList: myRecommendList,
        pagination: {...state.pagination, totalCount: totalCount, myRecommendTotalCount: myRecommendList.length}
    });
}
//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, {showLoading: action.payload});
}

reducerMap[actionTypes.OPEN_RECOMMEND_DIALOG] = function (state, action) {
    return Object.assign({}, state, {recommendDialogVisible: true, activeBuilding: action.payload});
}
reducerMap[actionTypes.CLOSE_RECOMMEND_DIALOG] = function (state, action) {
    return Object.assign({}, state, {recommendDialogVisible: false});
}
//推荐列表加载完成
reducerMap[actionTypes.GET_RECOMMEND_LIST_COMPLETE] = function (state, action) {
    return Object.assign({}, state, {recommendList: action.payload});
}
//导航切换
reducerMap[actionTypes.NAV_CHANGE] = function (state, action) {
    let showResult = {...state.showResult};
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
    return Object.assign({}, state, {showResult: showResult});
}

//过滤已推荐
function filterSearchResult(searchResultList, myRecommendList) {
    let searchResult = [...searchResultList];
    if (searchResult && myRecommendList) {
        let recommendIdList = myRecommendList.map(r => {return r.buildingId});
        console.log("推荐的楼盘列表:", JSON.stringify(recommendIdList));
        for (let i = searchResult.length - 1; i > -1; i--) {
            let buildInfo = searchResult[i];
            if (recommendIdList.includes(buildInfo.id)) {
                searchResult.splice(i, 1);
                console.log("找到的推荐:", buildInfo);
            }
        }
    }
    return searchResult;
}

//商铺列表加载完成
reducerMap[actionTypes.GET_BUILDING_SHOPS_COMPLETE] = function (state, action) {
    let navigator = [...state.showResult.navigator];
    return Object.assign({}, state, {
        activeBuildingShops: action.payload,
        showResult: {showBuildingList: false, showBuildingDetal: true, showShopDetail: false, navigator: navigator}
    });
}

//查看楼盘详细
reducerMap[actionTypes.GET_BUILDING_DETAIL_COMPLETE] = function (state, action) {
    console.log("reducer,楼盘详细:", action.payload);
    let navigator = [];
    if (Object.keys(action.payload).length > 0) {
        navigator = [{id: action.payload.id, name: action.payload.basicInfo.name, type: 'buildingDetail'}];
    }
    return Object.assign({}, state, {
        showResult: {showBuildingList: false, showBuildingDetal: true, showShopDetail: false, navigator: navigator},
        activeBuilding: action.payload
    });
}
//商铺详情加载完成
reducerMap[actionTypes.GET_BUILDING_SHOPS_DETAIL_COMPLETE] = function (state, action) {
    let navigator = [...state.showResult.navigator];
    if (Object.keys(action.payload).length > 0) {
        navigator.push({id: action.payload, name: action.payload.basicInfo.buildingNo, type: 'shopDetail'});
    }
    let showResult = {showBuildingList: false, showBuildingDetal: false, showShopDetail: true, navigator: navigator};
    return Object.assign({}, state, {activeShop: action.payload, showResult: showResult});
}
//关闭详情
reducerMap[actionTypes.DETAIL_CLOSE] = function (state, action) {
    return Object.assign({}, state, {expandSearchBox: true, showResult: {showBuildingList: true, showBuildingDetal: false, showShopDetail: false, navigator: []}, activeBuilding: {}, activeBuildingShops: []});
}

export default handleActions(reducerMap, initState);