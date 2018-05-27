import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    ext:[]
};
let rpReducerMap = {};

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
export default handleActions(rpReducerMap, initState)