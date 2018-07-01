//action路由
export const ACTION_ROUTE = 'CommissionManagerIndex';
//所有的action type在这里定义
//人数分摊组织设置
export const ORG_GET_PERMISSION_TREE = 'ORG_GET_PERMISSION_TREE';//获取有权限的部门树
export const ORG_GET_PERMISSION_TREE_UPDATE = 'ORG_GET_PERMISSION_TREE_UPDATE';
export const EMP_GET_LIST = 'EMP_GET_LIST';
export const EMP_LIST_UPDATE = 'EMP_LIST_UPDATE';

export const ORG_FT_PARAM_ADD = 'ORG_FT_PARAM_ADD';//人数组织分摊参数添加
export const ORG_FT_PARAM_EDIT = 'ORG_FT_PARAM_EDIT';//人数组织分摊参数修改
export const ORG_FT_PARAM_SAVE = 'ORG_FT_PARAM_SAVE';//人数组织分摊参数保存
export const ORG_FT_PARAM_DELETE = 'ORG_FT_PARAM_DELETE';//人数组织分摊参数删除
export const ORG_FT_PARAM_DELETE_UPDATE = 'ORG_FT_PARAM_DELETE_UPDATE'
export const ORG_FT_DIALOG_CLOSE = 'ORG_FT_DIALOG_CLOSE';//窗口关闭
export const ORG_FT_PARAMLIST_GET = 'ORG_FT_PARAMLIST_GET';//获取数据列表
export const ORG_FT_PARAMLIST_UPDATE = 'ORG_FT_PARAMLIST_UPDATE';//更新数据列表
export const ORG_FT_PARAM_SAVE_SUCCESS = 'ORG_FT_PARAM_SAVE_SUCCESS';
//组织参数设置
export const ORG_PARAM_ADD = 'ORG_PARAM_ADD';//添加组织参数
export const ORG_PARAM_EDIT = 'ORG_PARAM_EDIT';//修改组织参数
export const ORG_PARAM_DIALOG_CLOSE = 'ORG_PARAM_DIALOG_CLOSE';//对话框关闭
export const ORG_PARAMLIST_GET = 'ORG_PARAMLIST_GET';//组织参数列表数据获取
export const ORG_PARAMLIST_UPDATE = 'ORG_PARAMLIST_UPDATE';//组织参数列表数据更新
export const ORG_PARAM_SAVE = 'ORG_PARAM_SAVE';//组织参数保存
export const ORG_PARAM_SAVE_UPDATE = 'ORG_PARAM_SAVE_UPDATE';
export const ORG_PARAM_DEL = 'ORG_PARAM_DEL'
export const ORG_PARAM_DEL_UPDATE = 'ORG_PARAM_DEL_UPDATE'
//提成比例设置
export const INCOME_SCALE_ADD = 'INCOME_SCALE_ADD';//新增提成比例设置
export const INCOME_SCALE_EDIT = 'INCOME_SCALE_EDIT';//提成比例修改
export const INCOME_SCALE_SAVE = 'INCOME_SCALE_SAVE';//提成比例保存
export const INCOME_SCALE_DEL = 'INCOME_SCALE_DEL';//提成比例删除
export const INCOME_SCALE_DEL_UPDATE = 'INCOME_SCALE_DEL_UPDATE';//删除通知
export const INCOME_SCALE_DLGCLOSE = 'INCOME_SCALE_DLGCLOSE';//取消关闭对话框
export const INCOME_SCALE_LIST_GET = 'INCOME_SCALE_LIST_GET';//获取提成比例数据列表
export const INCOME_SCALE_LIST_UPDATE = 'INCOME_SCALE_LIST_UPDATE';//更新提成比例数据列表
export const INCOME_SCALE_SAVE_SUCCESS = 'INCOME_SCALE_SAVE_SUCCESS';
//业绩分摊设置
export const ACMENT_PARAM_ADD = 'ACMENT_PARAM_ADD';
export const ACMENT_PARAM_EDIT = 'ACMENT_PARAM_EDIT';
export const ACMENT_PARAM_SAVE = 'ACMENT_PARAM_SAVE';
export const ACMENT_PARAM_DEL='ACMENT_PARAM_DEL';
export const ACMENT_PARAM_DEL_UPDATE = 'ACMENT_PARAM_DEL_UPDATE'
export const ACMENT_PARAM_DLGCLOSE = 'ACMENT_PARAM_DLGCLOSE';
export const ACMENT_PARAM_LIST_GET = 'ACMENT_PARAM_LIST_GET';
export const ACMENT_PARAM_LIST_UPDATE = 'ACMENT_PARAM_LIST_UPDATE';
export const ACMENT_PARAM_ITEM_GET = 'ACMENT_PARAM_ITEM_GET';
export const ACMENT_PARAM_ITEM_GET_UPDATE = 'ACMENT_PARAM_ITEM_GET_UPDATE'
export const ACMENT_PARAM_ITEM_SAVE = 'ACMENT_PARAM_ITEM_SAVE';
export const ACMENT_PARAM_ITEM_SAVEUPDATE = 'ACMENT_PARAM_ITEM_SAVEUPDATE';
export const ACMENT_PARAM_ITEM_ADD = 'ACMENT_PARAM_ITEM_ADD';
export const ACMENT_PARAM_GET = 'ACMENT_PARAM_GET';
export const ACMENT_PARAM_UPDATE = 'ACMENT_PARAM_UPDATE';

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
export const DEALRP_RP_GET = 'DEALRP_RP_GET';//异步获取报告数据
export const DEALRP_RP_GETUPDATE = 'DEALRP_RP_GETUPDATE';//获取报告数据更新
export const DEALRP_CJBB_GET = 'DEALRP_CJBB_GET';//获取成交报备
export const DEALRP_CJBB_LISTUPDATE = 'DEALRP_CJBB_LISTUPDATE'//更新成功
//成交报告-物业页面
export const DEALRP_WY_SAVE = 'DEALRP_WY_SAVE';
export const DEALRP_WY_SAVEUPDATE = 'DEALRP_WY_SAVEUPDATE';
export const DEALRP_WY_GET = 'DEALRP_WY_GET';
export const DEALRP_WY_GETUPDATE = 'DEALRP_WY_GETUPDATE';
//成交报告-业主页面
export const DEALRP_YZ_SAVE = 'DEALRP_YZ_SAVE';
export const DEALRP_YZ_SAVEUPDATE = 'DEALRP_YZ_SAVEUPDATE';
export const DEALRP_YZ_GET = 'DEALRP_YZ_GET';
export const DEALRP_YZ_GETUPDATE = 'DEALRP_YZ_GETUPDATE';
//成交报告-客户页面
export const DEALRP_KH_SAVE = 'DEALRP_KH_SAVE';
export const DEALRP_KH_SAVEUPDATE = 'DEALRP_KH_SAVEUPDATE';
export const DEALRP_KH_GET = 'DEALRP_KH_GET';
export const DEALRP_KH_GETUPDATE = 'DEALRP_KH_GETUPDATE';
//成交报告-过户页面
export const DEALRP_GH_SAVE = 'DEALRP_GH_SAVE';
export const DEALRP_GH_SAVEUPDATE = 'DEALRP_GH_SAVEUPDATE';
export const DEALRP_GH_GET = 'DEALRP_GH_GET';
export const DEALRP_GH_GETUPDATE = 'DEALRP_GH_GETUPDATE';
//成交报告-业绩分配页面
export const DEALRP_FP_SAVE = 'DEALRP_FP_SAVE';
export const DEALRP_FP_SAVEUPDATE = 'DEALRP_FP_SAVEUPDATE';
export const DEALRP_FP_GET = 'DEALRP_FP_GET';
export const DEALRP_FP_GETUPDATE = 'DEALRP_FP_GETUPDATE';
//成交报告-附件
export const DEALRP_ATTACT_UPLOADFILE = 'DEALRP_ATTACT_UPLOADFILE';
export const DEALRP_ATTACT_UPLOAD_COMPLETE = 'DEALRP_ATTACT_UPLOAD_COMPLETE'
//
export const DEALRP_MYREPORT_GETUPDATE = 'DEALRP_MYREPORT_GETUPDATE';
export const DEALRP_MYREPORT_GET = 'DEALRP_MYREPORT_GET';
export const DEALRP_REPORT_SEARCH = 'DEALRP_REPORT_SEARCH';
export const DEALRP_REPORT_SEARCH_UPDATE = 'DEALRP_REPORT_SEARCH_UPDATE';
//人员分摊表
export const FINA_QUERYPPFT = 'FINA_QUERYPPFT'
export const FINA_QUERYPPFT_SUCCESS = 'FINA_QUERYPPFT_SUCCESS'
//查询应发提成表
export const FINA_QUERY_YFTCB = 'FINA_QUERY_YFTCB'
export const FINA_QUERY_YFTCB_SUCCESS = 'FINA_QUERY_YFTCB_SUCCESS'
//查询实发提成表
export const FINA_QUERY_SFTCB = 'FINA_QUERY_SFTCB'
export const FINA_QUERY_SFTCB_SUCCESS = 'FINA_QUERY_SFTCB_SUCCESS'
//查询提成成本表
export const FINA_QUERY_TCCBB = 'FINA_QUERY_TCCBB'
export const FINA_QUERY_TCCBB_SUCCESS = 'FINA_QUERY_TCCBB_SUCCESS'
//应发提成冲减表
export const FINA_QUERY_YFTCCJB = 'FINA_QUERY_YFTCCJB'
export const FINA_QUERY_YFTCCJB_SUCCESS = 'FINA_QUERY_YFTCCJB_SUCCESS'
//离职人员业绩确认表
export const FINA_QUERY_LZRYYJQRB = 'FINA_QUERY_LZRYYJQRB'
export const FINA_QUERY_LZRYYJQRB_SUCCESS = 'FINA_QUERY_LZRYYJQRB_SUCCESS'
//实发扣减确认表
export const FINA_QUERY_SFKJQRB = 'FINA_QUERY_SFKJQRB'
export const FINA_QUERY_SFKJQRB_SUCCESS = 'FINA_QUERY_SFKJQRB_SUCCESS'
//分佣详情表
export const FINA_QUERY_FYXQB = 'FINA_QUERY_FYXQB'
export const FINA_QUERY_FYXQB_SUCCESS = 'FINA_QUERY_FYXQB_SUCCESS'
//业绩调整汇总
export const FINA_QUERY_YJTZHZ = 'FINA_QUERY_YJTZHZ'
export const FINA_QUERY_YJTZHZ_SUCCESS = 'FINA_QUERY_YJTZHZ_SUCCESS'
//调佣详情表
export const FINA_QUERY_TYXQ = 'FINA_QUERY_TYXQ'
export const FINA_QUERY_TYXQ_SUCCESS = 'FINA_QUERY_TYXQ_SUCCESS'
//获取收付信息
export const DEALRP_FACTGET = 'DEALRP_FACTGET'
export const DEALRP_FACTGET_SUCCESS = 'DEALRP_FACTGET_SUCCESS'
//保存收款信息
export const DEALRP_FACTGET_GET_SAVE = 'DEALRP_FACTGET_GET_SAVE'
export const DEALRP_FACTGET_GET_SAVE_SUCCESS = 'DEALRP_FACTGET_GET_SAVE_SUCCESS'
//保存付款信息
export const DEALRP_FACTGET_PAY_SAVE = 'DEALRP_FACTGET_PAY_SAVE'
export const DEALRP_FACTGET_PAY_SAVE_SUCCESS = 'DEALRP_FACTGET_PAY_SAVE_SUCCESS'
//获取商铺详情
export const DEALRP_SHOP_GET = 'DEALRP_SHOP_GET'
export const DEALRP_SHOP_GET_SUCCESS = 'DEALRP_SHOP_GET_SUCCESS'
//获取楼盘详情
export const DEALRP_BUILDING_GET = 'DEALRP_BUILDING_GET'
export const DEALRP_BUILDING_GET_SUCCESS = 'DEALRP_BUILDING_GET_SUCCESS'
//同步日期
export const DEALRP_SYNC_DATE = 'DEALRP_SYNC_DATE'
//同步成交报告基础信息
export const DEALRP_SYNC_RP = 'DEALRP_SYNC_RP'
export const DEALRP_SYNC_WY = 'DEALRP_SYNC_WY'
export const DEALRP_SYNC_YZ = 'DEALRP_SYNC_YZ'
export const DEALRP_SYNC_KH = 'DEALRP_SYNC_KH'
export const DEALRP_SYNC_FP = 'DEALRP_SYNC_FP'
//打开成交报告详情页
export const DEALRP_OPEN_RP_DETAIL = 'DEALRP_OPEN_RP_DETAIL'
//清空界面输入
export const DEALRP_RP_CLEAR = 'DEALRP_RP_CLEAR'
//搜索分公司下面的员工信息
export const SEARCH_HUMAN_INFO = 'SEARCH_HUMAN_INFO'
export const SEARCH_HUMAN_INFO_SUCCESS = 'SEARCH_HUMAN_INFO_SUCCESS'
//删除成交报告
export const DEALRP_RP_DELETE = 'DEALRP_RP_DELETE'
export const DEALRP_RP_DELETE_SUCCESS = 'DEALRP_RP_DELETE_SUCCESS'
//月结页面相关
export const YJ_MONTH_GET = 'YJ_MONTH_GET'//获取月结月份
export const YJ_MONTH_GETUPDATE = 'YJ_MONTH_GETUPDATE'//获取月结月份返回
export const YJ_MONTH_START = 'YJ_MONTH_START'//开始月结
export const YJ_MONTH_START_UPDATE = 'YJ_MONTH_START_UPDATE'//开始月结返回
export const YJ_MONTH_CHECK = 'YJ_MONTH_CHECK'//月结进度检查接口
export const YJ_MONTH_CHECK_UPDATE = 'YJ_MONTH_CHECK_UPDATE'//月结进度检查接口返回
export const YJ_MONTH_CANCEL = 'YJ_MONTH_CANCEL'//取消月结
export const YJ_MONTH_CANCEL_UPDATE = 'YJ_MONTH_CANCEL_UPDATE'//取消月结返回
export const YJ_MONTH_YJQR_QUERY = 'YJ_MONTH_YJQR_QUERY'//业绩确认查询
export const YJ_MONTH_YJQR_QUERY_UPDATE = 'YJ_MONTH_YJQR_QUERY_UPDATE'//业绩确认查询返回
export const YJ_MONTH_YJQR_COMMIT='YJ_MONTH_YJQR_COMMIT'//业绩确认提交请求
export const YJ_MONTH_YJQR_COMMIT_UPDATE='YJ_MONTH_YJQR_COMMIT_UPDATE'//业绩确认提交请求返回
export const YJ_MONTH_SKQR_QUERY = 'YJ_MONTH_SKQR_QUERY'//实扣查询
export const YJ_MONTH_SKQR_QUERY_UPDATE = 'YJ_MONTH_SKQR_QUERY_UPDATE'//实扣查询返回
export const YJ_MONTH_SKQR_COMMIT = 'YJ_MONTH_SKQR_COMMIT'//实扣查询提交
export const YJ_MONTH_SKQR_COMMIT_UPDATE = 'YJ_MONTH_SKQR_COMMIT_UPDATE'//实扣查询提交返回
export const YJ_MONTH_ROLLBACK = 'YJ_MONTH_ROLLBACK'//月结回滚
export const YJ_MONTH_ROLLBACK_UPDATE = 'YJ_MONTH_ROLLBACK_UPDATE'//月结回滚返回
///////////////////////////////////////////////////////////
export const FINA_QUERY_YJQR_EMP = 'FINA_QUERY_YJQR_EMP'
export const YJ_MONTH_EMP = 'YJ_MONTH_EMP'
///////////////////////////////////////////////////////////
export const FINA_QUERY_SKQR_EMP = 'FINA_QUERY_SKQR_EMP'
export const YJ_MONTH_SKQR_EMP = 'YJ_MONTH_SKQR_EMP'