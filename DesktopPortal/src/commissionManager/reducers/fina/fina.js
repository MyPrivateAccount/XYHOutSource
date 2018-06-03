import { handleActions } from 'redux-actions';
import * as actionTypes from '../../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: ''},
    ext:[],
    dataSource:[],
    searchCondition:{}
};
let finaReducerMap = {};

finaReducerMap[actionTypes.FINA_QUERYPPFT_SUCCESS] = function (state, action) {
    console.log("readucer人员分摊表查询成功" + JSON.stringify(action.payload));
    return Object.assign({}, state, { dataSource:action.payload ,operInfo:{operType:'FINA_QUERYPPFT_SUCCESS'}});
}
export default handleActions(finaReducerMap, initState)