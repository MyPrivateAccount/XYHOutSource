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
        complementInfos:{},//补充协议
        discard:false,
        annexInfo:{},

    },
    operInfo: {
        basicOperType: 'add',
        attachFileOperType: 'add',
        attachPicOperType: 'add',
        complementOperType: 'add',
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
       // attachPicOperType: 'add',
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

    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    let contractInfo = {};
    let newState = {};
    if(action.payload.baseInfo){
        contractInfo = Object.assign({}, state.contractInfo, {baseInfo:action.payload.baseInfo});
        newState = Object.assign({}, state, { operInfo: operInfo, contractInfo: contractInfo});
    }
    else{
        newState = Object.assign({}, state, { operInfo: operInfo/*, contractInfo: contractInfo*/ });
    }
    
    
    
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



reducerMap[actionTypes.GOTO_THIS_CONTRACT_FINISH] = (state, action) => {
    console.log('action.payload.data', action.payload.data);
    let contractInfo = { ...state.contractInfo };
    let operInfo = { ...state.operInfo };
    let res = action.payload.data

    if (res.code === '0') {
        contractInfo = res.extension
        contractInfo.baseInfo = contractInfo.baseInfo
        contractInfo.attachInfo = { fileList: contractInfo.fileList }
       
        operInfo = {
            basicOperType: 'view',
            attachPicOperType: 'view',
   
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
    if(type === 'save')
    {
        oldFileList = action.payload.filelist;
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

// reducerMap[actionTypes.OPEN_ATTACHMENT] = function(state, action){
   
//     let contractInfo = Object.assign({}, {...state.contractInfo }, {baseInfo: action.payload.record})
//     let operInfo = { ...state.operInfo, attachPicOperType: 'add' };
//     return Object.assign({}, state, {operInfo: operInfo,  isCurShowContractDetail: false, contractInfo:contractInfo});
// }

reducerMap[actionTypes.OPEN_ATTACHMENT_FINISH] = function(state, action){
   
    let fileList = action.payload.fileList
    let attachInfo = Object.assign({}, {fileList:fileList });
    let contractInfo = Object.assign({}, {...state.contractInfo }, {baseInfo: action.payload.baseInfo, attachInfo:attachInfo});
    let attachPicOperType  = "add";
    if(fileList.length > 0)
    {
        attachPicOperType = "edit";
    }
    let operInfo = { ...state.operInfo, attachPicOperType: attachPicOperType };
    return Object.assign({}, state, {operInfo: operInfo,  isCurShowContractDetail: false, contractInfo:contractInfo});
}

reducerMap[actionTypes.OPEN_COMPLEMENT_FINISH] = function(state, action){
   
    let complementInfo = action.payload.complementInfo
    let complementInfos = Object.assign({}, {complementInfo:complementInfo });
    let contractInfo = Object.assign({}, {...state.contractInfo }, {baseInfo: action.payload.baseInfo, complementInfos:complementInfos});
    let complementOperType  = "add";
    if(complementInfo.length > 0)
    {
        complementOperType = "edit";
    }
    let operInfo = { ...state.operInfo, complementOperType: complementOperType };
    return Object.assign({}, state, {operInfo: operInfo,  isCurShowContractDetail: false, contractInfo:contractInfo});
}

reducerMap[actionTypes.CLOSE_ATTACHMENT] = function(state,action){
    return Object.assign({}, state, {attachFileOperType: 'view'})
}

reducerMap[actionTypes.BASIC_SUBMIT_END] = function(state, action){
    let operInfo = {attachPicOperType: "", basicOperType:""};
    let contractInfo={//合同信息
            id: "",
            baseInfo:{},
            attachInfo:{},
            additionalInfo:{},
            modifyInfo:{},
            complementInfo:{},//补充协议
            discard:false,
            annexInfo:{},
    
        }
    return Object.assign({}, state, {operInfo:operInfo, contractInfo:contractInfo});
}

reducerMap[actionTypes.ATTACH_SUBMIT_END] = function(state, action){
    let operInfo = {attachPicOperType: "", basicOperType:""};
    let contractInfo={//合同信息
            id: "",
            baseInfo:{},
            attachInfo:{},
            additionalInfo:{},
            modifyInfo:{},
            complementInfo:{},//补充协议
            discard:false,
            annexInfo:{},
    
        }
    return Object.assign({}, state, {operInfo:operInfo, contractInfo:contractInfo});
}


export default handleActions(reducerMap, initState);




