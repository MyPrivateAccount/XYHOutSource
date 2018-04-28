import { createAction } from 'redux-actions';
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
//审核操作
export const saveAudit = createAction(actionTypes.SAVE_AUDIT);//审核提交
export const getAuditHistory = createAction(actionTypes.GET_AUDIT_HISTORY);//获取审核历史
//页面切换
export const changeMenu = createAction(actionTypes.CHANGE_MENU);
//房源动态
export const getUpdateRecordDetail = createAction(actionTypes.GET_UPDATE_RECORD_DETAIL);//获取房源动态审核详细