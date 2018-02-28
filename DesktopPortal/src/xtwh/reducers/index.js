import {combineReducers} from 'redux';
import areaReducer from './area'
import dicReducer from './dic'


export default combineReducers({
    area: areaReducer,
    dic: dicReducer
});