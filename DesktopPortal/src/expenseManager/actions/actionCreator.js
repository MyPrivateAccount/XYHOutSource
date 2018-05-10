import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

export const getDepartment = createAction(actionTypes.GET_ALLDEPARTMENT);
export const changeMenu = createAction(actionTypes.CHNAGE_MENU);
