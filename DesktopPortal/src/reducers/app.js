import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';

const initState = {
    activeApp: {},
    operInfo: { objType: '', operType: '', snackbar: '' },
    appList: [],
    appPrivilege: [],
    judgePermissions: []//权限判断
};
let treeReducerMap = {};

treeReducerMap[actionTypes.APP_LIST_UPDATE] = function (state, action) {
    console.log("readucer获取applist", action.payload);
    return Object.assign({}, state, { appList: action.payload });
}

//权限获取
treeReducerMap[actionTypes.JUDGE_PERMISSION_COMPLETE] = function (state, action) {
    let judgePermissions = [...state.judgePermissions];
    if (action.payload) {
        action.payload.map(permission => {
            if (permission.isHave) {
                judgePermissions.push(permission.permissionItem);
            }
        });
    }
    return Object.assign({}, state, { judgePermissions: judgePermissions });
}
export default handleActions(treeReducerMap, initState);