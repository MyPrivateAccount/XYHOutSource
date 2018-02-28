import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux'
import flowReducer from './flowChart'


export default combineReducers({
    flowChart: flowReducer,
});