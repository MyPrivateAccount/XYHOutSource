//所有的action在这里创建
import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

//基础字典数据
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);

//部门组织树
export const orgGetPermissionTree = createAction(actionTypes.ORG_GET_PERMISSION_TREE);
export const getEmpList = createAction(actionTypes.EMP_GET_LIST);
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
export const orgParamDel = createAction(actionTypes.ORG_PARAM_DEL)
//提成比例设置页面action
export const incomeScaleAdd = createAction(actionTypes.INCOME_SCALE_ADD);
export const incomeScaleEdit = createAction(actionTypes.INCOME_SCALE_EDIT);
export const incomeScaleSave = createAction(actionTypes.INCOME_SCALE_SAVE);
export const incomeScaleDel = createAction(actionTypes.INCOME_SCALE_DEL);
export const incomeScaleListGet = createAction(actionTypes.INCOME_SCALE_LIST_GET);
export const incomeScaleDlgClose = createAction(actionTypes.INCOME_SCALE_DLGCLOSE);
//业绩分摊项设置页面action
export const acmentParamAdd = createAction(actionTypes.ACMENT_PARAM_ADD);
export const acmentParamEdit = createAction(actionTypes.ACMENT_PARAM_EDIT);
export const acmentParamSave = createAction(actionTypes.ACMENT_PARAM_SAVE);
export const acmentParamDel = createAction(actionTypes.ACMENT_PARAM_DEL);
export const acmentParamListGet = createAction(actionTypes.ACMENT_PARAM_LIST_GET);
export const acmentParamDlgClose = createAction(actionTypes.ACMENT_PARAM_DLGCLOSE);
export const acmentParamItemSave = createAction(actionTypes.ACMENT_PARAM_ITEM_SAVE);
export const acmentParamItemAdd = createAction(actionTypes.ACMENT_PARAM_ITEM_ADD);
//交易合同页面action
export const dealRpSave = createAction(actionTypes.DEALRP_RP_SAVE);
export const dealRpGet  = createAction(actionTypes.DEALRP_RP_GET);
export const getTradeReg = createAction(actionTypes.DEALRP_CJBB_GET);
//物业页面action
export const dealWySave = createAction(actionTypes.DEALRP_WY_SAVE);
export const dealWyGet = createAction(actionTypes.DEALRP_WY_GET);
//业主页面action
export const dealYzSave = createAction(actionTypes.DEALRP_YZ_SAVE);
export const dealYzGet = createAction(actionTypes.DEALRP_YZ_GET);
//客户页面action
export const dealKhSave = createAction(actionTypes.DEALRP_KH_SAVE);
export const dealKhGet = createAction(actionTypes.DEALRP_KH_GET);
//过户页面action
export const dealGhSave = createAction(actionTypes.DEALRP_GH_SAVE);
export const dealGhGet = createAction(actionTypes.DEALRP_GH_GET);
//业绩分配页面action
export const dealFpSave = createAction(actionTypes.DEALRP_FP_SAVE);
export const dealFpGet = createAction(actionTypes.DEALRP_FP_GET);
//附件上传文件
export const uploadFile = createAction(actionTypes.DEALRP_ATTACT_UPLOADFILE);
//
export const myReportGet = createAction(actionTypes.DEALRP_MYREPORT_GET);
export const searchReport = createAction(actionTypes.DEALRP_REPORT_SEARCH)
//查询人员分摊表
export const searchPPFt = createAction(actionTypes.FINA_QUERYPPFT);
//查询应发提成表
export const searchYftcb = createAction(actionTypes.FINA_QUERY_YFTCB);
//查询实发提成表
export const searchSftcb = createAction(actionTypes.FINA_QUERY_SFTCB);
//查询提成成本表
export const searchTccbb = createAction(actionTypes.FINA_QUERY_TCCBB);
//查询应发提成冲减表
export const searchYftccjb = createAction(actionTypes.FINA_QUERY_YFTCCJB);
//查询离职人员业绩确认表
export const searchLzryyjqrb = createAction(actionTypes.FINA_QUERY_LZRYYJQRB);
//查询实发扣减确认表
export const searchSfkjqrb = createAction(actionTypes.FINA_QUERY_SFKJQRB);
//查询分佣详情表
export const searchfyxqReport= createAction(actionTypes.FINA_QUERY_FYXQB);
//查询业绩调整汇总
export const searchYjtzhz = createAction(actionTypes.FINA_QUERY_YJTZHZ);
//查询调佣详情表
export const searchTyxq = createAction(actionTypes.FINA_QUERY_TYXQ);
//收付
export const factGet = createAction(actionTypes.DEALRP_FACTGET);
//收款
export const factGetGet = createAction(actionTypes.DEALRP_FACTGET_GET_SAVE);
//付款
export const factGetPay = createAction(actionTypes.DEALRP_FACTGET_PAY_SAVE);
//获取商铺详情
export const getShopDetail = createAction(actionTypes.DEALRP_SHOP_GET);
//获取楼盘详情
export const getBuildingDetail = createAction(actionTypes.DEALRP_BUILDING_GET);
//同步日期
export const syncYJDate = createAction(actionTypes.DEALRP_SYNC_DATE);
//同步报告基础信息
export const syncRp = createAction(actionTypes.DEALRP_SYNC_RP);
export const syncWy = createAction(actionTypes.DEALRP_SYNC_WY);
export const syncYz = createAction(actionTypes.DEALRP_SYNC_YZ);
export const syncKh = createAction(actionTypes.DEALRP_SYNC_KH);
export const syncFp = createAction(actionTypes.DEALRP_SYNC_FP);
//打开详情页面
export const openRpDetail = createAction(actionTypes.DEALRP_OPEN_RP_DETAIL)
//清空页面输入
export const rpClear = createAction(actionTypes.DEALRP_RP_CLEAR)
//搜索员工
export const searchHuman = createAction(actionTypes.SEARCH_HUMAN_INFO)
//删除报告
export const dealRpDelete = createAction(actionTypes.DEALRP_RP_DELETE)