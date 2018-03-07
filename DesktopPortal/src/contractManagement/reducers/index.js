import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux';
import searchReducer from './search';
import basicDataReducer from './basicData';
import contractReducer from './contract';
export default combineReducers({
    search: searchReducer,
    basicData: basicDataReducer,
    contractData: contractReducer,
});