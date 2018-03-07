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
}

let reduceMap = {};


reduceMap[actionTypes.OPEN_RECORD] = function(state, action){
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
    let newState = Object.assign({}, state, { contractInfo: contractInfo, operInfo: operInfo, contractDisplay: 'block' });
    return newState; 
}
export default handleActions(reduceMap, initState);




