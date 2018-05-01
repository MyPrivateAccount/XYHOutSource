import {handleActions} from 'redux-actions'
import clone from 'clone'
import {notification} from 'antd';
import * as actionTypes from '../constants'

const initState = {
    curHotBuilding: [],
    curHotArea: [],
    curHotShops: [],
    hotBuildingSearchList: [],//热门楼盘查询结果列表
    hotBuildingSearchList2: [],//热门商铺的楼盘里诶包
    hotShopsSearchList: [],//热门商铺选择列表
}
let map = {};
map[actionTypes.SEARCH_BUILDING_END] = (state, action) => {
    let result = action.payload || {};
    return Object.assign({}, state, {hotBuildingSearchList: result.extension || []});
}

export default handleActions(map, initState);