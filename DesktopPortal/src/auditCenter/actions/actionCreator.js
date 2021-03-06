import {createAction} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);
//查询
export const getAuditList = createAction(actionTypes.GET_AUDIT_LIST);
export const openAuditDetail = createAction(actionTypes.OPEN_AUDIT_DETAIL);
export const closeAuditDetail = createAction(actionTypes.CLOSE_AUDIT_DETAIL);
export const getAuditDetail = createAction(actionTypes.GET_AUDIT_DETAIL);
export const saveCondition = createAction(actionTypes.SAVE_CONDITION);//保存查询条件
export const getNoReadCount = createAction(actionTypes.GET_NO_READ_COUNT);//获取知会未读总数
export const getBuildingDetail = createAction(actionTypes.GET_BUILDING_DETAIL);//获取楼盘详细
export const getBuildingShops = createAction(actionTypes.GET_BUILDING_SHOPS);//获取楼盘下商铺列表
export const getShopDetail = createAction(actionTypes.GET_SHOP_DETAIL);//获取商铺详情
//审核操作
export const saveAudit = createAction(actionTypes.SAVE_AUDIT);//审核提交
export const getAuditHistory = createAction(actionTypes.GET_AUDIT_HISTORY);//获取审核历史
//页面切换
export const changeMenu = createAction(actionTypes.CHANGE_MENU);
//房源动态
export const getActiveDetail = createAction(actionTypes.GET_UPDATE_RECORD_DETAIL);//获取房源动态审核详细
//成交信息
export const getCustomerDealInfo = createAction(actionTypes.GET_CUSTOMER_DEALINFO);

//租壹屋
export const getZywBuildingDetail = createAction(actionTypes.GET_ZYW_BUILDING_DETAIL);//获取租壹屋楼盘详情
export const getZywCustomerDealInfo = createAction(actionTypes.GET_ZYW_CUSTOMER_DEALINFO);
export const getZywShopDetail = createAction(actionTypes.GET_ZYW_SHOP_DETAIL);
export const getZywActiveDetail = createAction(actionTypes.GET_ZYW_UPDATE_RECORD_DETAIL);//获取房源动态审核详细

//合同
export const getContractDetail = createAction(actionTypes.GET_CONTRACT_DETAIL);//获取合同分类信息