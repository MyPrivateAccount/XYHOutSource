import {createAction} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import {create} from 'domain';

export const getHumanList = createAction(actionTypes.GET_ALLHUMANINFO);

//基础字典数据
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const getOrgList = createAction(actionTypes.DIC_GET_ORG_LIST);
export const getOrgDetail = createAction(actionTypes.DIC_GET_ORG_DETAIL);
export const getUserByOrg = createAction(actionTypes.GET_ORG_USERLIST);
//设置遮罩层
//export const setLoadingVisible = createAction(actionTypes.SET_BASE_LOADING);
export const setSearchLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);

//单位选择处理
export const openOrgSelect = createAction(actionTypes.OPEN_ORG_SELECT);
export const closeOrgSelect = createAction(actionTypes.CLOSE_ORG_SELECT);
export const changeActiveOrg = createAction(actionTypes.CHAGNE_ACTIVE_ORG);

export const setVisibleHead = createAction(actionTypes.SET_VISIBLEHEAD);

//搜索处理
export const searchKeyWord = createAction(actionTypes.SEARCH_KEYWORD);
export const searchCondition = createAction(actionTypes.SEARCH_CONDITION);
export const searchCustomer = createAction(actionTypes.SEARCH_CUSTOMER);
// export const searchHumanType = createAction(actionTypes.SEARCH_HUMANTYPE);
// export const searchAgeType = createAction(actionTypes.SEARCH_AGETYPE);
// export const searchOrderType = createAction(actionTypes.SEARCH_ORDERTYPE);
export const searchConditionType = createAction(actionTypes.SEARCH_CONDITION);
export const searchIndex = createAction(actionTypes.SET_SEARCHINDEX);

export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);
// export const expandSearchbox = createAction(actionTypes.SEARCH_BOX_EXPAND);
export const getHumenDetail = createAction(actionTypes.GET_HUMEN_DETAIL);
export const getHumenDetailEnd = createAction(actionTypes.GET_HUMEN_DETAIL_END);
//面包屑
export const setbreadPageIndex = createAction(actionTypes.SET_USER_BREADINDEX);
export const setbreadPageItemIndex = createAction(actionTypes.SET_USER_BREADITEMINDEX);
export const setbreadPageItem = createAction(actionTypes.SET_USER_BREADITEM);
export const closebreadPage = createAction(actionTypes.CLOSE_USER_BREAD);
export const adduserPage = createAction(actionTypes.ADD_USER_BREAD);


//
export const getworkNumbar = createAction(actionTypes.GET_HUMANINFONUMBER);

export const postHumanInfo = createAction(actionTypes.POST_HUMANINFO);

export const getallOrgTree = createAction(actionTypes.DIC_GET_ORG_LIST);
export const setHumanInfo = createAction(actionTypes.SET_SELHUMANINFO);

export const getHumanImage = createAction(actionTypes.GET_HUMANIMAGE);

//转正
export const setSocialEN = createAction(actionTypes.POST_SOCIALINSURANCE);

//月结
export const getAllMonthList = createAction(actionTypes.MONTH_GETALLMONTHLIST);
export const recoverMonth = createAction(actionTypes.MONTH_RECOVER);
export const createMonth = createAction(actionTypes.MONTH_CREATE);
export const monthLast = createAction(actionTypes.MONTH_LAST);

//黑名单
export const getBlackList = createAction(actionTypes.GET_BLACKLST);
export const postBlackLst = createAction(actionTypes.POST_ADDBLACKLST);
export const selBlackList = createAction(actionTypes.SEL_BLACKLIST);
export const deleteBlackInfo = createAction(actionTypes.DELETE_BLACKINFO);

//职位新建
export const getcreateStation = createAction(actionTypes.GET_CRATESTATION);
export const setStation = createAction(actionTypes.SET_STATION);
export const deleteStation = createAction(actionTypes.DELETE_STATION);
export const getStationType = createAction(actionTypes.GET_STATIONTYPELIST);
export const getcreateOrgStation = createAction(actionTypes.GET_CRATEORGSTATION);

//薪酬管理
export const getSalaryItem = createAction(actionTypes.GET_SALARYITEM);
export const getSalaryList = createAction(actionTypes.GET_SALARYLIST);
export const setSalaryInfo = createAction(actionTypes.SET_SALARYINFO);
//export const updateSalaryInfo = createAction(actionTypes.UPDATE_SALARYINFO);
export const setSelSalaryList = createAction(actionTypes.SET_SELSALARYLIST);
export const deleteSalaryInfo = createAction(actionTypes.DELETE_SALARYINFO);

//离职
export const leavePosition = createAction(actionTypes.LEAVE_POSITON);

//异动
export const postChangeHuman = createAction(actionTypes.POST_CHANGEHUMAN);

//导表
export const exportMonthForm = createAction(actionTypes.EXPORT_MONTHFORM);
export const exportHumanForm = createAction(actionTypes.EXPORT_HUMANFORM);

//组织架构
export const deleteOrgbyId = createAction(actionTypes.DELETE_ORGBYID);
export const deleteMemOrgbyId = createAction(actionTypes.UPDATE_DELETE_ORGBYID);
export const upaddOrg = createAction(actionTypes.UPDATE_ADD_ORG);

export const addOrg = createAction(actionTypes.ADD_ORG);
export const updateOrg = createAction(actionTypes.UPDATE_ORG);

//考勤
export const getAttendenceSettingList = createAction(actionTypes.GET_ATTENDANCESETTINGLST);
export const postSetAttendenceSettingList = createAction(actionTypes.POST_ATTENDANCESETTINGLST);

export const importAttendenceList = createAction(actionTypes.IMPORT_ATTENDANCELST);
export const searchAttendenceList = createAction(actionTypes.SEARCH_ATTENDANCELST);
export const deleteAttendenceItem = createAction(actionTypes.DELETE_ATTENDANCEITEM);
export const selAttendenceList = createAction(actionTypes.SEL_ATTENDANCELIST);

//行政奖惩
export const addRewardPunishment = createAction(actionTypes.ADD_REWARDPUNISHMENT);
export const searchRewardPunishment = createAction(actionTypes.SEARCH_REWARDPUNISHMENT);
export const deleteRewardPunishment = createAction(actionTypes.DELTE_REWARDPUNISHMENT);

export const gethumanlstbyorgid = createAction(actionTypes.GETSELHUMANLIST_BYORGID);