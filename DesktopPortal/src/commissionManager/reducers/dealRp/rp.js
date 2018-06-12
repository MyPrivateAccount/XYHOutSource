import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    ext:[],
    rpSearchResult:[],
    rpCJBBResult:[],
    searchCondition:{
        pageIndex:0,
        pageSize:10,
    }
};
let rpReducerMap = {};
//保存接口反馈
rpReducerMap[actionTypes.DEALRP_RP_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新交易合同" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'HTSAVE_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_WY_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新物业信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'WYSAVE_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_YZ_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新业主信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'YZSAVE_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_KH_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新客户信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'KHSAVE_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_GH_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新过户信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'GHSAVE_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_FP_SAVEUPDATE] = function (state, action) {
    console.log("readucer更新业绩分配信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'FPSAVE_UPDATE'}});
}
//获取接口反馈
rpReducerMap[actionTypes.DEALRP_RP_GETUPDATE] = function (state, action) {
    console.log("readucer获取到交易合同" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'HTGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_WY_GETUPDATE] = function (state, action) {
    console.log("readucer获取到物业信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'WYGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_YZ_GETUPDATE] = function (state, action) {
    console.log("readucer获取到业主信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'YZGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_KH_GETUPDATE] = function (state, action) {
    console.log("readucer获取到客户信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'KHGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_GH_GETUPDATE] = function (state, action) {
    console.log("readucer获取到过户信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'GHGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_FP_GETUPDATE] = function (state, action) {
    console.log("readucer获取到业绩分配信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'FPGET_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_MYREPORT_GETUPDATE] = function (state, action) {
    console.log("readucer获取到成交报告列表信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { rpSearchResult:action.payload ,operInfo:{operType:'DEALRP_MYREPORT_GETUPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_REPORT_SEARCH_UPDATE] = function (state, action) {
    console.log("readucer搜索到成交报告列表信息" + JSON.stringify(action.payload));
    return Object.assign({}, state, { rpSearchResult:action.payload ,operInfo:{operType:'DEALRP_REPORT_SEARCH_UPDATE'}});
}
rpReducerMap[actionTypes.DEALRP_ATTACT_UPLOAD_COMPLETE] = function (state, action) {
    console.log("readucer上传文件成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { ext:action.payload ,operInfo:{operType:'DEALRP_ATTACT_UPLOAD_COMPLETE'}});
}
rpReducerMap[actionTypes.DEALRP_CJBB_LISTUPDATE] = function (state, action) {
    console.log("readucer获取成交报备列表成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { rpCJBBResult:action.payload ,operInfo:{operType:'DEALRP_CJBB_LISTUPDATE'}});
}
export default handleActions(rpReducerMap, initState)