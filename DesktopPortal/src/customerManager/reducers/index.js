import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux'
import searchReducer from './search'
import basicDataReducer from './basicData'

export default combineReducers({
    search: searchReducer,
    basicData: basicDataReducer,

});