import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    isLoading: false, // 全页loading
    editTimeInfo: { // 小定日期信息
      show: false,
      operating: false,
      shopId: '',
      saleStatus: '',
      current: '', // 当前类别
      buildingId: '', //楼盘id
    },
    shopInfoList: [], // 商铺列表
    kanBanList: [], // 看板商铺列表
    listLoading: false, // 列表loading
    customerDeal: [], // 成交信息
    seriesData: [], // 销售统计
    customerInfo: [],// 报备状态为【3】得报备信息
};
let map = {};

// 点击小定按钮设定小定日期
map[actionTypes.SHOW_TIME_DIALOG_MODAL] = (state, action) => {
  let editTimeInfo = { ...state.editTimeInfo }
  editTimeInfo.show = true;
  editTimeInfo.shopId = action.payload.id;
  editTimeInfo.saleStatus = action.payload.saleStatus;
  editTimeInfo.current = action.payload.current;
  editTimeInfo.buildingId = action.payload.buildingId;
  let newState = Object.assign({}, state, { editTimeInfo: editTimeInfo })
  return newState;
}
// 关闭时间弹框
map[actionTypes.CANCEL_TIME_DIALOG_MODAL] = (state, action) => { 
  let editTimeInfo = { ...state.editTimeInfo }
  editTimeInfo.show = false;
  let newState = Object.assign({}, state, { editTimeInfo: editTimeInfo })
  return newState;
}

// 点击保存小定时间
map[actionTypes.SAVE_TIME_START] = (state, action) => {
  let editTimeInfo = { ...state.editTimeInfo }
  editTimeInfo.operating = true;
  let newState = Object.assign({}, state, { editTimeInfo: editTimeInfo })
  return newState;
}
map[actionTypes.SAVE_TIME_FINISH] = (state, action) => {
  let editTimeInfo = { ...state.editTimeInfo }
  let res = action.payload
  if (res.code === '0') {
    editTimeInfo.operating = false;
    editTimeInfo.show = false;
    notification.success({
      message: '成功',
      duration: 3
    });
  } else {
    notification.error({
      message: '失败',
      duration: 3
    });
  }
  let newState = Object.assign({}, state, { editTimeInfo: editTimeInfo })
  return newState;
}

// 获取商铺销售列表
map[actionTypes.GET_SHOPS_SALE_STATUS_START] = (state, action) => {
  let newState
  action.payload ? // 根据是否是点击的销售类别切换数据，
  newState = Object.assign({}, state, { listLoading: true })
  : 
  newState = Object.assign({}, state, { isLoading: true })
  return newState;
}
map[actionTypes.GET_SHOPS_SALE_STATUS_FINISH] = (state, action) => {
  let shopInfoList = [...state.shopInfoList]
  let kanBanList = [...state.kanBanList]
  let res = action.payload
  if (res.code === '0') {
    res.who ? kanBanList = res.extension : shopInfoList = res.extension
  }
  let newState = Object.assign({}, state, { isLoading: false, shopInfoList: shopInfoList, listLoading: false, kanBanList: kanBanList})
  return newState;
}

// 点击查看成交信息
map[actionTypes.GET_CUSTOMER_DEAL_START] = (state, action) => {
  let customerDeal = []
  let newState = Object.assign({}, state, { customerDeal: customerDeal })
  return newState;
}
map[actionTypes.GET_CUSTOMER_DEAL_FINISH] = (state, action) => {
  let customerDeal = []
  let res = action.payload
  if (res.code === '0') {
    customerDeal = res.extension || []
  }
  let newState = Object.assign({}, state, { customerDeal: customerDeal })
  return newState;
}

// 获取楼盘下的商铺销售状态统计
map[actionTypes.GET_SALES_TATISTICS_START] = (state, action) => {
  let seriesData = []
  let newState = Object.assign({}, state, { seriesData: seriesData })
  return newState;
}
map[actionTypes.GET_SALES_TATISTICS_FINISH] = (state, action) => {
  let seriesData = []
  let res = action.payload
  if (res.code === '0') {
    seriesData = res.extension || []
  }
  let newState = Object.assign({}, state, { seriesData: seriesData })
  return newState;
}



// 获取成交客户等等信息
map[actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO_START] = (state, action) => {
  let customerInfo = []
  let newState = Object.assign({}, state, { customerInfo: customerInfo })
  return newState;
}
map[actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO_END] = (state, action) => {
  let customerInfo = []
  let res = action.payload
  if (res.code === '0') {
    customerInfo = res.extension || []
  }
  let newState = Object.assign({}, state, { customerInfo: customerInfo })
  return newState;
}




export default handleActions(map, initState); 