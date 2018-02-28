export const ACTION_ROUTE = "SearchToolIndex";
export const SET_LOADING = 'SET_SEARCH_LOADING';//新耀行楼盘查询
//流程
export const ACTIVE_FLOW = 'ACTIVE_FLOW';//选中流程
export const ACTIVE_FLOW_STEP = 'ACTIVE_FLOW_STEP';//选中流程步骤
export const DRAG_FLOW_STEP_ADD = 'DRAG_FLOW_STEP_ADD';//添加拖放到设计器的步骤节点
export const EDIT_FLOW_DEFINE = 'EDIT_FLOW_DEFINE';//编辑流程定义
export const SAVE_FLOW_DEFINE = 'SAVE_FLOW_DEFINE';//保存流程配置
export const OPEN_IMPORT_FLOW = 'OPEN_IMPORT_FLOW';//导入流程
export const SAVE_IMPORT_FLOW = 'SAVE_IMPORT_FLOW';//保存导入流程

//流程步骤相关
export const EDIT_FLOW_STEP = 'EDIT_FLOW_STEP';//更新流程步骤
export const DELETE_FLOW_SETP = 'DELETE_FLOW_SETP';//删除步骤
export const CHANGE_STEP_COMMON_PARAM = 'CHANGE_STEP_COMMON_PARAM';//更改流程步骤通用配置
export const CHANGE_STEP_BASIC_PARAM = 'CHANGE_STEP_BASIC_PARAM';//更改流程步骤基础配置

//流程命令
export const EXECUTE_NWF_COMMAND = 'EXECUTE_NWF_COMMAND';//执行nwf命令
export const EXECUTE_NWF_COMMAND_COMPLETE = 'EXECUTE_NWF_COMMAND_COMPLETE';
export const GET_COMMON_PARAMLIST = 'GET_COMMON_PARAMLIST';//获取通用参数配置
export const GET_COMMON_PARAMLIST_COMPLETE = 'GET_COMMON_PARAMLIST_COMPLETE';//获取通用参数配置

//流程参数
export const EDIT_CONFIG_TABLE = 'EDIT_CONFIG_TABLE';//配置弹出窗编辑
export const EDIT_CONFIG_TABLE_COMPLETE = 'EDIT_CONFIG_TABLE_COMPLETE';//配置弹出窗编辑完成
export const SAVE_CONFIG_TABLE_ITEM = 'SAVE_CONFIG_TABLE_ITEM';//保存配置项
export const REMOVE_CONFIG_TABLE_ITEM = 'REMOVE_CONFIG_TABLE_ITEM';//移除配置表中的配置项
export const OPEN_TABLE_CONFIG = 'OPEN_TABLE_CONFIG';//打开配置表弹出窗
export const CLOSE_TABLE_CONFIG = 'CLOSE_TABLE_CONFIG';//关闭配置表弹出窗