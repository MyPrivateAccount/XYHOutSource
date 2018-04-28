import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux'
import auditReducer from './audit'
//import houseActiveReducer from './houseActive'

export default combineReducers({
    audit: auditReducer,
    //activeInfo: houseActiveReducer
});