import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import { create } from 'domain';

export const getHumanList = createAction(actionTypes.GET_ALLHUMANINFO);

//基础字典数据
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const getOrgList = createAction(actionTypes.DIC_GET_ORG_LIST);
export const getOrgDetail = createAction(actionTypes.DIC_GET_ORG_DETAIL);
export const getUserByOrg = createAction(actionTypes.GET_ORG_USERLIST);
//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);

//单位选择处理
export const openOrgSelect = createAction(actionTypes.OPEN_ORG_SELECT);
export const closeOrgSelect = createAction(actionTypes.CLOSE_ORG_SELECT);
export const changeActiveOrg = createAction(actionTypes.CHAGNE_ACTIVE_ORG);

//搜索处理
export const searchKeyWord = createAction(actionTypes.SEARCH_KEYWORD);
export const searchCustomer = createAction(actionTypes.SEARCH_CUSTOMER);
export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);
export const expandSearchbox = createAction(actionTypes.SEARCH_BOX_EXPAND);

//面包屑
export const setbreadPage = createAction(actionTypes.SET_USER_BREAD);
export const closebreadPage = createAction(actionTypes.CLOSE_USER_BREAD);