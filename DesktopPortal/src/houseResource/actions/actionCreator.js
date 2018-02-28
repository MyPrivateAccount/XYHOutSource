import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
// 列表的展示切换
export const changeShowGroup = createAction(actionTypes.CHANGE_SHOW_GROUP);
//字典相关
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
//楼盘录入
export const saveBuildingBasic = createAction(actionTypes.BUILDING_BASIC_SAVE);
export const saveSupportInfo = createAction(actionTypes.BUILDING_SUPPORT_SAVE);
export const saveRelShops = createAction(actionTypes.BUILDING_RELSHOP_SAVE);
export const saveProject = createAction(actionTypes.BUILDING_PROJECT_SAVE);

export const editBuildingBasic = createAction(actionTypes.BUILDING_BASIC_EDIT);
export const editSupportInfo = createAction(actionTypes.BUILDING_SUPPORT_EDIT);
export const editRelshopInfo = createAction(actionTypes.BUILDING_RELSHOP_EDIT);
export const editProjectInfo = createAction(actionTypes.BUILDING_PROJECT_EDIT);

export const viewBuildingBasic = createAction(actionTypes.BUILDING_BASIC_VIEW);
export const viewSupportInfo = createAction(actionTypes.BUILDING_SUPPORT_VIEW);
export const viewRelshopInfo = createAction(actionTypes.BUILDING_RELSHOP_VIEW);
export const viewProjectInfo = createAction(actionTypes.BUILDING_PROJECT_VIEW);

// 商铺录入

// 获取楼盘列表信息
export const getBuildingsListAsync = createAction(actionTypes.BUILDING_GET_LIST_ASYNC);
export const buildingsListStart = createAction(actionTypes.BUILDING_LIST_START);
export const buildingsListFinish = createAction(actionTypes.BUILDING_LIST_FINISH);
// 商铺录入基本信息
export const saveShopBasicAsync = createAction(actionTypes.SHOP_BASIC_SAVE_ASYNC);
export const saveShopBasicStart = createAction(actionTypes.SHOP_BASIC_SAVE_START);
export const saveShopBasicFinish = createAction(actionTypes.SHOP_BASIC_SAVE_FINISH);
// 商铺录入租约信息
export const saveShopLeaseAsync = createAction(actionTypes.SHOP_LEASE_SAVE_ASYNC);
export const saveShopLeaseStart = createAction(actionTypes.SHOP_LEASE_SAVE_START);
export const saveShopLeaseFinish = createAction(actionTypes.SHOP_LEASE_SAVE_FINISH);
// 商铺录入配套设施信息
export const saveShopSupportAsync = createAction(actionTypes.SHOP_SUPPORT_SAVE_ASYNC);
export const saveShopSupportStart = createAction(actionTypes.SHOP_SUPPORT_SAVE_START);
export const saveShopSupportFinish = createAction(actionTypes.SHOP_SUPPORT_SAVE_FINISH);
// 商铺编辑基本信息
export const editShopBasic = createAction(actionTypes.SHOP_BASIC_EDIT);
export const viewShopBasic = createAction(actionTypes.SHOP_BASIC_VIEW);
// 商铺编辑租约信息
export const editShopLease = createAction(actionTypes.SHOP_LEASE_EDIT);
export const viewShopLease = createAction(actionTypes.SHOP_LEASE_VIEW);
// 商铺编辑配套设施信息
export const editShopSupport = createAction(actionTypes.SHOP_SUPPORT_EDIT);
export const viewShopSupport = createAction(actionTypes.SHOP_SUPPORT_VIEW);

// 提交商铺信息
export const submitShopsInfo = createAction(actionTypes.SHOPS_INFO_SUBMIT);
export const submitShopsInfoFinish = createAction(actionTypes.SHOPS_INFO_SUBMIT_FINISH);
export const submitShopsInfoStart = createAction(actionTypes.SHOPS_INFO_SUBMIT_START);
// 获取商铺列表信息
export const getShopsListAsync = createAction(actionTypes.SHOPS_GET_LIST_ASYNC);
export const shopsListStart = createAction(actionTypes.SHOPS_LIST_START);
export const shopsListFinish = createAction(actionTypes.SHOPS_LIST_FINISH);

// 获取自己负责的商铺列表信息
export const getMyBuildingsListAsync = createAction(actionTypes.BUILDING_GET_MYLIST_ASYNC);
export const myBuildingListStart = createAction(actionTypes.BUILDING_MYLIST_START);
export const myBuildingListFinish = createAction(actionTypes.BUILDING_MYLIST_FINISH);

// 跳转页面
export const gotoBuildPage = createAction(actionTypes.GOTO_BUILD_PAGE);
export const gotoShopPage = createAction(actionTypes.GOTO_SHOP_PAGE);
export const gotoAddShop = createAction(actionTypes.GOTO_ADD_SHOP);
export const changeMyAdd = createAction(actionTypes.CHANGE_MYADD);
export const gotoThisShop = createAction(actionTypes.GOTO_THIS_SHOP);
export const gotoThisShopStart = createAction(actionTypes.GOTO_THIS_SHOP_START);
export const gotoThisShopFinish = createAction(actionTypes.GOTO_THIS_SHOP_FINISH);
export const gotoThisBuild = createAction(actionTypes.GOTO_THIS_BUILD);
export const gotoThisBuildStart = createAction(actionTypes.GOTO_THIS_BUILD_START);
export const gotoThisBuildFinish = createAction(actionTypes.GOTO_THIS_BUILD_FINISH);
export const gotoChangeMyAdd = createAction(actionTypes.GOTO_CHANGE_MYADD);

// 删除商铺
export const deleteShop = createAction(actionTypes.DELETE_SHOP);
export const deleteStart = createAction(actionTypes.DELETE_START);
export const deleteFinish = createAction(actionTypes.DELETE_FINISH);

// 提交楼盘信息
export const submitBuildInfo = createAction(actionTypes.BUILD_INFO_SUBMIT);
export const submitBuildInfoStart = createAction(actionTypes.BUILD_INFO_SUBMIT_START);
export const submitBuildInfoFinish = createAction(actionTypes.BUILD_INFO_SUBMIT_FINISH);

// 查看更多商铺
export const lookMore = createAction(actionTypes.LOOK_MORE);
// 搜索我的楼盘
export const mySearchFinish = createAction(actionTypes.MYSEARCH_FINISH);

// 动态添加排序
export const activeAdd = createAction(actionTypes.ADD_ACTIVE);
// 保存楼栋批次
export const saveBatchBuildingAsync = createAction(actionTypes.BATCH_BUILDING_SAVE_ASYNC)
export const saveBatchBuildingStart = createAction(actionTypes.BATCH_BUILDING_SAVE__START);
export const saveBatchBuildingFinish = createAction(actionTypes.BATCH_BUILDING_SAVE__FINISH);
export const saveMySelectedRows = createAction(actionTypes.SAVE_SELECTED_ROWS);




// 编辑楼栋批次
export const editBatchBuilding = createAction(actionTypes.BATCH_BUILDING_EDIT);
export const viewBatchBuilding = createAction(actionTypes.BATCH_BUILDING_VIEW);
export const addBatchBuilding = createAction(actionTypes.BATCH_BUILDING_ADD);

// 2017/11/23
export const comfirmCreateBuilding = createAction(actionTypes.COMFIRM_CREATE_BUDINGNO);
export const changeCell = createAction(actionTypes.CHANGE_CELL);

// 上传图片
export const savePictureAsync = createAction(actionTypes.SAVE_PICTURE_ASYNC);
export const uploadPicFinish = createAction(actionTypes.UPLOAD_PIC_FINISH);
export const shopPicView = createAction(actionTypes.SHOP_PIC_VIEW);
export const shopPicEdit = createAction(actionTypes.SHOP_PIC_EDIT);
export const buildingPicView = createAction(actionTypes.BUILDING_PIC_VIEW);
export const buildingPicEdit = createAction(actionTypes.BUILDING_PIC_EDIT);
export const deletePicAsync = createAction(actionTypes.DELETE_PICTURE_ASYNC);
export const saveCompleteFileList = createAction(actionTypes.SAVE_COMPLETE_FILE_LIST);
export const saveDeletePicList = createAction(actionTypes.SAVE_DELETE_PIC_LIST);

// 2017-11-28 驻场首页
export const getMyBuildingList = createAction(actionTypes.GET_MYBUILDING_LIST);
export const myListStart = createAction(actionTypes.MYLIST_START);
export const myListFinish = createAction(actionTypes.MYLIST_FINISH);
export const changeNowListInfo = createAction(actionTypes.CHANGE_NOWLIST_INFO);
export const searchMyBuildingList = createAction(actionTypes.SEARCH_MYBUILDING_LIST);
export const searchFinish = createAction(actionTypes.SEARCH_FINISH);
export const getMyCustomerInfo = createAction(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS);
export const customerTransactionsFinish = createAction(actionTypes.CUSTOMOR_TRANSACTIONS_FINISH);
export const getThisBuilding = createAction(actionTypes.GET_THIS_BUILDING);
export const getThisBuildStart = createAction(actionTypes.GET_THIS_BUILDING_START);
export const getThisBuildFinish = createAction(actionTypes.GET_THIS_BUILDING_FINISH);
export const comfirmAsync = createAction(actionTypes.COMFIRM_ASYNC);
export const comfirmFinish = createAction(actionTypes.COMFIRM_FINISH);
export const lookAsync = createAction(actionTypes.LOOK_ASYNC);
export const lookFinish = createAction(actionTypes.LOOK_FINISH);
export const reportAsync = createAction(actionTypes.REPORT_ASYNC);
export const reportFinish = createAction(actionTypes.REPORT_FINISH);
export const statusCount = createAction(actionTypes.STATUS_COUNT);
export const countFinish = createAction(actionTypes.COUNT_FINISH);
export const getReportCustomerDeal = createAction(actionTypes.GET_REPORT_CUSTOMER_DEAL);
export const getReportCustomerDealStart = createAction(actionTypes.GET_REPORT_CUSTOMER_DEAL_START);
export const getReportCustomerDealFinish = createAction(actionTypes.GET_REPORT_CUSTOMER_DEAL_FINISH);

// 2017-12-7 录楼盘报备规则佣金方案
export const saveRulesTemplate = createAction(actionTypes.RULES_TEMPLATE_SAVE);
export const rulesTemplateView = createAction(actionTypes.RULES_TEMPLATE_VIEW);
export const rulesTemplateEdit = createAction(actionTypes.RULES_TEMPLATE_EDIT);
export const saveRulesInfo = createAction(actionTypes.RULES_SAVE);
export const rulesView = createAction(actionTypes.RULES_VIEW);
export const rulesEdit = createAction(actionTypes.RULES_EDIT);
export const saveCommissionInfo = createAction(actionTypes.COMMISSION_SAVE);
export const commissionView = createAction(actionTypes.COMMISSION_VIEW);
export const commissionEdit = createAction(actionTypes.COMMISSION_EDIT);
export const saveTemplateRows = createAction(actionTypes.SAVE_TEMPLATE_ROWS);



// 2017-12-8 成交信息
export const showEditModal = createAction(actionTypes.SHOW_EDIT_MODAL);
export const hideEditModal = createAction(actionTypes.HIDE_EDIT_MODAL);
export const customerDealAsync = createAction(actionTypes.CUSTOMER_DEAL_ASYNC);
export const customerDealStart = createAction(actionTypes.CUSTOMER_DEAL_ASYNC_START);
export const customerDealFinish = createAction(actionTypes.CUSTOMER_DEAL_ASYNC_FINISH);

// 2017-12-9 销控中心
export const showTimeDialog = createAction(actionTypes.SHOW_TIME_DIALOG_MODAL);
export const cancelTimeDialog = createAction(actionTypes.CANCEL_TIME_DIALOG_MODAL);
export const saveTimeAsync = createAction(actionTypes.SAVE_TIME_ASYNC);
export const saveTimeStart = createAction(actionTypes.SAVE_TIME_START);
export const saveTimeFinish = createAction(actionTypes.SAVE_TIME_FINISH);
export const getShopsSaleStatus = createAction(actionTypes.GET_SHOPS_SALE_STATUS);
export const getShopsSaleStatusStart = createAction(actionTypes.GET_SHOPS_SALE_STATUS_START);
export const getShopsSaleStatusFinish = createAction(actionTypes.GET_SHOPS_SALE_STATUS_FINISH);
export const getSalestatistics = createAction(actionTypes.GET_SALES_TATISTICS);
export const getSalestatisticsStart = createAction(actionTypes.GET_SALES_TATISTICS_START);
export const getSalestatisticsFinish = createAction(actionTypes.GET_SALES_TATISTICS_FINISH);
export const getMakeDealCustomerInfo = createAction(actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO);
export const getMakeDealCustomerInfoStart= createAction(actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO_START);
export const getMakeDealCustomerInfoEnd = createAction(actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO_END);
export const getCustomerDeal = createAction(actionTypes.GET_CUSTOMER_DEAL);
export const getCustomerDealStart = createAction(actionTypes.GET_CUSTOMER_DEAL_START);
export const getCustomerDealFinish = createAction(actionTypes.GET_CUSTOMER_DEAL_FINISH);

// 2017-12-11 驻场首页，批量向开发商报备
export const showBatchReportModal = createAction(actionTypes.SHOW_BATCH_REPORT_MODAL);
export const hideBatchReportModal = createAction(actionTypes.HIDE_BATCH_REPORT_MODAL);
export const showTemplateModal = createAction(actionTypes.SHOW_TEMPLATE_MODAL);
export const hideTemplateModal = createAction(actionTypes.HIDE_TEMPLATE_MODAL);
export const changeChecked = createAction(actionTypes.CHANGE_CHECKED);
export const SearchValphone = createAction(actionTypes.SEARCH_VALPHONE);
export const SearchValphoneStart = createAction(actionTypes.SEARCH_VALPHONE_START);
export const SearchValphoneEnd = createAction(actionTypes.SEARCH_VALPHONE_END);


// 2017-12-13 房源审核
export const getHouseAuditBuildingListAsync = createAction(actionTypes.GET_HOUSE_AUDIT_BUILDING_LIST_ASYNC);
export const getHouseAuditBuildingListStart = createAction(actionTypes.GET_HOUSE_AUDIT_BUILDING_LIST_START);
export const getHouseAuditBuildingListFinish = createAction(actionTypes.GET_HOUSE_AUDIT_BUILDING_LIST_FINISH);
export const getHouseAuditShopsListAsync = createAction(actionTypes.GET_HOUSE_AUDIT_SHOPS_LIST_ASYNC);
export const getHouseAuditShopsListStart = createAction(actionTypes.GET_HOUSE_AUDIT_SHOPS_LIST_START);
export const getHouseAuditShopsListFinish = createAction(actionTypes.GET_HOUSE_AUDIT_SHOPS_LIST_FINISH);

// 2017-12-19 驻场管理
export const editZcManager = createAction(actionTypes.EDIT_ZC_MANAGER);
export const closeModal = createAction(actionTypes.CLOSE_MODAL);
export const openModal = createAction(actionTypes.OPEN_MODAL);
export const savePerson = createAction(actionTypes.SAVE_PERSON);
export const savePersonStart = createAction(actionTypes.SAVE_PERSON_START);
export const savePersonFinish = createAction(actionTypes.SAVE_PERSON_FINISH);
export const getBuildingsite = createAction(actionTypes.GET_BUILDING_SITE);
export const getBuildingsiteStart = createAction(actionTypes.GET_BUILDING_SITE_START);
export const getBuildingsiteEnd = createAction(actionTypes.GET_BUILDING_SITE_END);
export const getsiteuserlist = createAction(actionTypes.GET_SITEUSER_LIST);
export const getsiteuserlistStart = createAction(actionTypes.GET_SITEUSER_LIST_START);
export const getsiteuserlistEnd = createAction(actionTypes.GET_SITEUSER_LIST_END);
export const changeArr = createAction(actionTypes.CHANGE_ARR);

// 2017-12-20 房源动态
export const gotoShopDetailPage = createAction(actionTypes.GOTO_SHOP_DETAIL_PAGE);
export const gotoShopDetailPageStart = createAction(actionTypes.GOTO_SHOP_DETAIL_PAGE_START);
export const gotoShopDetailPageFinish = createAction(actionTypes.GOTO_SHOP_DETAIL_PAGE_FINISH);
export const gotoProjectDetailPage = createAction(actionTypes.GOTO_PROJECT_DETAIL_PAGE);
export const gotoProjectDetailPageStart = createAction(actionTypes.GOTO_PROJECT_DETAIL_PAGEE_START);
export const gotoProjectDetailPageFinish = createAction(actionTypes.GOTO_PROJECT_DETAIL_PAGE_FINISH);
export const getId = createAction(actionTypes.GET_ID);
export const getShops = createAction(actionTypes.GET_SHOPS);
export const getShopsStart = createAction(actionTypes.GET_SHOPS_START);
export const getShopsFinish = createAction(actionTypes.GET_SHOPS_FINISH);
export const setDescription = createAction(actionTypes.DESCRIPTION_SET_VALUE);
export const clearDescription = createAction(actionTypes.DESCRIPTION_CLEAR_VALUE);
export const submitDynamic = createAction(actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES);
export const submitDynamicStart = createAction(actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES_START);
export const submitDynamicEnd = createAction(actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES_END);
export const postCheckedArr = createAction(actionTypes.POST_CHECKED_ARR);
export const getExaminesStatus = createAction(actionTypes.GET_EXAMINES_STATUS);
export const getExaminesStatusStart = createAction(actionTypes.GET_EXAMINES_STATUS_START);
export const getExaminesStatusEnd = createAction(actionTypes.GET_EXAMINES_STATUS_END);
export const priceEdit = createAction(actionTypes.PRICE_EDIT);
export const priceView = createAction(actionTypes.PRICE_VIEW);
export const youhuiEdit = createAction(actionTypes.YOU_HUI_EDIT);
export const youhuiView = createAction(actionTypes.YOU_HUI_VIEW);
export const reportIsUse =  createAction(actionTypes.IS_USE_REPORT);
export const getDynamicInfoList = createAction(actionTypes.GET_DYNAMIC_INFO_LIST);
export const getDynamicInfoListEnd = createAction(actionTypes.GET_DYNAMIC_INFO_LIST_END);
export const getDynamicInfoDetail = createAction(actionTypes.GET_DYNAMIC_INFO_DETAIL);
export const getShopDynamicInfoDetailEnd = createAction(actionTypes.GET_SHOP_DYNAMIC_INFO_DETAIL_END);
export const getProjectDynamicInfoDetailEnd = createAction(actionTypes.GET_PROJECT_DYNAMIC_INFO_DETAIL_END);
export const getDynamicStatusEnd = createAction(actionTypes.GET_DYNAMIC_STATUS_END);
export const getDynamicStatusStart = createAction(actionTypes.GET_DYNAMIC_STATUS_START);
export const setTitle = createAction(actionTypes.SET_TITLE);
export const clearTitle = createAction(actionTypes.TITLE_CLEAR_VALUE);

export const getShopList = createAction(actionTypes.DYNAMIC_SET_SHOPS_VALUE);
export const getShopsEnd = createAction(actionTypes.DYNAMIC_SET_SHOPS_VALUE_END);
export const getShopsClear = createAction(actionTypes.DYNAMIC_SET_SHOPS_VALUE_CLEAR);

export const getBuilding = createAction(actionTypes.DYNAMIC_SET_BUILDING_VALUE)
export const getBuildingStart = createAction(actionTypes.DYNAMIC_SET_BUILDING_VALUE_START)
export const getBuildingEnd = createAction(actionTypes.DYNAMIC_SET_BUILDING_VALUE_END)

//房源消息
export const sendBuildingMsg = createAction(actionTypes.SEND_BUILDING_MSG);
export const setMsgLoading = createAction(actionTypes.SET_MSG_LOADING);

// 新增楼盘列表部分 2017-12-28更改list请求方式和list样式
export const getAddBuilding = createAction(actionTypes.GET_ADD_BUILDING)
export const getAddBuildingStart = createAction(actionTypes.GET_ADD_BUILDING_START)
export const getAddBuildingEnd = createAction(actionTypes.GET_ADD_BUILDING_END)

export const geAddtShopList = createAction(actionTypes.GET_ADD_SHOP_LIST)
export const geAddtShopStart = createAction(actionTypes.GET_ADD_SHOP_LIST_START)
export const getAddShopsEnd = createAction(actionTypes.GET_ADD_SHOP_LIST_END)

// 删除楼盘
export const deleteBuilding = createAction(actionTypes.DELETE_BUILDING);
export const deleteBuildingStart = createAction(actionTypes.DELETE_BUILDING_START);
export const deleteBuildingFinish = createAction(actionTypes.DELETE_BUILDING_FINISH);

// 切换楼盘列表 
export const getChangeBuildingList = createAction(actionTypes.GET_CHANGE_BUILDING_LIST);
export const getChangeBuildingListStart = createAction(actionTypes.GET_CHANGE_BUILDING_LIST_START);
export const getChangeBuildingListEnd = createAction(actionTypes.GET_CHANGE_BUILDING_LIST_END);
export const getThisProjectIndex = createAction(actionTypes.GET_THIS_PROJECT_INDEX);

export const editShopSummaryInfo = createAction(actionTypes.EDIT_SHOP_SUMMARY_INFO);
export const viewShopSummaryInfo = createAction(actionTypes.VIEW_SHOP_SUMMARY_INFO);
export const saveShopSummaryInfo = createAction(actionTypes.SAVE_SHOP_SUMMARY_INFO);
export const saveShopSummaryInfoStart = createAction(actionTypes.SAVE_SHOP_SUMMARY_INFO_START);
export const saveShopSummaryInfoEnd = createAction(actionTypes.SAVE_SHOP_SUMMARY_INFO_END);

export const getUserTypeValue = createAction(actionTypes.GET_USER_TYPE_VALUE);
export const upDateUserTypeValue = createAction(actionTypes.UPDATE_USER_TYPE_VALUE);

export const basicLoadingStart = createAction(actionTypes.LOADING_START_BASIC);
export const basicLoadingEnd = createAction(actionTypes.LOADING_END_BASIC);

export const batchBuildLoadingStart = createAction(actionTypes.LOADING_START_BATCH_BUILDING);
export const batchBuildLoadingEnd = createAction(actionTypes.LOADING_END_BATCH_BUILDING);

export const supportLoadingStart = createAction(actionTypes.LOADING_START_SUPPORT);
export const supportLoadingEnd = createAction(actionTypes.LOADING_END_SUPPORT);

export const relShopLoadingStart = createAction(actionTypes.LOADING_START_RELSHOP);
export const relShopLoadingEnd = createAction(actionTypes.LOADING_END_RELSHOP);

export const projectLoadingStart = createAction(actionTypes.LOADING_START_PROJECT);
export const projectLoadingEnd = createAction(actionTypes.LOADING_END_PROJECT);

export const ruleLoadingStart = createAction(actionTypes.LOADING_START_RULE);
export const ruleLoadingEnd = createAction(actionTypes.LOADING_END_RULE);

export const ruleTemplateLoadingStart = createAction(actionTypes.LOADING_START_RULE_TEMPLATE);
export const ruleTemplateLoadingEnd = createAction(actionTypes.LOADING_END_RULE_TEMPLATE);

export const commissionLoadingStart = createAction(actionTypes.LOADING_START_COMMISSION);
export const commissionLoadingEnd = createAction(actionTypes.LOADING_END_COMMISSION);

export const attchLoadingStart = createAction(actionTypes.LOADING_START_ATTCH);
export const attchLoadingEnd = createAction(actionTypes.LOADING_END_ATTCH);

export const shopBasicLoadingStart = createAction(actionTypes.LOADING_START_SHOP_BASIC);
export const shopBasicLoadingEnd = createAction(actionTypes.LOADING_END_SHOP_BASIC);

export const shopLeaseLoadingStart = createAction(actionTypes.LOADING_START_SHOP_LEASE);
export const shopLeaseLoadingEnd = createAction(actionTypes.LOADING_END_SHOP_LEASE);

export const shopSupportLoadingStart = createAction(actionTypes.LOADING_START_SHOP_SUPPORT);
export const shopSupportLoadingEnd = createAction(actionTypes.LOADING_END_SHOP_SUPORT);

export const shopSummaryLoadingStart = createAction(actionTypes.LOADING_START_SHOP_SUMMARY);
export const shopSummaryLoadingEnd = createAction(actionTypes.LOADING_END_SHOP_SUMMARY);

// 指派驻场审核列表
export const getExaminesList = createAction(actionTypes.GET_EXAMINESLIST_LIST);
export const getExaminesListStart = createAction(actionTypes.GET_EXAMINESLIST_LIST_START);
export const getExaminesListEnd = createAction(actionTypes.GET_EXAMINESLIST_LIST_END);
