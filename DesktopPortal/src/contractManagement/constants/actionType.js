export const ACTION_ROUTE = "ContractManagementIndex";

//字典项获取
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
//遮罩层
export const SET_SEARCH_LOADING = 'SET_SEARCH_LOADING';//获取待审核列表

//页面切换
export const CHANGE_MENU = 'CHANGE_MENU';//菜单切换


//搜索处理
export const SAVE_SEARCH_CONDITION = 'SAVE_SEARCH_CONDITION';//保存查询条件
export const CHANGE_KEYWORD = 'CHANGE_KEYWORD';
export const SEARCH_START = 'SEARCH_START';
export const SEARCH_COMPLETE = 'SEARCH_COMPLETE';


//部门选择
export const OPEN_ORG_SELECT = 'OPEN_ORG_SELECT';//打开部门选择
export const CLOSE_ORG_SELECT = 'CLOSE_ORG_SELECT';//关闭
export const CHAGNE_ACTIVE_ORG = 'CHAGNE_ACTIVE_ORG';//切换部门

export const GOTO_THIS_CONTRACT ='GOTO_THIS_CONTRACT';
export const GOTO_THIS_CONTRACT_START ='GOTO_THIS_CONTRACT_START';
export const GOTO_THIS_CONTRACT_FINISH ='GOTO_THIS_CONTRACT_FINISH';
export const GOTO_CHANGE_MYADD ='GOTO_CHANGE_MYADD';




//客户详情处理
export const OPEN_CUSTOMER_DETAIL = 'OPEN_CUSTOMER_DETAIL';//打开审核详细
export const CLOSE_CUSTOMER_DETAIL = 'CLOSE_CUSTOMER_DETAIL';//关闭审核详细
export const OPEN_ADJUST_CUSTOMER = 'OPEN_ADJUST_CUSTOMER';//打开调客对话框
export const CLOSE_ADJUST_CUSTOMER = 'CLOSE_ADJUST_CUSTOMER';//关闭调客对话框
export const GET_CUSTOMER_DETAIL = 'GET_CUSTOMER_DETAIL';//加载客户详情
export const GET_CUSTOMER_DETAIL_COMPLETE = 'GET_CUSTOMER_DETAIL_COMPLETE';//加载客户详情完成
export const ADJUST_CUSTOMER = 'ADJUST_CUSTOMER';//调客
export const GET_CUSTOMER_ALL_PHONE = 'GET_CUSTOMER_ALL_PHONE';//获取所有电话号码
export const GET_CUSTOMER_ALL_PHONE_COMPLETE = 'GET_CUSTOMER_ALL_PHONE_COMPLETE';//获取所有电话号码
export const GET_REPEAT_JUDGE_INFO = 'GET_REPEAT_JUDGE_INFO';//客户去重信息
export const GET_REPEAT_JUDGE_INFO_COMPLETE = 'GET_REPEAT_JUDGE_INFO_COMPLETE';//客户去重信息获取完成



//调客审核列表
export const GET_AUDIT_LIST = 'GET_AUDIT_LIST';//获取调客审核列表
export const GET_AUDIT_LIST_COMPLETE = 'GET_AUDIT_LIST_COMPLETE';//获取调客审核完成
export const GET_CUSTOMER_OF_USERID = 'GET_CUSTOMER_BY_USERID';//根据用户获取客户列表
export const GET_CUSTOMER_OF_USERID_COMPLETE = 'GET_CUSTOMER_BY_USERID';//获取完成
export const CHANGE_SOURCE_ORG = 'CHANGE_SOURCE_ORG';//切换调客单位
export const CHANGE_TARGET_ORG = 'CHANGE_TARGET_ORG';//切换接收单位
export const OPEN_CUSTOMER_AUDIT_INFO = 'OPEN_CUSTOMER_AUDIT_INFO';//打开调客审核详细
export const GET_AUDIT_HISTORY = 'GET_AUDIT_HISTORY';//获取当前记录的审核历史
export const GET_AUDIT_HISTORY_COMPLETE = 'GET_AUDIT_HISTORY_COMPLETE';//获取当前记录的审核历史
export const REMOVE_ADJUST_REQUEST_ITEM = 'REMOVE_ADJUST_REQUEST_ITEM';//移除单个调客请求中的客户

//合同文件上传
export const OPEN_ATTACHMENT = 'OPEN_ATTACHMENT'; //打开合同上传页
export const CLOSE_ATTACHMENT = 'CLOSE_ATTACHMENT';//关闭合同上传页
export const UPLOAD_ATTCHMENT_LIST = 'UPLOAD_ATTCHMENT_LIST';//上传附件
export const UPLOAD_ATTCHMENT_LIST_COMPLETE = 'UPLOAD_ATTCHMENT_LIST_COMPLETE';//上传附件完成

//合同录入
export const OPEN_RECORD = 'OPEN_RECORD'; //打开合同录入页
export const CLOSE_RECORD = 'CLOSE_RECORD';//关闭合同录入页
export const CONTRACT_INFO_SUBMIT = 'CONTRACT_INFO_SUBMIT';//提交合同信息
export const CONTRACT_INFO_SUBMIT_START = 'CONTRACT_INFO_SUBMIT_START';//开始提交合同信息
export const CONTRACT_INFO_SUBMIT_FINISH = 'SUBMIT_CONTRACT_INFO_COMPLETE';//提交合同信息完成
export const CLEAR_CONTRACT_INFO = 'CLEAR_CONTRACT_INFO';//清除合同信息

//导出
export const EXPORT_CONTRACT = 'EXPORT_CONTRACT';
export const EXPORT_CONTRACT_COMPLETE = 'EXPORT_CONTRACT_COMPLETE';
export const EXPORT_MULTI_CONTRACT = 'EXPORT_MULTI_CONTRACT';
export const EXPORT_MULTI_CONTRACT_COMPLETE = 'EXPORT_MULTI_CONTRACT_COMPLETE';


//合同相关
export const CONTRACT_GET_DETAIL = 'CONTRACT_GET_DETAIL';//合同整体信息

export const CONTRACT_BASIC_SAVE = 'CONTRACT_BASIC_SAVE';//合同基础信息保存
export const CONTRACT_BASIC_ADD = 'CONTRACT_BASIC_ADD';//合同基础信息添加
export const CONTRACT_BASIC_EDIT = 'CONTRACT_BASIC_EDIT';//合同基础信息编辑
export const CONTRACT_BASIC_VIEW = 'CONTRACT_BASIC_VIEW';

export const CONTRACT_PICTURE_SAVE = 'CONTRACT_PICTURE_SAVE';//附件图片保存
export const CONTRACT_PICTURE_EDIT = 'CONTRACT_PICTURE_EDIT';
export const CONTRACT_PICTURE_VIEW = 'CONTRACT_PICTURE_VIEW';

export const CONTRACT_FILE_SAVE = 'CONTRACT_PICTURE_SAVE';//附件文件保存
export const CONTRACT_FILE_EDIT = 'CONTRACT_PICTURE_EDIT';
export const CONTRACT_FILE_VIEW = 'CONTRACT_PICTURE_VIEW';

export const LOADING_START_BASIC = 'LOADING_START_BASIC';
export const LOADING_END_BASIC = 'LOADING_END_BASIC';

