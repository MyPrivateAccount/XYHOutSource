import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    result:[]
};
let monthReducerMap = {};

monthReducerMap[actionTypes.YJ_MONTH_GETUPDATE] = function (state, action) {
    console.log("readucer月结月份查询接口返回结果" + JSON.stringify(action.payload));
    return Object.assign({}, state, { result:action.payload ,operInfo:{operType:'YJ_MONTH_GETUPDATE'}});
}
export default handleActions(monthReducerMap, initState)