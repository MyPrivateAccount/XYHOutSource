import * as actionTypes from '../constants/actionType';
import {handleActions} from 'redux-actions'
const initState = {
    activeMenu: {},
    showLoading: false,
    navigator:[],
    searchKeyWord: '',
    searchResult:[],
}


let reducerMap = {};

reducerMap[actionTypes.CHNAGE_MENU] = function(state, action){
    return Object.assign(state, {actionMenu: action.payload, searchKeyWord:'', searchResult:[], navigator:[]});
}

reducerMap[actionTypes.SET_SEARCH_LOADING] = function(state, action){
    return Object.assign(state, {showLoading:action.payload});
}

export default handleActions(reducerMap, initState);