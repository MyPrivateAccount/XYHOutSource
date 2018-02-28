import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux'
import buildingReducer from './building';
import basicDataReducer from './basicData';
import shopReducer from './shop';
import indexReducer from './indexReducer';
import centerReducer from './xkCenterReducer';
import managerReducer from './zcManageReducer';
import activeReducer from './houseActiveReducer';
import msgReducer from './msg'


export default combineReducers({
    building: buildingReducer,
    basicData: basicDataReducer,
    shop: shopReducer,
    index: indexReducer,
    center: centerReducer,
    manager: managerReducer,
    active: activeReducer,
    msg: msgReducer,
});