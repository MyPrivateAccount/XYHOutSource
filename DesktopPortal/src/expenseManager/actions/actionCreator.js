import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

export const getDepartment = createAction(actionTypes.GET_ALLDEPARTMENT);
export const changeMenu = createAction(actionTypes.CHNAGE_MENU);

//字典操作
export const getDicInfo = createAction(actionTypes.GET_CHARGEDICINFO);

//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);

//面包屑
export const closebreadPage = createAction(actionTypes.CLOSE_USER_BREAD);
export const setuserPage = createAction(actionTypes.SET_USER_BREAD);
export const setuserPageIndex = createAction(actionTypes.SET_USER_BREADINDEX);
export const adduserPage = createAction(actionTypes.SET_USER_BREADADD);

export const uploadFile = createAction(actionTypes.UPLOAD_CHARGEFILE);


//服务接口
export const postChargeInfo = createAction(actionTypes.POST_CHARGEINFO);

export const postSearchCondition = createAction(actionTypes.POST_SEARCHCONDITION);
export const updateSearchStatu = createAction(actionTypes.UPDATE_SEARCHCHECKSTATU);
export const updateChargePrice = createAction(actionTypes.UPDATE_SEARCHCHARGEPRICE);

export const getRecieptByID = createAction(actionTypes.GET_RECIEPTBYID);

export const paymentCharge = createAction(actionTypes.POST_PAYMENTCHARGE);


export const postReciept = createAction(actionTypes.POST_RECIEPTINFO);

export const selCharge = createAction(actionTypes.SELECTCHARGE);
export const clearCharge = createAction(actionTypes.CLEARCHARGE);