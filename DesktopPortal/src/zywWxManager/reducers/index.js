import {combineReducers} from 'redux';
import hotShopReducer from './hotShop'
import footPrintReducer from './footPrint'
import dayReportReducer from './dayReport'


export default combineReducers({
    hotShop: hotShopReducer,
    footPrint: footPrintReducer,
    dayReport: dayReportReducer
});