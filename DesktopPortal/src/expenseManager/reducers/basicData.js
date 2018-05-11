import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    chargeCostType: []
};

let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let chargeCostType = [...state.chargeCostType];
    action.payload.map((group) => {
        if (group.groupId === "CHARGE_COST_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            chargeCostType = group.dictionaryDefines;
        }
    })

    return Object.assign({}, state, {
        chargeCostType: chargeCostType,
    });
}

export default handleActions(reducerMap,initState);