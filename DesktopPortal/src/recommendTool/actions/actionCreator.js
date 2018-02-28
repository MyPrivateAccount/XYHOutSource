import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

//字典相关
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const setLoading = createAction(actionTypes.SET_SEARCH_LOADING);
//查询
export const searchBuilding = createAction(actionTypes.SEARCH_XYH_BUILDING);
export const getMyRecommendBuilding=createAction(actionTypes.GET_MYCOMMEND_BUILDING);
export const recommendBuilding = createAction(actionTypes.RESULT_BUILDING_RECOMMEND);//楼盘推荐
export const getBuildingDetail = createAction(actionTypes.GET_BUILDING_DETAIL);
export const getBuildingShops = createAction(actionTypes.GET_BUILDING_SHOPS);//获取楼盘商铺列表
export const getShopDetail = createAction(actionTypes.GET_BUILDING_SHOPS_DETAIL);//获取商铺详细
export const closeResultDetail = createAction(actionTypes.DETAIL_CLOSE);//关闭详细
//推荐
export const openRecommendDialog = createAction(actionTypes.OPEN_RECOMMEND_DIALOG);
export const closeRecommendDialog = createAction(actionTypes.CLOSE_RECOMMEND_DIALOG);
export const searchRecommendList = createAction(actionTypes.GET_RECOMMEND_LIST);//获取推荐列表
export const cancelRecommend = createAction(actionTypes.CANCEL_RECOMMEND);//取消推荐
//导航
export const changeNav = createAction(actionTypes.NAV_CHANGE);
