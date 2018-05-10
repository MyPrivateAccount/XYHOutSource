import * as actionTypes from '../constants/actionTypes';
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

export default handleActions(reducerMap, initState);