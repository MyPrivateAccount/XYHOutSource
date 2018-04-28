import {combineReducers} from 'redux';
// import { routerReducer } from 'react-router-redux'
import searchReducer from './search'
import orgDataReducer from './orgData'

export default combineReducers({
    search: searchReducer,
    orgData: orgDataReducer,
});