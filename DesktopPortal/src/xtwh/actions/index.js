import {createAction} from 'redux-actions';
import * as actionTypes from '../constants';

export const getGroupListAsync = createAction(actionTypes.DIC_GET_GROUPLIST_ASYNC)
export const getGroupListStart = createAction(actionTypes.DIC_GET_GROUPLIST_START)
export const getGroupListFinish = createAction(actionTypes.DIC_GET_GROUPLIST_FINISH)
export const setCurrentGroup = createAction(actionTypes.DIC_SET_CURRENT_GROUP)

export const addDicGroup = createAction(actionTypes.DIC_ADD_GROUP)
export const editDicGroup = createAction(actionTypes.DIC_EDIT_GROUP)
export const cancelEditGroup = createAction(actionTypes.DIC_CANCEL_EDIT)

export const saveDicGroupAsync = createAction(actionTypes.DIC_SAVE_GROUP_ASYNC)
export const saveDicGroupStart = createAction(actionTypes.DIC_SAVE_GROUP_START)
export const saveDicGroupFinish = createAction(actionTypes.DIC_SAVE_GROUP_FINISH)

export const delDicGroupAsync = createAction(actionTypes.DIC_DEL_GROUP_ASYNC)
export const delDicGroupStart = createAction(actionTypes.DIC_DEL_GROUP_START)
export const delDicGroupFinish = createAction(actionTypes.DIC_DEL_GROUP_FINISH)

export const addDicValue = createAction(actionTypes.DIC_ADD_VALUE)
export const editDicValue = createAction(actionTypes.DIC_EDIT_VALUE)
export const cancelEditValue = createAction(actionTypes.DIC_CANCEL_EDIT_VALUE)

export const getParListAsync = createAction(actionTypes.DIC_GET_PARLIST_ASYNC)
export const getParListStart = createAction(actionTypes.DIC_GET_PARLIST_START)
export const getParListFinish = createAction(actionTypes.DIC_GET_PARLIST_FINISH)


export const saveDicValueAsync = createAction(actionTypes.DIC_SAVE_VALUE_ASYNC)
export const saveDicValueStart = createAction(actionTypes.DIC_SAVE_VALUE_START)
export const saveDicValueFinish = createAction(actionTypes.DIC_SAVE_VALUE_FINISH)

export const delDicValueAsync = createAction(actionTypes.DIC_DEL_VALUE_ASYNC)
export const delDicValueStart = createAction(actionTypes.DIC_DEL_VALUE_START)
export const delDicValueFinish = createAction(actionTypes.DIC_DEL_VALUE_FINISH)

// 启用字典项定义
export const startDicValueAsync = createAction(actionTypes.START_DIC_VALUE_ASYNC)
export const startDicValueStart = createAction(actionTypes.START_DIC_VALUE_START)
export const startDicValueEnd = createAction(actionTypes.START_DIC_VALUE_END)




//======== 区域 =========
export const getAreaListAsync = createAction(actionTypes.AREA_GET_LIST_ASYNC)
export const getAreaListStart = createAction(actionTypes.AREA_GET_LIST_START)
export const getAreaListFinish = createAction(actionTypes.AREA_GET_LIST_FINISH)
export const setCurrentArea = createAction(actionTypes.AREA_SET_CURRENT)
export const delAreaAsync = createAction(actionTypes.AREA_DEL_ASYNC)
export const delAreaStart = createAction(actionTypes.AREA_DEL_START)
export const delAreaFinish = createAction(actionTypes.AREA_DEL_FINISH)
export const addArea = createAction(actionTypes.AREA_ADD)
export const editArea = createAction(actionTypes.AREA_EDIT)
export const viewArea = createAction(actionTypes.AREA_VIEW)
export const cancelArea = createAction(actionTypes.AREA_CANCEL)
export const saveAreaAsync = createAction(actionTypes.AREA_SAVE_ASYNC)
export const saveAreaStart = createAction(actionTypes.AREA_SAVE_START)
export const saveAreaFinish = createAction(actionTypes.AREA_SAVE_FINISH)

//业态规划相关
export const addTradePlanning = createAction(actionTypes.TRADE_PLANNING_ADD);
export const addTradePlanningEnd = createAction(actionTypes.TRADE_PLANNING_ADD_END);
export const getTradePlanningList = createAction(actionTypes.TRADE_PLANNING_GETLIST);
export const getTradePlanningListEnd = createAction(actionTypes.TRADE_PLANNING_GETLIST_END);
export const editTradePlanning = createAction(actionTypes.TRADE_PLANNING_EDIT);
export const editTradePlanningEnd = createAction(actionTypes.TRADE_PLANNING_EDIT_END);
export const delTradePlanning = createAction(actionTypes.TRADE_PLANNING_REMOVE);
export const delTradePlanningEnd = createAction(actionTypes.TRADE_PLANNING_REMOVE_END);