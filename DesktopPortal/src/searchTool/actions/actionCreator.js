import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

//字典相关
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const setLoading = createAction(actionTypes.SET_SEARCH_LOADING);
//查询
export const searchBuilding = createAction(actionTypes.SEARCH_XYH_BUILDING);
export const getXYHBuildingDetail = createAction(actionTypes.XYH_GET_BUILDING_DETAIL);
export const getXYHBuildingPrev = createAction(actionTypes.RESULT_PREV);
export const getXYHBuildingNext = createAction(actionTypes.RESULT_NEXT);
export const closeResultDetail = createAction(actionTypes.RESULT_CLOSE);
export const expandSearchbox = createAction(actionTypes.SEARCH_BOX_EXPAND);
export const getBuildingShops = createAction(actionTypes.GET_BUILDING_SHOPS);//获取楼盘商铺列表
export const getShopDetail = createAction(actionTypes.GET_BUILDING_SHOPS_DETAIL);//获取商铺详细
export const recommendBuilding = createAction(actionTypes.RESULT_BUILDING_RECOMMEND);//楼盘推荐
export const backToPrevView = createAction(actionTypes.RESULT_VIEW_BACK);//返回操作
export const getRecommendPermission = createAction(actionTypes.GET_RECOMMEND_PERMISSION);//获取推荐权限
//推荐
export const openRecommendDialog = createAction(actionTypes.OPEN_RECOMMEND_DIALOG);
export const closeRecommendDialog = createAction(actionTypes.CLOSE_RECOMMEND_DIALOG);
export const searchRecommendList = createAction(actionTypes.GET_RECOMMEND_LIST);//获取推荐列表
export const cancelRecommend = createAction(actionTypes.CANCEL_RECOMMEND);//取消推荐
//成交信息
export const getCustomerDeal = createAction(actionTypes.GET_CUSTOMER_DEAL_INFO);
//导航
export const changeNav = createAction(actionTypes.NAV_CHANGE);
//消息
export const openMsgList = createAction(actionTypes.OPEN_MSG_LIST);
export const openMsgDetail = createAction(actionTypes.OPEN_MSG_DETAIL);
export const getMsgList = createAction(actionTypes.GET_MSG_LIST);
export const getMsgDetail = createAction(actionTypes.GET_MSG_DETAIL);
// 获取房源动态列表
export const getActiveList = createAction(actionTypes.GET_ACTIVE_LIST);
export const getActiveListStart = createAction(actionTypes.GET_ACTIVE_LIST_START);
export const getActiveListEnd = createAction(actionTypes.GET_ACTIVE_LIST_END);
