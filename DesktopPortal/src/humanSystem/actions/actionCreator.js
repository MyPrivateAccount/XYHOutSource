import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

export const getHumanList = createAction(actionTypes.GET_ALLHUMANINFO);

//基础字典数据
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
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

//搜索处理
export const searchKeyWord = createAction(actionTypes.SEARCH_KEYWORD);
export const searchCustomer = createAction(actionTypes.SEARCH_CUSTOMER);
export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);
export const getRepeatJudgeInfo = createAction(actionTypes.GET_REPEAT_JUDGE_INFO);
export const expandSearchbox = createAction(actionTypes.SEARCH_BOX_EXPAND);