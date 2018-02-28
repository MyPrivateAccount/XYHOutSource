import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
//区域字典
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
//部门组织树
export const orgGetPermissionTree = createAction(actionTypes.ORG_GET_PERMISSION_TREE);
export const getOrgDataByID = createAction(actionTypes.ORG_DATA_GET);
export const orgNodeSave = createAction(actionTypes.ORG_DATA_SAVE);
export const orgNodeAdd = createAction(actionTypes.ORG_NODE_ADD);
export const orgNodeEdit = createAction(actionTypes.ORG_NODE_EDIT);
export const orgNodeDelete = createAction(actionTypes.ORG_DATA_DELETE);
export const orgDialogClose = createAction(actionTypes.ORG_DIALOG_CLOSE);
export const orgNodeSelected = createAction(actionTypes.ORG_NODE_SELECTED);
//员工
export const empAdd = createAction(actionTypes.EMP_ADD);
export const empEdit = createAction(actionTypes.EMP_EDIT);
export const empSave = createAction(actionTypes.EMP_SAVE);
export const empListGet = createAction(actionTypes.EMP_GET_LIST);
export const empListUpdate = createAction(actionTypes.EMP_LIST_UPDATE);
export const empDelete = createAction(actionTypes.EMP_DELETE);
export const empDialogClose = createAction(actionTypes.EMP_DIALOG_CLOSE);
export const empGetPrivList = createAction(actionTypes.EMP_GET_PRIV_LIST);
export const empRoleEdit = createAction(actionTypes.EMP_ROLE_EDIT);
export const empRoleSave = createAction(actionTypes.EMP_ROLE_SAVE);
export const empRestPwd = createAction(actionTypes.EMP_RESET_PWD);
//角色
export const roleAdd = createAction(actionTypes.ROLE_ADD);
export const roleEdit = createAction(actionTypes.ROLE_EDIT);
export const roleDialogClose = createAction(actionTypes.ROLE_DIALOG_CLOSE);
export const roleGetList = createAction(actionTypes.ROLE_GET_LIST);
export const roleAppSave = createAction(actionTypes.ROLE_APPLICATION_SAVE);
export const roleSlected = createAction(actionTypes.ROLE_SELECTED);
export const roleDelete = createAction(actionTypes.ROLE_DELETE);
export const roleSave = createAction(actionTypes.ROLE_SAVE);
export const rolePrivilegeGet = createAction(actionTypes.ROLE_PRIVILEGE_GET);
export const roleGetAllToolPrivilegeItem = createAction(actionTypes.ROLE_TOOL_PRIVILEGE_ITEM_GET);
export const rolePrivilegeSave = createAction(actionTypes.ROLE_PRIVILEGE_SAVE);
export const rolePrivilegeEdit = createAction(actionTypes.ROLE_PRIVILEGE_EDIT);
//应用
export const appAdd = createAction(actionTypes.APP_ADD);
export const appEdit = createAction(actionTypes.APP_EDIT);
export const appDataSave = createAction(actionTypes.APP_DATA_SAVE);
export const appDialogClose = createAction(actionTypes.APP_DIALOG_CLOSE);
export const appListGet = createAction(actionTypes.APP_LIST_GET);
export const appPrivilegeGet = createAction(actionTypes.APP_PRIVILEGE_GET);
export const appDelete = createAction(actionTypes.APP_DELETE);
//权限
export const privilegeAdd = createAction(actionTypes.PRIVILEGE_ADD);
export const privilegeEdit = createAction(actionTypes.PRIVILEGE_EDIT);
export const privilegeGetList = createAction(actionTypes.PRIVILEGE_GET_LIST);
export const privilegeDialogClose = createAction(actionTypes.PRIVILEGE_DIALOG_CLOSE);
export const privilegeSave = createAction(actionTypes.PRIVILEGE_SAVE);
export const privilegeDelete = createAction(actionTypes.PRIVILEGE_DELETE);
//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);
