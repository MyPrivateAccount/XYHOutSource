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
export const searchCondition = createAction(actionTypes.SEARCH_CONDITION);
export const searchCustomer = createAction(actionTypes.SEARCH_CUSTOMER);
export const searchHumanType = createAction(actionTypes.SEARCH_HUMANTYPE);
export const searchAgeType = createAction(actionTypes.SEARCH_AGETYPE);
export const searchOrderType = createAction(actionTypes.SEARCH_ORDERTYPE);
export const searchConditionType = createAction(actionTypes.SEARCH_CONDITION);

export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);
export const expandSearchbox = createAction(actionTypes.SEARCH_BOX_EXPAND);

//面包屑
export const setbreadPageIndex = createAction(actionTypes.SET_USER_BREADINDEX);
export const setbreadPageItem = createAction(actionTypes.SET_USER_BREADITEM);
export const closebreadPage = createAction(actionTypes.CLOSE_USER_BREAD);
export const adduserPage = createAction(actionTypes.ADD_USER_BREAD);

//
export const getworkNumbar = createAction(actionTypes.GET_HUMANINFONUMBER);

export const postHumanInfo = createAction(actionTypes.POST_HUMANINFO);

export const getallOrgTree = createAction(actionTypes.DIC_GET_ORG_LIST);

//月结
export const getAllMonthList = createAction(actionTypes.MONTH_GETALLMONTHLIST);
export const recoverMonth = createAction(actionTypes.MONTH_RECOVER);
export const createMonth = createAction(actionTypes.MONTH_CREATE);
export const monthLast = createAction(actionTypes.MONTH_LAST);

//黑名单
export const postBlackLst = createAction(actionTypes.POST_ADDBLACKLST);
export const selBlackList = createAction(actionTypes.SEL_BLACKLIST);

//职位新建
export const createStation = createAction(actionTypes.POST_CRATESTATION);