import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';

const initState = {
    showLoading: false,
    operInfo: { operType: '' },
    companyAList: [],
    activeCompanyA: {},
    companySearchResult: [],//查询结果

    allCompanyAData:[],
    isShowCompanyADialog: false,
};
let reducerMap = {};

reducerMap[actionTypes.COMPANYA_ADD] = function (state, action) {
    console.log("readucer新增" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: 'comA', operType: 'add', dialogOpen: true } });
}
reducerMap[actionTypes.COMPANYA_EDIT] = function (state, action) {
    console.log("readucer编辑" + JSON.stringify(action.payload));
    let activeCompanyA = action.payload;
    return Object.assign({}, state, { activeCompanyA: activeCompanyA, operInfo: { objType: 'comA', operType: 'edit', dialogOpen: true } });
}

reducerMap[actionTypes.COMPANYA_LIST_UPDATE] = function (state, action) {
    console.log("readucer列表:" + JSON.stringify(action.payload));
    return Object.assign({}, state, { companyAList: action.payload });
}

reducerMap[actionTypes.COMPANYA_DIALOG_CLOSE] = function (state, action) {
    console.log("readucer退出dialog" + JSON.stringify(action.payload));
    return Object.assign({}, state, { operInfo: { objType: '', operType: '', empRoleOperType: '' } });
}


// //删除
// reducerMap[actionTypes.COMPANYA_DELETE] = function (state, action) {

//     return Object.assign({}, state, { activeEmp: action.payload, operInfo: { empRoleOperType: 'edit' } });
// }
//查询
reducerMap[actionTypes.COMPANYA_SERACH_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { companySearchResult: action.payload });
}

//设置遮罩层
reducerMap[actionTypes.SET_SEARCH_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}


reducerMap[actionTypes.OPEN_COMPANYA_DIALOG] = function(state,action){
    let allCompanyAData = {};
    if(action.payload)
    {
        allCompanyAData = action.payload;
        // if(allCompanyAData === null || allCompanyAData.length === 0){
        //     return state;
        // }
    }
    return Object.assign({}, state, { isShowCompanyADialog: true, allCompanyAData: allCompanyAData });
}

reducerMap[actionTypes.CLOSE_COMPANYA_DIALOG] = function(state,action){
    return Object.assign({}, state, { isShowCompanyADialog: false });
}
export default handleActions(reducerMap, initState)