import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

export const setLoading = createAction(actionTypes.SET_LOADING);

//流程
export const selectedFlow = createAction(actionTypes.ACTIVE_FLOW);
export const selectedFlowStep = createAction(actionTypes.ACTIVE_FLOW_STEP);
export const dragFlowStepAdd = createAction(actionTypes.DRAG_FLOW_STEP_ADD);
export const editFlowDefine = createAction(actionTypes.EDIT_FLOW_DEFINE);
export const deleteFlowDefine = createAction(actionTypes.DELETE_FLOW_SETP);
//流程
export const executeNwfCommand = createAction(actionTypes.EXECUTE_NWF_COMMAND);
export const getCommonParamList = createAction(actionTypes.GET_COMMON_PARAMLIST);
//export const getWorkFlowDefine = createAction(actionTypes.GET_WORKFLOW_DEFINE);
export const openTableConfig = createAction(actionTypes.OPEN_TABLE_CONFIG);
export const closeTableConfig = createAction(actionTypes.CLOSE_TABLE_CONFIG);
export const editTableConfig = createAction(actionTypes.EDIT_CONFIG_TABLE);
export const removeTableConfigItem = createAction(actionTypes.REMOVE_CONFIG_TABLE_ITEM);
export const saveTableConfigItem = createAction(actionTypes.SAVE_CONFIG_TABLE_ITEM);
export const changeStepCommonParam = createAction(actionTypes.CHANGE_STEP_COMMON_PARAM);
export const changeStepBasicParam = createAction(actionTypes.CHANGE_STEP_BASIC_PARAM);
export const updateFlowStep = createAction(actionTypes.EDIT_FLOW_STEP);

export const saveFlowDefine = createAction(actionTypes.SAVE_FLOW_DEFINE);
export const openImportFlow = createAction(actionTypes.OPEN_IMPORT_FLOW);
export const saveImportFlow = createAction(actionTypes.SAVE_IMPORT_FLOW);