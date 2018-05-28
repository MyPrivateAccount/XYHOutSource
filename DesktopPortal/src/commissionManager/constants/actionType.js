//action路由
export const ACTION_ROUTE = 'CommissionManagerIndex';
//所有的action type在这里定义
//人数分摊组织设置
export const ORG_GET_PERMISSION_TREE = 'ORG_GET_PERMISSION_TREE';//获取有权限的部门树
export const ORG_GET_PERMISSION_TREE_UPDATE = 'ORG_GET_PERMISSION_TREE_UPDATE';
export const ORG_FT_PARAM_ADD = 'ORG_FT_PARAM_ADD';//人数组织分摊参数添加
export const ORG_FT_PARAM_EDIT = 'ORG_FT_PARAM_EDIT';//人数组织分摊参数修改
export const ORG_FT_PARAM_SAVE = 'ORG_FT_PARAM_SAVE';//人数组织分摊参数保存
export const ORG_FT_PARAM_DELETE = 'ORG_FT_PARAM_DELETE';//人数组织分摊参数删除
export const ORG_FT_DIALOG_CLOSE = 'ORG_FT_DIALOG_CLOSE';//窗口关闭
export const ORG_FT_PARAMLIST_GET = 'ORG_FT_PARAMLIST_GET';//获取数据列表
export const ORG_FT_PARAMLIST_UPDATE = 'ORG_FT_PARAMLIST_UPDATE';//更新数据列表
//组织参数设置
export const ORG_PARAM_ADD = 'ORG_PARAM_ADD';//添加组织参数
export const ORG_PARAM_EDIT = 'ORG_PARAM_EDIT';//修改组织参数
export const ORG_PARAM_DIALOG_CLOSE = 'ORG_PARAM_DIALOG_CLOSE';//对话框关闭
export const ORG_PARAMLIST_GET = 'ORG_PARAMLIST_GET';//组织参数列表数据获取
export const ORG_PARAMLIST_UPDATE = 'ORG_PARAMLIST_UPDATE';//组织参数列表数据更新
export const ORG_PARAM_SAVE = 'ORG_PARAM_SAVE';//组织参数保存
//提成比例设置
export const INCOME_SCALE_ADD = 'INCOME_SCALE_ADD';//新增提成比例设置
export const INCOME_SCALE_EDIT = 'INCOME_SCALE_EDIT';//提成比例修改
export const INCOME_SCALE_SAVE = 'INCOME_SCALE_SAVE';//提成比例保存
export const INCOME_SCALE_DEL = 'INCOME_SCALE_DEL';//提成比例删除
export const INCOME_SCALE_DLGCLOSE = 'INCOME_SCALE_DLGCLOSE';//取消关闭对话框
export const INCOME_SCALE_LIST_GET = 'INCOME_SCALE_LIST_GET';//获取提成比例数据列表
export const INCOME_SCALE_LIST_UPDATE = 'INCOME_SCALE_LIST_UPDATE';//更新提成比例数据列表
//业绩分摊设置
export const ACMENT_PARAM_ADD = 'ACMENT_PARAM_ADD';
export const ACMENT_PARAM_EDIT = 'ACMENT_PARAM_EDIT';
export const ACMENT_PARAM_SAVE = 'ACMENT_PARAM_SAVE';
export const ACMENT_PARAM_DEL='ACMENT_PARAM_DEL';
export const ACMENT_PARAM_DLGCLOSE = 'ACMENT_PARAM_DLGCLOSE';
export const ACMENT_PARAM_LIST_GET = 'ACMENT_PARAM_LIST_GET';
export const ACMENT_PARAM_LIST_UPDATE = 'ACMENT_PARAM_LIST_UPDATE';
export const ACMENT_PARAM_ITEM_SAVE = 'ACMENT_PARAM_ITEM_SAVE';
export const ACMENT_PARAM_ITEM_SAVEUPDATE = 'ACMENT_PARAM_ITEM_SAVEUPDATE';
export const ACMENT_PARAM_ITEM_ADD = 'ACMENT_PARAM_ITEM_ADD';

//遮罩层
export const SET_SEARCH_LOADING = 'SET_SEARCH_LOADING';
//字典相关
export const DIC_GET_PARLIST = 'DIC_GET_PARLIST';
export const DIC_GET_PARLIST_COMPLETE = 'DIC_GET_PARLIST_COMPLETE';
export const DIC_GET_AREA = 'DIC_GET_AREA';
export const DIC_GET_AREA_COMPLETE = 'DIC_GET_AREA_COMPLETE';
export const DIC_GET_ORG_LIST = 'DIC_GET_ORG_LIST';//获取部门数据
export const DIC_GET_ORG_LIST_COMPLETE = 'DIC_GET_ORG_LIST_COMPLETE';
export const DIC_GET_ORG_DETAIL = 'DIC_GET_ORG_DETAIL';//获取部门详细数据
export const DIC_GET_ORG_DETAIL_COMPLETE = 'DIC_GET_ORG_DETAIL_COMPLETE';
export const GET_ORG_USERLIST = 'GET_ORG_USERLIST';//获取部门用户列表
export const GET_ORG_USERLIST_COMPLETE = 'GET_ORG_USERLIST_COMPLETE';//获取部门用户列表
export const DIC_GET_ALL_ORG_LIST = 'DIC_GET_ALL_ORG_LIST';
export const DIC_GET_ALL_ORG_LIST_COMPLETE = 'DIC_GET_ALL_ORG_LIST_COMPLETE';

//成交报告－交易合同页面
export const DEALRP_RP_SAVE = 'DEALRP_RP_SAVE';//交易合同保存
export const DEALRP_RP_SAVEUPDATE = 'DEALRP_RP_SAVEUPDATE';//交易合同保存成功
//成交报告-物业页面
export const DEALRP_WY_SAVE = 'DEALRP_WY_SAVE';
export const DEALRP_WY_SAVEUPDATE = 'DEALRP_WY_SAVEUPDATE';
//成交报告-业主页面
export const DEALRP_YZ_SAVE = 'DEALRP_YZ_SAVE';
export const DEALRP_YZ_SAVEUPDATE = 'DEALRP_YZ_SAVEUPDATE';
//成交报告-客户页面
export const DEALRP_KH_SAVE = 'DEALRP_KH_SAVE';
export const DEALRP_KH_SAVEUPDATE = 'DEALRP_KH_SAVEUPDATE';
//成交报告-过户页面
export const DEALRP_GH_SAVE = 'DEALRP_GH_SAVE';
export const DEALRP_GH_SAVEUPDATE = 'DEALRP_GH_SAVEUPDATE';
//成交报告-业绩分配页面
export const DEALRP_FP_SAVE = 'DEALRP_FP_SAVE';
export const DEALRP_FP_SAVEUPDATE = 'DEALRP_FP_SAVEUPDATE';
