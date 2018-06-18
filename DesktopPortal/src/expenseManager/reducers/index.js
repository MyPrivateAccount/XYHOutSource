import {combineReducers} from 'redux';
//import basicData from './basicData';
import search from './search';
import {routerReducer} from 'react-router-redux'

export default combineReducers({
  //  basicData:basicData,
    search:search,
    router: routerReducer,
});