import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';


export const increaseCount = createAction(actionTypes.INCREASE_COUNT);
export const decreaseCount = createAction(actionTypes.DECREASE_COUNT);

export const openNewWindow = createAction(actionTypes.WINDOWS_OPEN_NEW);
export const closeWindow = createAction(actionTypes.WINDOWS_CLOSE);
export const restoreWindow = createAction(actionTypes.WINDOWS_RESTORE);
export const activeWindow = createAction(actionTypes.WINDOWS_ACTIVE);
export const maxmizeWindow = createAction(actionTypes.WINDOWS_MAXIMIZE);
export const minimizeWindow = createAction(actionTypes.WINDOWS_MINIMIZE);
export const resotreXWindow = createAction(actionTypes.WINDOWS_RESTORE_X);
export const openOrActiveWindow = createAction(actionTypes.WINDOWS_OPENORACTIVE);
//工具列表获取
export const appListGet = createAction(actionTypes.APP_LIST_GET);

// 修改密码
export const resetPassword = createAction(actionTypes.PASSWORD_RESET);
export const colse = createAction(actionTypes.CLOSE_PASSWORD);

//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);

//系统消息
export const getUnReadCount = createAction(actionTypes.GET_UNREAD_COUNT);
export const getRecieveList = createAction(actionTypes.GET_RECEIVE_LIST);
export const getMsgDetail = createAction(actionTypes.GET_SYS_MSG_DETAIL);
//权限判断
export const judgePermission = createAction(actionTypes.JUDGE_PERMISSION);

//字典相关
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);