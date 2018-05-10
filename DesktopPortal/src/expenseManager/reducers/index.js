import {combineReducers} from 'redux';
import basicData from './basicData';
import search from './search';
export default combineReducers({
    basicData:basicData,
    search:search,
});