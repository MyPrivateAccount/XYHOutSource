import { combineReducers } from 'redux';
import { routerReducer } from 'react-router-redux'
import appReducer from './app'
import orgReducer from './org'
import roleReducer from './role'
import privilegeReducer from './privilege'
import empReducer from './emp'

export default combineReducers({
    app: appReducer,
    org: orgReducer,
    role: roleReducer,
    privilege: privilegeReducer,
    emp: empReducer
});