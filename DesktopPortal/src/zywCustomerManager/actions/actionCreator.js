import {createAction} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
//基础字典数据
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const getOrgList = createAction(actionTypes.DIC_GET_ORG_LIST);
export const getOrgDetail = createAction(actionTypes.DIC_GET_ORG_DETAIL);
export const getUserByOrg = createAction(actionTypes.GET_ORG_USERLIST);
//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);
//客户详情处理
export const openCustomerDetail = createAction(actionTypes.OPEN_CUSTOMER_DETAIL);
export const closeCustomerDetail = createAction(actionTypes.CLOSE_CUSTOMER_DETAIL);
export const changeCustomerMenu = createAction(actionTypes.CHANGE_MENU);
export const getCustomerDetail = createAction(actionTypes.GET_CUSTOMER_DETAIL);
export const getCustomerAllPhone = createAction(actionTypes.GET_CUSTOMER_ALL_PHONE);
//单位选择处理
export const openOrgSelect = createAction(actionTypes.OPEN_ORG_SELECT);
export const closeOrgSelect = createAction(actionTypes.CLOSE_ORG_SELECT);
export const changeActiveOrg = createAction(actionTypes.CHAGNE_ACTIVE_ORG);
//调客处理
export const openAdjustCustomer = createAction(actionTypes.OPEN_ADJUST_CUSTOMER);
export const closeAdjustCustomer = createAction(actionTypes.CLOSE_ADJUST_CUSTOMER);
export const adjustCustomer = createAction(actionTypes.ADJUST_CUSTOMER);
export const getAuditList = createAction(actionTypes.GET_AUDIT_LIST);
export const getCustomerByUserID = createAction(actionTypes.GET_CUSTOMER_OF_USERID);
export const changeSourceOrg = createAction(actionTypes.CHANGE_SOURCE_ORG);
export const changeTargetOrg = createAction(actionTypes.CHANGE_TARGET_ORG);
export const openCustomerAuditDetail = createAction(actionTypes.OPEN_CUSTOMER_AUDIT_INFO);
export const getAuditHistory = createAction(actionTypes.GET_AUDIT_HISTORY);//获取审核历史
//搜索处理
export const changeKeyWord = createAction(actionTypes.CHANGE_KEYWORD);
export const searchCustomer = createAction(actionTypes.SEARCH_CUSTOMER);
export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);
export const getRepeatJudgeInfo = createAction(actionTypes.GET_REPEAT_JUDGE_INFO);
export const removeAdjustItem = createAction(actionTypes.REMOVE_ADJUST_REQUEST_ITEM);
export const getRepeatGroup = createAction(actionTypes.GET_REAPEAT_GROUP);
export const getCustomerByGroup = createAction(actionTypes.GET_CUSTOMER_BY_GROUP);
export const getRepeatCount = createAction(actionTypes.GET_REPEAT_COUNT);
// 拉无效
export const showCustomerlossModal = createAction(actionTypes.SHOW_CUSTOMER_LOSS_MODAL);
export const hideCustomerlossModal = createAction(actionTypes.HIDE_CUSTOMER_LOSS_MODAL);

export const customerloss = createAction(actionTypes.CUSTOMER_LOSS);
export const customerlossStart = createAction(actionTypes.CUSTOMER_LOSS_START);
export const customerlossEnd = createAction(actionTypes.CUSTOMER_LOSS_END);
// 激活
export const customerlossActive = createAction(actionTypes.CUSTOMER_LOSS_ACTIVE);
export const customerlossActiveStart = createAction(actionTypes.CUSTOMER_LOSS_ACTIVE_START);
export const customerlossActiveEnd = createAction(actionTypes.CUSTOMER_LOSS_ACTIVE_END);

// 意向业态
export const getTreeListAsync = createAction(actionTypes.GET_TREE_LIST);
export const getTreeListComplete = createAction(actionTypes.GET_TREE_LIST_COMPLETE);
