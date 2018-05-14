import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    chargeCostTypeList: [{value: 0, key: "test"}],
    navigator: [{menuID: 'menu_index', disname: '费用信息'}, {menuID: 'home', disname: '费用'}]
};

let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let chargeCostTypeList = [...state.chargeCostTypeList];
    action.payload.map((group) => {
        if (group.groupId === "CHARGE_COST_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            chargeCostTypeList = group.dictionaryDefines;
        }
    })

    return Object.assign({}, state, {
        chargeCostTypeList: chargeCostTypeList,
    });
}

reducerMap[actionTypes.CLOSE_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: []});
}

reducerMap[actionTypes.SET_USER_BREADINDEX] = function(state, action) {
    return Object.assign({}, state, {navigator: state.navigator.slice(0, action.payload+1)});
}

reducerMap[actionTypes.SET_USER_BREAD] = function(state, action) {
    return Object.assign({}, state, {navigator: action.payload});
}

reducerMap[actionTypes.SET_USER_BREADADD] = function(state, action) {
    state.navigator.push(action.payload);
    return Object.assign({}, state, {navigator: state.navigator.slice()});
}

export default handleActions(reducerMap,initState);