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