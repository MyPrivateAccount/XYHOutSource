import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    contractInfo:{//合同信息
        id: NewGuid(),
        contractBasicInfo:{},
        contractAttachInfo:{},
        additionalInfo:{},
        modifyRecord:{},

    },
    operInfo: {
        basicOperType: 'add',
        attachPicOperType: 'add',
    },
    submitLoading: false, // 提交按钮
    contractDisplay: 'block', // 点击提交合同后，展示view页面， 所有操作按钮隐藏。
    basicloading: false,
    supportloading: false,
    attachloading: false,
    previewVisible: false,

}

let reducerMap = {};


reducerMap[actionTypes.OPEN_RECORD] = function(state, action){
    let contractInfo={//合同信息
        id: NewGuid(),
        contractBasicInfo:{
            
        },
        //contractAttachInfo:{},

    }
    let operInfo = {
        basicOperType: 'add',
        //attachPicOperType: 'add',
    }

    let newState = Object.assign({}, state, { contractInfo: contractInfo, basicloading: false,operInfo: operInfo, contractDisplay: 'block' });
    return newState; 
}

// 保存各个模板的loading
reducerMap[actionTypes.LOADING_START_BASIC] = function (state, action) {
    return Object.assign({}, state, { basicloading: true });
}
reducerMap[actionTypes.LOADING_END_BASIC] = function (state, action) {
    return Object.assign({}, state, { basicloading: false });
}


// 基本信息编辑
reducerMap[actionTypes.CONTRACT_BASIC_EDIT] = (state, action) => {
    let contractInfo = { ...state.contractInfo };
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'edit' });
    let newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo });
    return newState;
  }
  // 基本信息查看
reducerMap[actionTypes.CONTRACT_BASIC_VIEW] = (state, action) => {
    let contractInfo = { ...state.shopsInfo };
  
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo });
    return newState;
  }
  
// 点击提交按钮
reducerMap[actionTypes.CONTRACT_INFO_SUBMIT_START] = function (state, action) {
    let newState = Object.assign({}, state, { submitLoading: true });
    return newState;
}
reducerMap[actionTypes.CONTRACT_INFO_SUBMIT_FINISH] = function (state, action) {
    let operInfo = {
        basicOperType: 'view',

    }
    let newState = Object.assign({}, state, { submitLoading: false, operInfo: operInfo, buildDisplay: 'none' });
    return newState;
}
export default handleActions(reducerMap, initState);




