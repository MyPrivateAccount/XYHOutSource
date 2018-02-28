import {combineReducers} from 'redux';
import {routerReducer} from 'react-router-redux'
import {reducer as oidcReducer} from 'redux-oidc';
import subscriptionsReducer from './subscriptions';
import windows from './windows';
import appReducer from './app';
import messageReducer from './message';
import basicDataReducer from './basicData';

export default combineReducers({
    oidc: oidcReducer,
    router: routerReducer,
    subscriptions: subscriptionsReducer,
    windows: windows,
    app: appReducer,
    message: messageReducer,
    basicData: basicDataReducer
});