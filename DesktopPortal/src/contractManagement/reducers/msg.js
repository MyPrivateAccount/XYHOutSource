import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false
}
let reducerMap = {};
//更改加载状态
reducerMap[actionTypes.SET_MSG_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}

export default handleActions(reducerMap, initState); 