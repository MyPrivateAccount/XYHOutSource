import { createAction } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
//基础字典数据
export const getDicParList = createAction(actionTypes.DIC_GET_PARLIST);
export const getAreaList = createAction(actionTypes.DIC_GET_AREA);
export const getOrgList = createAction(actionTypes.DIC_GET_ORG_LIST);
export const getOrgDetail = createAction(actionTypes.DIC_GET_ORG_DETAIL);
export const getUserByOrg = createAction(actionTypes.GET_ORG_USERLIST);
export const getAllOrgList = createAction(actionTypes.DIC_GET_ALL_ORG_LIST);
//设置遮罩层
export const setLoadingVisible = createAction(actionTypes.SET_SEARCH_LOADING);
//合同的录入
export const saveContractBasic = createAction(actionTypes.CONTRACT_BASIC_SAVE);
export const editContractBasic = createAction(actionTypes.CONTRACT_BASIC_EDIT);
export const viewContractBasic = createAction(actionTypes.CONTRACT_BASIC_VIEW);

// 提交合同信息
export const submitContractInfo = createAction(actionTypes.CONTRACT_INFO_SUBMIT);
export const submitContractStart = createAction(actionTypes.CONTRACT_INFO_SUBMIT_START);
export const submitContractFinish = createAction(actionTypes.CONTRACT_INFO_SUBMIT_FINISH);

//搜索处理
export const changeKeyWord = createAction(actionTypes.CHANGE_KEYWORD);
export const searchStart = createAction(actionTypes.SEARCH_START);
export const saveSearchCondition = createAction(actionTypes.SAVE_SEARCH_CONDITION);

//跳转界面
export const gotoThisContract = createAction(actionTypes.GOTO_THIS_CONTRACT);
export const gotoThisContractStart = createAction(actionTypes.GOTO_THIS_CONTRACT_START);
export const gotoThisContractFinish = createAction(actionTypes.GOTO_THIS_CONTRACT_FINISH);
export const gotoChangeMyAdd = createAction(actionTypes.GOTO_CHANGE_MYADD);
// 上传图片
// export const savePictureAsync = createAction(actionTypes.SAVE_PICTURE_ASYNC);
// export const uploadPicFinish = createAction(actionTypes.UPLOAD_PIC_FINISH);

// export const buildingPicView = createAction(actionTypes.BUILDING_PIC_VIEW);
// export const buildingPicEdit = createAction(actionTypes.BUILDING_PIC_EDIT);
// export const deletePicAsync = createAction(actionTypes.DELETE_PICTURE_ASYNC);
// export const saveCompleteFileList = createAction(actionTypes.SAVE_COMPLETE_FILE_LIST);
// export const saveDeletePicList = createAction(actionTypes.SAVE_DELETE_PIC_LIST);

export const savePictureAsync = createAction(actionTypes.CONTRACT_SAVE_PICTURE_ASYNC);
//export const uploadPicFinish = createAction(actionTypes.UPLOAD_PIC_FINISH);
export const contractPicView = createAction(actionTypes.CONTRACT_PIC_VIEW);
export const contractPicEdit = createAction(actionTypes.CONTRACT_PIC_EDIT);
export const deletePicAsync = createAction(actionTypes.DELETE_PICTURE_ASYNC);
export const saveCompleteFileList = createAction(actionTypes.SAVE_COMPLETE_FILE_LIST);
export const saveDeletePicList = createAction(actionTypes.SAVE_DELETE_PIC_LIST);

//文件上传
export const openAttachMent = createAction(actionTypes.OPEN_ATTACHMENT);
export const openAttachMentStart = createAction(actionTypes.OPEN_ATTACHMENT_START);
export const openAttachMentFinish = createAction(actionTypes.OPEN_ATTACHMENT_FINISH);
export const closeAttachMent = createAction(actionTypes.CLOSE_ATTACHMENT);
export const uploadAttachMentList = createAction(actionTypes.UPLOAD_ATTCHMENT_LIST);
export const uploadAttachMentListComplete = createAction(actionTypes.UPLOAD_ATTCHMENT_LIST_COMPLETE);


//合同录入
export const openContractRecord = createAction(actionTypes.OPEN_RECORD);
export const openContractRecordNavigator = createAction(actionTypes.OPEN_RECORD_NAVIGATOR);
export const closeContractReord = createAction(actionTypes.CLOSE_RECORD);

//export const subMitContractInfo = createAction(actionTypes.SUBMIT_CONTRACT_INFO);
//export const clearContractInfo = createAction(actionTypes.CLEAR_CONTRACT_INFO);

//导出
export const exportContract = createAction(actionTypes.EXPORT_CONTRACT);
export const exportMultiContract = createAction(actionTypes.EXPORT_MULTI_CONTRACT);


//合同详情处理
export const openContractDetail = createAction(actionTypes.OPEN_CONTRACT_DETAIL);
export const closeContractDetail = createAction(actionTypes.CLOSE_CONTRACT_DETAIL);
export const changeContractMenu = createAction(actionTypes.CHANGE_MENU);
export const getContractDetail = createAction(actionTypes.GET_CONTRACT_DETAIL);


export const openContractChoose = createAction(actionTypes.OPEN_CONTRACT_CHOOSE);
export const closeContractChoose = createAction(actionTypes.CLOSE_CONTRACT_CHOOSE);


//
export const openModifyHistory = createAction(actionTypes.OPEN_MODIFY_HISTORY);
export const closeModifyHistory = createAction(actionTypes.CLOSE_MODIFY_HISTORY);


//补充协议
export const openComplement = createAction(actionTypes.OPEN_COMPLEMENT);
export const openComplementStart = createAction(actionTypes.OPEN_COMPLEMENT_START);
export const openComplementFinish = createAction(actionTypes.OPEN_COMPLEMENT_FINISH);
export const closeComplement = createAction(actionTypes.CLOSE_COMPLEMENT);
export const contractComplementEdit = createAction(actionTypes.CONTRACT_COMPLEMENT_EDIT);
export const contractComplementSave = createAction(actionTypes.CONTRACT_COMPLEMENT_SAVE);



export const getAllPhone = createAction(actionTypes.GET_CUSTOMER_ALL_PHONE);



//单位选择处理
export const openOrgSelect = createAction(actionTypes.OPEN_ORG_SELECT);
export const closeOrgSelect = createAction(actionTypes.CLOSE_ORG_SELECT);
export const changeActiveOrg = createAction(actionTypes.CHAGNE_ACTIVE_ORG);


//临时更改设置当前用户的部门id为当前选择orgid
export const setInitActiveOrg = createAction(actionTypes.SET_INIT_ACTIVEORG);

//甲方公司
export const companyAEdit = createAction(actionTypes.COMPANYA_EDIT);
export const companyAAdd = createAction(actionTypes.COMPANYA_ADD);
export const companyASave = createAction(actionTypes.COMPANYA_SAVE);
export const companyADelete = createAction(actionTypes.COMPANYA_DELETE);
export const companyAListUpdate = createAction(actionTypes.COMPANYA_LIST_UPDATE);
export const companyACloseDialog = createAction(actionTypes.COMPANYA_DIALOG_CLOSE);
export const companyListGet = createAction(actionTypes.COMPANYA_GET_LIST);








//调客处理
export const openAdjustCustomer = createAction(actionTypes.OPEN_ADJUST_CUSTOMER);
export const closeAdjustCustomer = createAction(actionTypes.CLOSE_ADJUST_CUSTOMER);
export const adjustCustomer = createAction(actionTypes.ADJUST_CUSTOMER);
export const getAuditList = createAction(actionTypes.GET_AUDIT_LIST);
export const getCustomerByUserID = createAction(actionTypes.GET_CUSTOMER_OF_USERID);
export const changeSourceOrg = createAction(actionTypes.CHANGE_SOURCE_ORG);
export const changeTargetOrg = createAction(actionTypes.CHANGE_TARGET_ORG);
export const openCustomerAuditDetail = createAction(actionTypes.OPEN_CUSTOMER_AUDIT_INFO);
export const getAuditHistory = createAction(actionTypes.GET_AUDIT_HISTORY);//获取审核历史


export const getRepeatJudgeInfo = createAction(actionTypes.GET_REPEAT_JUDGE_INFO);
export const removeAdjustItem = createAction(actionTypes.REMOVE_ADJUST_REQUEST_ITEM);

export const basicLoadingStart = createAction(actionTypes.LOADING_START_BASIC);
export const basicLoadingEnd = createAction(actionTypes.LOADING_END_BASIC);
export const attchLoadingStart = createAction(actionTypes.LOADING_START_ATTACH);
export const attchLoadingEnd = createAction(actionTypes.LOADING_END_ATTACH);


export const basicSubmitEnd = createAction(actionTypes.BASIC_SUBMIT_END);
export const attachSubmitEnd = createAction(actionTypes.ATTACH_SUBMIT_END);
