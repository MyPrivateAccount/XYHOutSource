import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    contractInfo:{//合同信息
        id: NewGuid(),
        baseInfo:{},
        attachInfo:{},
        additionalInfo:{},
        modifyInfo:{},
        complementInfo:{},//补充协议
        discard:false,
        annexInfo:{},

    },
    operInfo: {
        basicOperType: 'add',
        attachFileOperType: 'add',
        attachPicOperType: 'add',
    },
    curFollowContract:{},
    completeFileList: [],
    deletePicList: [],
    submitLoading: false, // 提交按钮
    contractDisplay: 'block', // 点击提交合同后，展示view页面， 所有操作按钮隐藏。
    basicloading: false,
    attachloading: false,
    previewVisible: false,
    contractChooseVisible:false,//打开合同选择页
    isDisabled: false,
    isCurShowContractDetail: false,

    modifyHistoryVisible:false,

}

let reducerMap = {};

reducerMap[actionTypes.OPEN_MODIFY_HISTORY] = function(state,action){
    return Object.assign({}, state, { modifyHistoryVisible: true });
}

reducerMap[actionTypes.CLOSE_MODIFY_HISTORY] = function(state,action){
    return Object.assign({}, state, { modifyHistoryVisible: false });
}
reducerMap[actionTypes.OPEN_RECORD] = function(state, action){
    if(action.payload.record){
        let operInfo = {
            basicOperType: 'view',
            attachPicOperType: 'view',
        }
        let curFollowContract = {};
        if(action.payload.contractName){
            curFollowContract = {name:action.payload.contractName };
        }
        let contractInfo = {...state.contractInfo}
        let newContractInfo = Object.assign({}, contractInfo, {baseInfo:action.payload.record});
        let newState = Object.assign({}, state, { contractInfo: newContractInfo, basicloading: false,operInfo: operInfo, contractDisplay: 'block', isCurShowContractDetail: true, curFollowContract:curFollowContract});
        return newState; 
    }
    const id = NewGuid();
    let contractInfo={//合同信息
        id: id,
        baseInfo:{
            id: id,
        },
        //contractAttachInfo:{},
        discard:false,
        complementInfo:{},//补充协议
        annexInfo:{},
        attachInfo:{},

    }
    let operInfo = {
        basicOperType: 'add',
        attachPicOperType: 'add',
    }

    let newState = Object.assign({}, state, { contractInfo: contractInfo, basicloading: false,operInfo: operInfo, contractDisplay: 'block' , isCurShowContractDetail: false,curFollowContract:{} });
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

    //let contractInfo = Object.assign({}, state.contractInfo, {baseInfo:action.payload.baseInfo, modifyInfo:action.payload.modifyInfo});
    
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo/*, contractInfo: contractInfo*/ });
    //console.log('newstate:', newState);
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
        contractInfo.baseInfo = contractInfo.baseInfo
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
    let newState = Object.assign({}, state, {contractChooseVisible:true , curFollowContract: {}});
    return newState;
}

reducerMap[actionTypes.CLOSE_CONTRACT_CHOOSE] = (state, action) =>{
    let curFollowContract = {}
    if(action.payload){
        curFollowContract = action.payload.record
    }
    let newState = Object.assign({}, state, {contractChooseVisible:false, curFollowContract: curFollowContract});
    return newState;
}

//图片信息编辑  //此时的情况是应该已经拿到了当前的合同id
reducerMap[actionTypes.CONTRACT_PIC_EDIT] = (state, action) => {
    let contractInfo = { ...state.contractInfo };
    let operInfo = Object.assign({}, state.operInfo, { attachPicOperType: 'edit' });
    let newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo });
    return newState;
  }
  // 图片信息查看
  // map[actionTypes.SHOP_PIC_VIEW] = (state, action) => {
  //   let shopsInfo = { ...state.shopsInfo };
  //   let operInfo = Object.assign({}, state.operInfo, { attachPicOperType: 'view' });
  //   let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  //   return newState;
  // }
  reducerMap[actionTypes.CONTRACT_PIC_VIEW] = (state, action) => {
    console.log("进入预览附件");
    const type = action.payload.type // add' 新增， 'delete'删除， 'cancel' 取消这是上一次操作的动作
    let contractInfo = { ...state.contractInfo }
    let attachInfo, oldFileList, nowFileList
   

    attachInfo = { ...state.contractInfo.attachInfo }
    oldFileList = [...state.contractInfo.attachInfo.fileList || []]
    nowFileList = action.payload.filelist || [];
    
    let operInfo = { ...state.operInfo, attachPicOperType: 'view' };
    if (type === 'delete') {
      nowFileList.forEach((v, i) => {
        let num = oldFileList.findIndex((item) => {
          return v.uid === item.fileGuid
        })
        if (num !== -1) {
          let myIndex = num;
          oldFileList.splice(myIndex, 1)
        }
      })
    } else {
      nowFileList.forEach((v, i) => {
        let num = oldFileList.findIndex((item) => {
          return v.fileGuid === item.fileGuid
        })
        if (num === -1) {
          if (type === 'add') {
            // console.log('add');
            oldFileList.push(v);
          }
        } else {
          if (type === 'cancel') {
            // console.log('cancel');
            oldFileList = oldFileList;
          }
        }
      })
    }

    attachInfo = Object.assign({}, state.contractInfo.attachInfo, { fileList: oldFileList })
    contractInfo = Object.assign({}, contractInfo, {attachInfo: attachInfo});
    let newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo});
    console.log("离开预览附件:", newState);
    return newState;
  }
  
  // 存上传照片
  reducerMap[actionTypes.SAVE_COMPLETE_FILE_LIST] = (state, action) => {
    let completeFileList = []
    completeFileList = action.payload.completeFileList
    let newState = Object.assign({}, state, { completeFileList: completeFileList });
    return newState;
  }
  // 存删除的照片数组
  reducerMap[actionTypes.SAVE_DELETE_PIC_LIST] = (state, action) => {
    // console.log(state.completeFileList, action.payload.deletePicList,'上传的图片数组啊啊啊')
    let deletePicList = [], complete = []
    deletePicList = action.payload.deletePicList
    complete = state.completeFileList
    if (state.completeFileList.length !== 0) {
      deletePicList.map((v) => {
        let index = complete.findIndex((item) => {
          return item.fileGuid === v.uid
        })
        // console.log(index, '???')
        complete.splice(index, 1)
      })
    }
    let newState = Object.assign({}, state, { deletePicList: deletePicList, completeFileList: complete });
    return newState;
  }

reducerMap[actionTypes.LOADING_START_ATTACH] = function (state, action) {
    return Object.assign({}, state, { attachloading: true });
}
reducerMap[actionTypes.LOADING_END_ATTACH] = function (state, action) {
    return Object.assign({}, state, { attachloading: false });
}

reducerMap[actionTypes.OPEN_ATTACHMENT] = function(state, action){
    let operInfo = { ...state.operInfo, attachPicOperType: 'add' };
    return Object.assign({}, state, {operInfo: operInfo,  isCurShowContractDetail: false});
}

reducerMap[actionTypes.CLOSE_ATTACHMENT] = function(state,action){
    return Object.assign({}, state, {attachFileOperType: 'view'})
}
export default handleActions(reducerMap, initState);




