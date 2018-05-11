import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

export const getDepartment = createAction(actionTypes.GET_ALLDEPARTMENT);
export const changeMenu = createAction(actionTypes.CHNAGE_MENU);
//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);
