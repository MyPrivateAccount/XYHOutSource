import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux';
import searchReducer from './search';
import basicDataReducer from './basicData';
import {reducer as oidcReducer} from 'redux-oidc';

export default combineReducers({
    oidc: oidcReducer,
    router: routerReducer,
    search: searchReducer,
    basicData: basicDataReducer
});