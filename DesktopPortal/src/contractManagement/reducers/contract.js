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
    contractChooseVisible:false,//打开合同选择页


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
    let contractInfo = Object.assign({}, { ...state.contractInfo.contractBasicInfo }, {contractBasicInfo:action.body});
    
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo });
    console.log('newstate:', newState);
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


// 点击进入这个合同（用于在保存后编辑，然后准备阶段）
reducerMap[actionTypes.GOTO_THIS_CONTRACT_START] = (state, action) => {
    let newState = state;
    return newState;
}
reducerMap[actionTypes.GOTO_THIS_CONTRACT_FINISH] = (state, action) => {
    // console.log(state.buildInfo, '旧值', action)
    let contractInfo = { ...state.contractInfo };
    let operInfo = { ...state.operInfo };
    let res = action.payload.data

    if (res.code === '0') {
        contractInfo = res.extension
        contractInfo.buildingBasic = contractInfo.basicInfo
        contractInfo.buildingBasic.location = [contractInfo.basicInfo.city, contractInfo.basicInfo.district, contractInfo.basicInfo.area]
        contractInfo.supportInfo = contractInfo.facilitiesInfo
        contractInfo.relShopInfo = contractInfo.shopInfo
        contractInfo.projectInfo = { summary: contractInfo.summary };
        contractInfo.attachInfo = { fileList: contractInfo.fileList, attachmentList: contractInfo.attachmentList }
        if ([1, 8].includes(res.extension.examineStatus)) {
            operInfo = {
                basicOperType: 'view',
                supportOperType: 'view',
                relShopOperType: 'view',
                projectOperType: 'view',
                attachPicOperType: 'view',
                attachFileOperType: 'view',
                batchBuildOperType: 'view',
                rulesOperType: 'view',
                rulesTemplateOperType: 'view',
                commissionOperType: 'view',
            }
        } else {
            operInfo = {
                basicOperType: 'edit',
                supportOperType: 'edit',
                relShopOperType: 'edit',
                projectOperType: 'edit',
                attachPicOperType: 'edit',
                attachFileOperType: 'edit',
                batchBuildOperType: 'edit',
                rulesOperType: 'edit',
                rulesTemplateOperType: 'edit',
                commissionOperType: 'edit',
            }
        }

    }
    let newState = Object.assign({}, state, { contractInfo: contractInfo, operInfo: operInfo, buildDisplay: 'block' });
    console.log(newState, '新值')
    return newState;
}

reducerMap[actionTypes.OPEN_CONTRACT_CHOOSE] = (state, action) =>{
    let newState = Object.assign({}, state, {contractChooseVisible:true});
    return newState;
}

reducerMap[actionTypes.CLOSE_CONTRACT_CHOOSE] = (state, action) =>{
    let newState = Object.assign({}, state, {contractChooseVisible:false});
    return newState;
}
export default handleActions(reducerMap, initState);




