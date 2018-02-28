// 驻场首页reducer

import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'


const initState = {
  customerInfo: [],
  nowInfo: {}, // 当前楼盘数据
  nowInfoIndex: 0, // 当前楼盘index
  searchList: [],
  indexLoading: false,
  myBuildingList: [], //自己负责的楼盘列表
  infoList: [], // 报备信息
  buildingInfo: {}, // 楼盘信息
  statusCounts: {
    submitCount: 0, // 待确认数量
    todayReportCount: 0, // 已确认数量
    submintIds: [], // 待确认ID
    transactionsResponses: [], // 待报备ID
  },
  editDealInfo: { // 成交信息
    show: false,
    title: '',
    operating: false,
    entity: {},
  },
  batchReportInfo: { // 批量报备
    show: false,
    current: '',
    buildingId: '',
  },
  showTemplateInfo: { // 模板显示
    show: false,
    isChecked: false, // 是否勾选不再提示
  },
  vaiPhoneInfo: [], // 显示全号码得数据
  reportCustomerDeal: [], // 报备成交信息
};
let map = {};
// 通过是否显示全号码
map[actionTypes.SEARCH_VALPHONE_START] = (state, action) => {
  let vaiPhoneInfo = []
  let newState = Object.assign({}, state, { vaiPhoneInfo: vaiPhoneInfo });
  return newState;
}
map[actionTypes.SEARCH_VALPHONE_END] = (state, action) => {
  let res = action.payload;
  let vaiPhoneInfo = []
  if (res.code === '0') {
    vaiPhoneInfo = res.extension
  }
  let newState = Object.assign({}, state, { vaiPhoneInfo: vaiPhoneInfo })
  return newState;
}

// // 获取自己负责的楼盘列表
// map[actionTypes.MYLIST_START] = (state, action) => {
//   let newState = Object.assign({}, state, { indexLoading: true });
//   return newState;
// }
// map[actionTypes.MYLIST_FINISH] = (state, action) => {
//   let res = action.payload;
//   let myBuildingList;
//   if (res.code === '0') {
//     myBuildingList = res.extension
//   }
//   let newState = Object.assign({}, state, { myBuildingList: myBuildingList })
//   return newState;
// }

// 切换楼盘
map[actionTypes.CHANGE_NOWLIST_INFO] = (state, action) => {
  let nowInfo = { ...state.nowInfo }
  nowInfo = action.payload
  let newState = Object.assign({}, state, { nowInfo: nowInfo });
  return newState;
}
// 切换楼盘当前index
map[actionTypes.GET_THIS_PROJECT_INDEX] = (state, action) => {
  let newState = Object.assign({}, state, { nowInfoIndex: action.payload.index });
  return newState;
}
// 搜索我得楼盘
map[actionTypes.SEARCH_FINISH] = (state, action) => {
  let searchList = [...state.searchList]
  let res = action.payload
  if (res.code === '0') {
    searchList = res.extension
  }
  let newState = Object.assign({}, state, { searchList: searchList });
  return newState;
}

// 根据楼盘id查询出该楼盘下得所有报备信息
map[actionTypes.CUSTOMOR_TRANSACTIONS_FINISH] = (state, action) => {
  let infoList = [...state.infoList],
    res = action.payload, status
  if (res.code === '0') {
    // if (res.entity.status) {
    //   status = res.entity.status
    //   if (status[0] === 1) { // 提交报备处只显示今日的报备数据
    //     let newArr = [] 
    //     console.log(res.extension, 'wo yao ankaahidhhduheud')
    //     res.extension.forEach((v, i) => {
    //       let markTime = v.expectedBeltTime
    //       if (moment(markTime).format('YYYY-MM-DD') === moment().format('YYYY-MM-DD')) {
    //         newArr.push(v)
    //         infoList = newArr
    //         infoList.totalCount = newArr.length
    //       } else {
    //         infoList = []
    //         infoList.totalCount = 0
    //       }
    //     })
    //   } else {
    //     console.log(1)
    //     infoList = res.extension
    //     infoList.totalCount = res.totalCount
    //   }
    // } else {
    // console.log(222222222)
    infoList = res.extension
    infoList.totalCount = res.totalCount
    //}
  }
  let newState = Object.assign({}, state, { infoList: infoList, indexLoading: false });
  // console.log(newState.infoList, '>>>>>>>>')
  return newState;
}
// 根据楼盘id查询确认报备和向开放商报备状态的数据总数
map[actionTypes.COUNT_FINISH] = (state, action) => {
  let statusCounts = { ...state.statusCounts }
  let res = action.payload.data
  if (res.code === '0') {
    statusCounts = res.extension
  }
  let newState = Object.assign({}, state, { statusCounts: statusCounts });
  return newState;
}

//  根据楼盘id查询出该楼盘的基本信息
map[actionTypes.GET_THIS_BUILDING_START] = (state, action) => {
  let buildingInfo = { ...state.buildingInfo }
  let newState = Object.assign({}, state, { buildingInfo: buildingInfo });
  return newState;
}
map[actionTypes.GET_THIS_BUILDING_FINISH] = (state, action) => {
  let buildingInfo = { ...state.buildingInfo }
  let res = action.payload
  if (res.data.code === '0') {
    buildingInfo = res.data.extension
  }
  let newState = Object.assign({}, state, { buildingInfo: buildingInfo });
  return newState;
}

// 批量确认报备
map[actionTypes.COMFIRM_FINISH] = (state, action) => {
  let res = action.payload
  let newState
  // console.log(res, '??')
  if (res.code === '0') {
    notification.success({
      message: '已确认成功',
      duration: 3
    });
    newState = Object.assign({}, state);
  } else {
    notification.error({
      message: '确认失败',
      duration: 3
    });
  }
  // console.log(newState, 999)
  return newState;
}

// 批量向开发商报备
map[actionTypes.REPORT_FINISH] = (state, action) => {
  let res = action.payload
  let body = res.body,
    newState
  // console.log(res, '向开发商报备??')
  if (res.code === '0') {
    notification.success({
      message: '已向开发商报备成功',
      duration: 3
    });
    newState = Object.assign({}, state);
  } else {
    notification.error({
      message: '向开发商报备失败',
      duration: 3
    });
  }
  // console.log(newState, 999)
  return newState;
}

// 点击批量向开发商报备显示弹框
map[actionTypes.SHOW_BATCH_REPORT_MODAL] = (state, action) => {
  let data = action.payload
  let batchReportInfo = { ...state.batchReportInfo }
  batchReportInfo.show = true;
  batchReportInfo.current = data.current;
  batchReportInfo.buildingId = data.buildingId;
  let newState = Object.assign({}, state, { batchReportInfo: batchReportInfo })
  return newState;
}
// 关闭模态框
map[actionTypes.HIDE_BATCH_REPORT_MODAL] = (state, action) => {
  let batchReportInfo = { ...state.batchReportInfo }
  batchReportInfo.show = false;
  let newState = Object.assign({}, state, { batchReportInfo: batchReportInfo })
  return newState;
}

// 点击复制，显示模板弹框
map[actionTypes.SHOW_TEMPLATE_MODAL] = (state, action) => {
  console.log(action, '点击复制，显示模板弹框')
  let showTemplateInfo = { ...state.showTemplateInfo }
  showTemplateInfo.show = true;
  let newState = Object.assign({}, state, { showTemplateInfo: showTemplateInfo })
  return newState;
}
// 关闭模态框
map[actionTypes.HIDE_TEMPLATE_MODAL] = (state, action) => {
  let showTemplateInfo = { ...state.showTemplateInfo }
  showTemplateInfo.show = false;
  let newState = Object.assign({}, state, { showTemplateInfo: showTemplateInfo })
  return newState;
}
// 勾选不再提示
map[actionTypes.CHANGE_CHECKED] = (state, action) => {
  let showTemplateInfo = { ...state.showTemplateInfo }
  showTemplateInfo.isChecked = true;
  let newState = Object.assign({}, state, { showTemplateInfo: showTemplateInfo })
  return newState;
}


// 确认带看
map[actionTypes.LOOK_FINISH] = (state, action) => {
  let res = action.payload
  let body = res.body,
    spanStatus,
    newState
  console.log(res, '确认带看reducer')
  if (res.code === '0') {
    notification.success({
      message: '确认成功',
      duration: 3
    });
    newState = Object.assign({}, state);
  } else {
    notification.error({
      message: '确认失败',
      duration: 3
    });
  }
  console.log(newState, '确认带看新值')
  return newState;
}

// 点击确认成交弹出录入成交信息窗口
map[actionTypes.SHOW_EDIT_MODAL] = (state, action) => {
  console.log(action, '哈哈哈')
  let editDealInfo = { ...state.editDealInfo }
  let data = action.payload
  if (data.record) {
    editDealInfo.entity = data.record
  } else {
    editDealInfo.entity = data
  }
  editDealInfo.show = true;
  editDealInfo.title = '录入成交信息';
  let newState = Object.assign({}, state, { editDealInfo: editDealInfo })
  console.log(newState, '确认成交新值')
  return newState;
}
// 点击取消关闭录入成交信息窗口
map[actionTypes.HIDE_EDIT_MODAL] = (state, action) => {
  let editDealInfo = { ...state.editDealInfo }
  editDealInfo.show = false;
  let newState = Object.assign({}, state, { editDealInfo: editDealInfo })
  return newState;
}

// 确认成交
map[actionTypes.CUSTOMER_DEAL_ASYNC_START] = (state, action) => {
  let editDealInfo = { ...state.editDealInfo }
  editDealInfo.operating = true
  return Object.assign({}, state, { editDealInfo: editDealInfo });
}
map[actionTypes.CUSTOMER_DEAL_ASYNC_FINISH] = (state, action) => {
  console.log(action, 'CUSTOMER_DEAL_ASYNC_FINISH')
  let editDealInfo = { ...state.editDealInfo }
  editDealInfo.operating = false
  editDealInfo.show = false
  let res = action.payload
  let body = res.body
  console.log(res, '确认成交reducer')
  if (res.code === '0') {
    notification.success({
      message: '确认成功',
      duration: 3
    });
  } else {
    notification.error({
      message: res.message,
      duration: 3
    });
  }
  return Object.assign({}, state, { editDealInfo: editDealInfo });
}


// 点击查看成交信息
map[actionTypes.GET_REPORT_CUSTOMER_DEAL_START] = (state, action) => {
  let reportCustomerDeal = []
  let newState = Object.assign({}, state, { reportCustomerDeal: reportCustomerDeal })
  return newState;
}
map[actionTypes.GET_REPORT_CUSTOMER_DEAL_FINISH] = (state, action) => {
  let reportCustomerDeal = []
  let res = action.payload
  if (res.code === '0') {
    reportCustomerDeal = res.extension || []
  }
  let newState = Object.assign({}, state, { reportCustomerDeal: reportCustomerDeal })
  return newState;
}

export default handleActions(map, initState); 