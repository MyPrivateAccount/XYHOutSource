//所有的action在这里创建
import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
//部门组织树
export const orgGetPermissionTree = createAction(actionTypes.ORG_GET_PERMISSION_TREE);
//添加 
export const orgFtParamAdd = createAction(actionTypes.ORG_FT_PARAM_ADD);
//修改
export const orgFtParamUpdate = createAction(actionTypes.ORG_FT_PARAM_EDIT);
//保存
export const orgFtParamSave = createAction(actionTypes.ORG_FT_PARAM_SAVE);
//关闭
export const orgFtDialogClose = createAction(actionTypes.ORG_FT_DIALOG_CLOSE);
//获取数据列表
export const orgFtParamListGet = createAction(actionTypes.ORG_FT_PARAMLIST_GET);
//删除数据
export const orgFtParamDelete = createAction(actionTypes.ORG_FT_PARAM_DELETE);
//组织参数页面action
export const orgParamAdd = createAction(actionTypes.ORG_PARAM_ADD);
export const orgParamEdit = createAction(actionTypes.ORG_PARAM_EDIT);
export const orgParamSave = createAction(actionTypes.ORG_PARAM_SAVE);
export const orgParamListGet = createAction(actionTypes.ORG_PARAMLIST_GET);
export const orgParamDlgClose = createAction(actionTypes.ORG_PARAM_DIALOG_CLOSE);
//提成比例设置页面action
export const incomeScaleAdd = createAction(actionTypes.INCOME_SCALE_ADD);
export const incomeScaleEdit = createAction(actionTypes.INCOME_SCALE_EDIT);
export const incomeScaleSave = createAction(actionTypes.INCOME_SCALE_SAVE);
export const incomeScaleDel = createAction(actionTypes.INCOME_SCALE_DEL);
export const incomeScaleListGet = createAction(actionTypes.INCOME_SCALE_LIST_GET);
export const incomeScaleDlgClose = createAction(actionTypes.INCOME_SCALE_DLGCLOSE);