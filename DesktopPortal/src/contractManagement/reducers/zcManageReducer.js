import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    isLoading: false, 
    infoLoading: false, 
    editManagerInfo: {
      show: false,
      operating: false
    },
    dataSource: [], // 驻场列表
    userArr: [], // 驻场用户
    checkedArr: [], // 勾选的驻场数组
    currentInfo: {}, // 我点击的当前这个列表的信息
    examinesArr: [], // 审核列表
    zcData: [], 
    examineStatus: ''
};
let map = {};


// 点击编辑弹出修改驻场框
map[actionTypes.OPEN_MODAL] = (state, action) => { 
  console.log(action.payload, '打开模态框')
  let currentInfo = { ...state.currentInfo }
  let editManagerInfo = { ...state.editManagerInfo }
  editManagerInfo.show = true;
  currentInfo = action.payload
  let newState = Object.assign({}, state, { editManagerInfo: editManagerInfo, currentInfo: currentInfo })
  return newState;
}
// 关闭
map[actionTypes.CLOSE_MODAL] = (state, action) => { 
  let editManagerInfo = { ...state.editManagerInfo }
  editManagerInfo.show = false;
  let newState = Object.assign({}, state, { editManagerInfo: editManagerInfo })
  return newState;
}

// 获取列表
map[actionTypes.GET_BUILDING_SITE_START] = (state, action) => { 
  let newState = Object.assign({}, state, { isLoading: true })
  return newState;
}
map[actionTypes.GET_BUILDING_SITE_END] = (state, action) => { 
  let res = action.payload, newState,
      dataSource = state.dataSource.slice()
  if (res.code === '0') {
    dataSource = res.extension
    newState = Object.assign({}, state, { isLoading: false, dataSource: dataSource })
  } else {
    notification.error({
      message: '获取列表失败',
      duration: 3
    })
  }
  return newState;
}

// 获取驻场用户
map[actionTypes.GET_SITEUSER_LIST_START] = (state, action) => { 
  // let checkedArr = [...state.checkedArr],userArr = [...state.userArr]
  let newState = Object.assign({}, state, { infoLoading: true })
  return newState;
}
map[actionTypes.GET_SITEUSER_LIST_END] = (state, action) => { 
  let zcData = []
  let newState, checkedIds, res = action.payload
  if (res.code === '0') {
    zcData = action.payload.extension
    newState = Object.assign({}, state, { infoLoading: false,  zcData: zcData })
  }
  return newState;
}

// 获取审核列表
map[actionTypes.GET_EXAMINESLIST_LIST_START] = (state, action) => { 
  let examinesArr = []
  let newState = Object.assign({}, state, { examinesArr: examinesArr })
  return newState;
}
map[actionTypes.GET_EXAMINESLIST_LIST_END] = (state, action) => { 
  let examinesArr = []
  let type = action.payload.type  // 1 表示为未审核的  2 表示审核
  let newState, checkedIds, checkedArr = [], userArr = [], currentInfo = { ...state.currentInfo }
  let zcData = action.payload.zcData.slice()
  let examineStatus
  if (type === 1) {
    examineStatus = ''
    checkedIds = [currentInfo.residentUser1, currentInfo.residentUser2, currentInfo.residentUser3, currentInfo.residentUser4]
    if(zcData.length > 0) {
      userArr = zcData.map(v => {
      if (checkedIds.indexOf(v.id) !== -1) {
        v.isChecked = true
        checkedArr.push({ id: v.id,name: v.trueName })
      }
      return v
      })
    }
    newState = Object.assign({}, state, { checkedArr: checkedArr, userArr: userArr, examineStatus: '' })
  } else {
    examineStatus = action.payload.examineStatus
    let ids = JSON.parse(action.payload.data)
    checkedIds = [ids.residentUser1, ids.residentUser2, ids.residentUser3, ids.residentUser4]
    userArr = zcData.map(v => {
      if (checkedIds.indexOf(v.id) !== -1) {
        v.isChecked = true
        checkedArr.push({ id: v.id,name: v.trueName })
      }
      return v
    })
    newState = Object.assign({}, state, { checkedArr: checkedArr, userArr: userArr, examineStatus: examineStatus})
  }
 
  return newState;
}


map[actionTypes.CHANGE_ARR] = (state, action) => {
  let checkedArr = [],
      userArr =  []
  let newState = Object.assign({}, state,{checkedArr: action.payload.checkedArr, userArr: action.payload.arr})
  return newState;
}




// 保存
map[actionTypes.SAVE_PERSON_START] = (state, action) => { 
  let editManagerInfo = { ...state.editManagerInfo }
  editManagerInfo.operating = true;
  let newState = Object.assign({}, state, { editManagerInfo: editManagerInfo })
  return newState;
}
map[actionTypes.SAVE_PERSON_FINISH] = (state, action) => { 
  let editManagerInfo = { ...state.editManagerInfo }
  editManagerInfo.operating = false;
  editManagerInfo.show = false;
  let newState = Object.assign({}, state, { editManagerInfo: editManagerInfo })
  return newState;
}






export default handleActions(map, initState); 