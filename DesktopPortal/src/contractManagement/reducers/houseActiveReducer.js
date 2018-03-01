import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'


const initState = {
   isLoading: false,
   dynamicLoading: false, // 楼盘列表loading
   projectId: '',
   shopId: '',
   shopList: [], // 商铺列表,
   descriptionValue: '', // 详情描述文本
   myTitleValue: '', // 标题文本
   submitLoading: false, // 保存按钮loading
   hotCheckedArr: [], // 勾选的商铺热卖id组合
   pushCheckedArr: [], // 勾选的商铺加推id组合
   statusArr: [], // 审核状态
   activeType: 'view', 
   isUse: true, // 是否报备，true默认为要报备 
   dynamicStatusArr: [], // 审核详情状态
   recordList: [], // 是否有审核列表
   dynamicData: [],// 楼盘列表
   loadingData: false, // 加载商铺数据loading
}

let map = {};
// 是否暂停或者启用报备
map[actionTypes.IS_USE_REPORT] = (state, action) => {
  let isUse = action.payload.isUse
  let newState = Object.assign({}, state, { isUse: isUse })
  return newState;
}

// 获取id
map[actionTypes.GET_ID] = (state, action) => {
  // console.log(action.payload, '获取id')
  let type = action.payload.type,
      id = action.payload.id, newState
  type === 'project' ?
  newState = Object.assign({}, state, { projectId: id })
  : 
  newState = Object.assign({}, state, { shopId: id })
  return newState;
}

// 获取根据销售状态不同的商铺列表
map[actionTypes.GET_SHOPS_START] = (state, action) => {
  let shopList = [ ...state.shopList ]
  let newState = Object.assign({}, state, { shopList: shopList, isLoading: true })
  return newState;
}
map[actionTypes.GET_SHOPS_FINISH] = (state, action) => {
  let shopList = [ ...state.shopList ]
  // console.log(shopList, '旧的')
  let res =  action.payload
  if (res.code === '0') {
    shopList = res.extension
  }
  let newState = Object.assign({}, state, { shopList: shopList, isLoading: false })
  return newState;
}

// 设置文本框的值
map[actionTypes.DESCRIPTION_SET_VALUE] = (state, action) => {
  // console.log(action.payload)
  return Object.assign({}, state, {descriptionValue: action.payload})
}
// 清空文本框的值 
map[actionTypes.DESCRIPTION_CLEAR_VALUE] = (state, action) => {
  return Object.assign({}, state, {descriptionValue: ''})
}

//设置标题
map[actionTypes.SET_TITLE] = (state, action) => {
  // console.log(action.payload, '设置标题')
  return Object.assign({}, state, {myTitleValue: action.payload})
}
// 清空文本框的值 
map[actionTypes.TITLE_CLEAR_VALUE] = (state, action) => {
  return Object.assign({}, state, {myTitleValue: ''})
}

// 获取勾选的商铺id组合
map[actionTypes.POST_CHECKED_ARR] = (state, action) => {
  let hotCheckedArr = [ ...state.hotCheckedArr ],
  pushCheckedArr = [ ...state.pushCheckedArr ], newState;
  if (action.payload.type === 'hot') {
    newState = Object.assign({}, state, { hotCheckedArr: action.payload.arr })
  } else {
    newState = Object.assign({}, state, { pushCheckedArr: action.payload.arr })
  }
  return newState
}

// 保存
map[actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES_START] = (state, action) => {
  let submitDisabled = { ...submitDisabled }
  return Object.assign({}, state, {submitLoading: true, submitDisabled: submitDisabled})
}

map[actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES_END] = (state, action) => {
  let submitDisabled = { ...submitDisabled }
  let res =  action.payload
  if (res.code === '0') {
    notification.success({
      message: '保存成功',
      duration: 3
    })
  } else {
    notification.error({
      message: '保存失败',
      duration: 3
    })
  }
  return Object.assign({}, state, {submitLoading: false, submitDisabled: submitDisabled})    
}
// 根据楼盘id查询该楼盘下有无审核的板块
map[actionTypes.GET_EXAMINES_STATUS_START] = (state, action) => {
  return Object.assign({}, state)
}

map[actionTypes.GET_EXAMINES_STATUS_END] = (state, action) => {
  let res =  action.payload
  let statusArr
  if (res.code === '0') {
    statusArr = res.extension || []
  } 
  return Object.assign({}, state, {statusArr: statusArr})    
}

map[actionTypes.GET_DYNAMIC_INFO_LIST_END] = (state, action) => {
  let recordList = [];
  let res = action.payload;
  if (res.code === '0') {
    recordList = res.extension || []
  }
  return Object.assign({}, state, {recordList: recordList})
}

// 获取审核详情的状态
map[actionTypes.GET_DYNAMIC_STATUS_START] = (state, action) => {
  let dynamicStatusArr = [], newState
  if (action.payload.isLoading === false) {
    newState = Object.assign({}, state, { dynamicStatusArr: dynamicStatusArr })  
  } else {
    newState = Object.assign({}, state, { dynamicStatusArr: dynamicStatusArr,isLoading:true })  
  }
  return newState    
}
map[actionTypes.GET_DYNAMIC_STATUS_END] = (state, action) => {
  let res =  action.payload
  let dynamicStatusArr = []
  if (res.code === '0') {
    dynamicStatusArr = res.extension || []
  }
  // console.log(dynamicStatusArr, '状态数据数组')
  return Object.assign({}, state, { dynamicStatusArr: dynamicStatusArr,isLoading:false  })    
}




// 获取楼盘列表
map[actionTypes.DYNAMIC_SET_BUILDING_VALUE_START] = (state, action)=>{
  return Object.assign({}, state, {dynamicLoading: true})
}
map[actionTypes.DYNAMIC_SET_BUILDING_VALUE_END] = (state, action) => {
  return Object.assign({}, state, {dynamicLoading: false, dynamicData: action.payload || []})    
}

// 获取商铺列表
map[actionTypes.DYNAMIC_SET_SHOPS_VALUE_CLEAR] = (state, action) => {
  let dynamicData = state.dynamicData.concat([])
  delete dynamicData[action.payload.index].shops
  dynamicData[action.payload.index].isChecked = false
  return Object.assign({}, state, {dynamicData: dynamicData, loadingData: true})
}
map[actionTypes.DYNAMIC_SET_SHOPS_VALUE_END] = (state, action) => {
  let dynamicData = state.dynamicData.concat([])
  dynamicData[action.payload.index].shops = action.payload.shops
  dynamicData[action.payload.index].isChecked = true
  return Object.assign({}, state, {dynamicData: dynamicData,  loadingData: false})
}

export default handleActions(map, initState); 