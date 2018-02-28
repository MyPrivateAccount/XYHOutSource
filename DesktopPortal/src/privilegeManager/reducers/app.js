import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';


const action = appAction('PrivilegeManagerIndex')

const initState = {
    activeApp: {},
    operInfo: { operType: '' },
    appList: [],
    appPrivilege: []
};
let appReducerMap = {};
appReducerMap[actionTypes.APP_ADD] = function (state, action) {
    console.log("readucer应用新增");
    return Object.assign({}, state, { operInfo: { operType: 'add' } });
}
appReducerMap[actionTypes.APP_EDIT] = function (state, action) {
    console.log("readucer应用编辑" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { operType: 'edit' }, activeApp: action.payload });
}
// appReducerMap[actionTypes.APP_MESSAGE] = function (state, action) {
//     console.log("readucer后台通知:" + JSON.stringify(action.payload));
//     let result = action.payload;
//     if (result.msg) {
//         notification[result.isOk ? 'success' : 'error']({
//             message: '应用',
//             description: result.msg,
//             duration: 3
//         });
//     }
//     return Object.assign({}, state);
// }
appReducerMap[actionTypes.APP_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer弹出框关闭");
    return Object.assign({}, state, { operInfo: { operType: '' } });
}
appReducerMap[actionTypes.APP_LIST_UPDATE] = function (state, action) {
    console.log("readucer获取applist", action.payload);
    return Object.assign({}, state, { appList: action.payload });
}
appReducerMap[actionTypes.APP_PRIVILEGE_UPDATE] = function (state, action) {
    console.log("readucer显示应用对应权限");
    return Object.assign({}, state, { appPrivilege: action.payload });
}
appReducerMap[actionTypes.APP_EXPAND_CHANGE] = function (state, action) {
    console.log('readucer展开' + JSON.stringify(action.payload));
    console.log('state:' + JSON.stringify(state));
    var treeSource = state.roleSource.slice();
    //ToggleExpandStatus(treeSource, action.payload.id);
    return Object.assign({}, state, { roleSource: treeSource });
}
export default handleActions(appReducerMap, initState);