import {handleActions} from 'redux-actions'
import {notification} from 'antd';
import clone from 'clone';
import * as actionTypes from '../constants'

const initState = {
    curCity: null,
    loading: false,
    footPrintDataSource: [], //足迹数据源
    pagination: {current: 1, pageSize: 10, total: 0}//跟进数据源
}

let map = {};

map[actionTypes.FPRINT_GET_LIST_END] = (state, action) => {
    let result = action.payload || {};
    let footPrintDataSource = result.extension || [];
    let current = result.pageIndex || 0;
    let pageSize = result.pageSize || 10;
    let total = result.totalCount || 0;
    return Object.assign({}, state, {footPrintDataSource: footPrintDataSource, pagination: {current: current, pageSize: pageSize, total: total}});
}
//切换城市
map[actionTypes.CHANGE_CUR_CITY] = (state, action) => {
    return Object.assign({}, state, {curCity: action.payload});
}

export default handleActions(map, initState);