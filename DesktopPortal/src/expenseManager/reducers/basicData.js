import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    chargeCostTypeList: []
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

export default handleActions(reducerMap,initState);