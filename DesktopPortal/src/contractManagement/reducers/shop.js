import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'


const initState = {
  buildingList: [], // 楼盘列表
  shopsList: [], //商铺列表
  myBuildingList: [], //自己负责的楼盘列表
  shopsInfo: {
    basicInfo: {},   // 基础信息
    supportInfo: {}, // 配套信息
    leaseInfo: {},   // 租约信息
    attachInfo: {},   // 附加信息
    summary: '', // 商铺简介
    id: NewGuid(),
    isDisabled: true,
    buildingId: '',
    buildName: ''
  },
  dynamicShopsInfo: {
    basicInfo: {},   // 基础信息
    supportInfo: {}, // 配套信息
    leaseInfo: {},   // 租约信息
    attachInfo: {},   // 附加信息
  },
  eidtFlagPrice: false, // 是否点击单价编辑按钮默认是展示页面
  operInfo: {
    basicOperType: 'add',
    supportOperType: 'add',
    leaseOperType: 'add',
    attachPicOperType: 'add',
    projectOperType: 'add',
  },
  myAdd: '',
  isLoading: false,
  total: 0,
  buildingNoInfos: [],
  submitLoading: false,
  showGroup: { // list列表框显示状态
    isShow: true,
    isActiveShow: true,
  },
  completeFileList: [],
  deletePicList: [],
  changeList: [], // 切换楼盘list
  indexLoading: false, // 驻场首页loading
  shopDetailProjectInfo: [], // 用于商铺基本信息详情
  basicloading: false,
  supportloading: false,
  summaryLoading: false,
  leaseLoading: false,

};
let map = {};
map[actionTypes.LOADING_START_SHOP_BASIC] = function (state, action) {
  return Object.assign({}, state, { basicloading: true });
}
map[actionTypes.LOADING_END_SHOP_BASIC] = function (state, action) {
  return Object.assign({}, state, { basicloading: false });
}

map[actionTypes.LOADING_START_SHOP_LEASE] = function (state, action) {
  return Object.assign({}, state, { leaseLoading: true });
}
map[actionTypes.LOADING_END_SHOP_LEASE] = function (state, action) {
  return Object.assign({}, state, { leaseLoading: false });
}

map[actionTypes.LOADING_START_SHOP_SUPPORT] = function (state, action) {
  return Object.assign({}, state, { supportloading: true });
}
map[actionTypes.LOADING_END_SHOP_SUPORT] = function (state, action) {
  return Object.assign({}, state, { supportloading: false });
}

map[actionTypes.LOADING_START_SHOP_SUMMARY] = function (state, action) {
  return Object.assign({}, state, { summaryLoading: true });
}
map[actionTypes.LOADING_END_SHOP_SUMMARY] = function (state, action) {
  return Object.assign({}, state, { summaryLoading: false });
}
// 存上传照片
map[actionTypes.SAVE_COMPLETE_FILE_LIST] = (state, action) => {
  let completeFileList = []
  completeFileList = action.payload.completeFileList
  let newState = Object.assign({}, state, { completeFileList: completeFileList });
  return newState;
}
// 存删除的照片数组
map[actionTypes.SAVE_DELETE_PIC_LIST] = (state, action) => {
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

// 更改小列表框的显示状态
// isShow, isActiveShow ==> 0 两者都为true
// isShow, isActiveShow ==> 1 isShow为true ， isActiveShow为false
// isShow, isActiveShow ==> 2 isShow为false ， isActiveShow为true
map[actionTypes.CHANGE_SHOW_GROUP] = (state, action) => {
  let showGroup = { ...state.showGroup },
    type = action.payload.type
  // console.log(showGroup, '旧值')
  switch (type) {
    case 0:
      showGroup = {
        isShow: true,
        isActiveShow: true,
      }; break;
    case 1:
      showGroup = {
        isShow: true,
        isActiveShow: !showGroup.isActiveShow,
      }; break;
    default:
      showGroup = {
        isShow: !showGroup.isShow,
        isActiveShow: true,
      }
  }
  let newState = Object.assign({}, state, { showGroup: showGroup });
  // console.log(newState.showGroup, '新增')
  return newState;
}

// 获取楼盘列表
map[actionTypes.BUILDING_LIST_START] = (state, action) => {
  let newState = Object.assign({}, state);
  return newState;
}
map[actionTypes.BUILDING_LIST_FINISH] = (state, action) => {
  let res = action.payload;
  let buildings;
  if (res.code === '0') {
    buildings = res.extension
  }
  let newState = Object.assign({}, state, { buildingList: buildings })
  return newState;
}

// 获取自己负责的楼盘列表
map[actionTypes.BUILDING_MYLIST_START] = (state, action) => {
  let newState = Object.assign({}, state, { isLoading: true, indexLoading: true });
  return newState;
}


// 获取商铺列表
map[actionTypes.SHOPS_LIST_START] = (state, action) => {
  let newState = Object.assign({}, state);
  return newState;
}
map[actionTypes.SHOPS_LIST_FINISH] = (state, action) => {
  let myBuildingList = [...state.myBuildingList];
  let res = action.payload;
  let id = res.buildingId
  let shopsList;
  if (res.code === '0') {
    shopsList = res.extension
  }
  myBuildingList.map((building, i) => {
    if (id === building.id) {
      building.children = shopsList || [];
      building.shopTotal = res.totalCount;
    }
  })
  let newState = Object.assign({}, state, { myBuildingList: myBuildingList })
  return newState;
}

// 基本信息
map[actionTypes.SHOP_BASIC_SAVE_START] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo }
  let operInfo = Object.assign({}, state, { shopsInfo: shopsInfo });
  return operInfo;
}

map[actionTypes.SHOP_BASIC_SAVE_FINISH] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let res = action.payload.data;
  if (res.code === '0') {
    shopsInfo.basicInfo = res.extension
    shopsInfo.buildingId = res.extension.buildingId
    notification.success({
      message: '保存成功',
      duration: 3
    });
    shopsInfo.isDisabled = false
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo, basicloading: false, });
    return newState;
  }
  else if (res.code === '102') {
    notification.error({
      message: '该商铺已存在',
      duration: 3
    });
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'add' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo, basicloading: false, });
    return newState;
  }

}
// 基本信息编辑
map[actionTypes.SHOP_BASIC_EDIT] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'edit' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}
// 基本信息查看
map[actionTypes.SHOP_BASIC_VIEW] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };

  let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}


// 租约信息

map[actionTypes.SHOP_LEASE_SAVE_START] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state, { shopsInfo: shopsInfo,  });
  return operInfo;
}

map[actionTypes.SHOP_LEASE_SAVE_FINISH] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let res = action.payload.data;
  if (res.code === '0') {
    shopsInfo.leaseInfo = res.extension
    notification.success({
      message: '保存成功',
      duration: 3
    });
    let operInfo = Object.assign({}, state.operInfo, { leaseOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo, leaseLoading: false, });
    return newState;
  }

}
// 租约信息编辑
map[actionTypes.SHOP_LEASE_EDIT] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { leaseOperType: 'edit' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}
// 租约信息查看
map[actionTypes.SHOP_LEASE_VIEW] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { leaseOperType: 'view' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}


// 配套设施

map[actionTypes.SHOP_SUPPORT_SAVE_START] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state, { shopsInfo: shopsInfo });
  return operInfo;
}

map[actionTypes.SHOP_SUPPORT_SAVE_FINISH] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let res = action.payload.data;
  if (res.code === '0') {
    shopsInfo.supportInfo = res.extension
    notification.success({
      message: '保存成功',
      duration: 3
    });
    let operInfo = Object.assign({}, state.operInfo, { supportOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo , supportloading: false});
    return newState;
  }
}
//配套设施信息编辑
map[actionTypes.SHOP_SUPPORT_EDIT] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { supportOperType: 'edit' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}
// 配套设施信息查看
map[actionTypes.SHOP_SUPPORT_VIEW] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { supportOperType: 'view' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}

//图片信息编辑
map[actionTypes.SHOP_PIC_EDIT] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state.operInfo, { attachPicOperType: 'edit' });
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
  return newState;
}
// 图片信息查看
// map[actionTypes.SHOP_PIC_VIEW] = (state, action) => {
//   let shopsInfo = { ...state.shopsInfo };
//   let operInfo = Object.assign({}, state.operInfo, { attachPicOperType: 'view' });
//   let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
//   return newState;
// }
map[actionTypes.SHOP_PIC_VIEW] = (state, action) => {
  const type = action.payload.type // add' 新增， 'delete'删除， 'cancel' 取消
  let shopsInfo = { ...state.shopsInfo }
  let dynamicShopsInfo = { ...state.dynamicShopsInfo }
  let attachInfo, oldFileList, nowFileList
 
  if (action.payload.dynamic === 'dynamic') {
    attachInfo = { ...state.dynamicShopsInfo.attachInfo }
    oldFileList = [...state.dynamicShopsInfo.attachInfo.fileList]
    nowFileList = action.payload.filelist
  } else {
    attachInfo = { ...state.shopsInfo.attachInfo }
    oldFileList = [...state.shopsInfo.attachInfo.fileList]
    nowFileList = action.payload.filelist
  }
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
  if (action.payload.dynamic !== 'dynamic') {
    attachInfo = Object.assign({}, state.shopsInfo.attachInfo, { fileList: oldFileList })
    shopsInfo = Object.assign({}, state.shopsInfo, { attachInfo: attachInfo })
  } else {
    attachInfo = Object.assign({}, state.dynamicShopsInfo.attachInfo, { fileList: oldFileList })
    dynamicShopsInfo = Object.assign({}, state.dynamicShopsInfo, { attachInfo: attachInfo })
  }
  let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo, dynamicShopsInfo: dynamicShopsInfo });
  return newState;
}


// 点击增加楼盘跳转页面
map[actionTypes.GOTO_BUILD_PAGE] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: 'menu_building_dish' });
  return newState;
}

// 点击增加商铺跳转页面
map[actionTypes.GOTO_SHOP_PAGE] = (state, action) => {
  console.log(action.payload, '点击')
  let res = action.payload
  let operInfo = {
    basicOperType: 'add',
    supportOperType: 'add',
    leaseOperType: 'add',
    attachPicOperType: 'add',
    projectOperType: 'add',
  }
  let shopsInfo = {
    basicInfo: {},   // 基础信息
    supportInfo: {}, // 配套信息
    leaseInfo: {},   // 租约信息
    attachInfo: {},   // 附加信息
    summary: '', // 商铺简介
    id: NewGuid(),
    isDisabled: true,
    buildingId: res.id,
    buildName: res.basicInfo.name
  }
  let buildingNoInfos = action.payload.buildingNoInfos
  let newState = Object.assign({}, state, { myAdd: 'menu_shops', operInfo: operInfo, shopsInfo: shopsInfo, buildingNoInfos: buildingNoInfos });
  return newState;
}

map[actionTypes.CHANGE_MYADD] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: '' });
  return newState;
}

// 点击进入这个商铺
map[actionTypes.GOTO_THIS_SHOP_START] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: '' });
  return newState;
}
map[actionTypes.GOTO_THIS_SHOP_FINISH] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = { ...state.operInfo };
  // let dynamicShopsInfo = { ...state.dynamicShopsInfo };
  let res = action.payload.data;
  console.log(res, '???')
  if (res.code === '0') {
    shopsInfo = res.extension
    shopsInfo.supportInfo = shopsInfo.facilitiesInfo
    shopsInfo.leaseInfo.dateRange = [];
    shopsInfo.leaseInfo.dateRange.push(shopsInfo.leaseInfo.startDate ? moment(shopsInfo.leaseInfo.startDate) : null);
    shopsInfo.leaseInfo.dateRange.push(shopsInfo.leaseInfo.endDate ? moment(shopsInfo.leaseInfo.endDate) : null);
    shopsInfo.attachInfo = { fileList: shopsInfo.fileList, attachmentList: shopsInfo.attachmentList }
    if (res.extension.examineStatus === 8 || res.extension.examineStatus === 1) {
      operInfo = {
        basicOperType: 'view',
        supportOperType: 'view',
        leaseOperType: 'view',
        attachPicOperType: 'view',
        projectOperType: 'view',
      }
    } else {
      operInfo = {
        basicOperType: 'edit',
        supportOperType: 'edit',
        leaseOperType: 'edit',
        attachPicOperType: 'edit',
        projectOperType: 'edit',
      }
    }
  }
  let newState;
  if (res.dynamic === 'dynamic') { // 动态房源不跳转到新增房源的页面去
    // console.log(1)
    newState = Object.assign({}, state, { shopsInfo: shopsInfo, operInfo: operInfo });
  } else {
    let buildingNoInfos = res.build.buildingNoInfos
    newState = Object.assign({}, state, { shopsInfo: shopsInfo, myAdd: 'menu_shops', operInfo: operInfo, buildingNoInfos: buildingNoInfos  });
  }
  return newState;
}

map[actionTypes.GOTO_CHANGE_MYADD] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: 'menu_building_dish' });
  return newState;
}

// 删除商铺
map[actionTypes.DELETE_START] = (state, action) => {
  let shopsList = state.shopsList
  let newState = Object.assign({}, state, { shopsList: shopsList });
  return newState;
}
map[actionTypes.DELETE_FINISH] = (state, action) => {
  let myBuildingList = [...state.myBuildingList]
  let res = action.payload.data
  let body = res.body
  if (res.code === '0') {
    notification.success({
      message: '删除成功',
      duration: 3
    })
  } else {
    notification.error({
      message: '删除失败',
      duration: 3
    })
  }
  let newState = Object.assign({}, state, { myBuildingList: myBuildingList });
  return newState;
}

// 提交商铺信息完成
map[actionTypes.SHOPS_INFO_SUBMIT_START] = function (state, action) {
  let newState = Object.assign({}, state, { submitLoading: true });
  return newState;
}
map[actionTypes.SHOPS_INFO_SUBMIT_FINISH] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo }
  let res = action.payload.data
  notification[res.code === "0" ? 'success' : 'error']({
    message: res.code === "0" ? '提交成功' : '提交失败',
    duration: 3
  })
  return Object.assign({}, state, { shopsInfo: shopsInfo, myAdd: 'menu_index', submitLoading: false })
}

// 搜索我得楼盘
map[actionTypes.MYSEARCH_FINISH] = (state, action) => {
    let newState;
    let changeList = [];
    if (Array.isArray(action.payload)) {
      changeList = action.payload;
    }
    newState = Object.assign({}, state, { isLoading: false, changeList: changeList })
    return newState;
}


// 房源动态列表

// 点击商铺跳转详情页面
map[actionTypes.GOTO_SHOP_DETAIL_PAGE_START] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: '' });
  return newState;
}
map[actionTypes.GOTO_SHOP_DETAIL_PAGE_FINISH] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: 'activeShop' });
  return newState;
}

// 点击楼盘跳转详情页面
map[actionTypes.GOTO_PROJECT_DETAIL_PAGEE_START] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: '' });
  return newState;
}
map[actionTypes.GOTO_PROJECT_DETAIL_PAGE_FINISH] = (state, action) => {
  let newState = Object.assign({}, state, { myAdd: 'activeProject' });
  return newState;
}


// 价格编辑
map[actionTypes.PRICE_EDIT] = (state, action) => {
  let newState = Object.assign({}, state, { eidtFlagPrice: true })
  return newState;
}
map[actionTypes.PRICE_VIEW] = (state, action) => {
  let dynamicShopsInfo = { ...state.dynamicShopsInfo }
  dynamicShopsInfo.basicInfo = action.payload.body
  let newState = Object.assign({}, state, { eidtFlagPrice: false, dynamicShopsInfo: dynamicShopsInfo })
  return newState;
}

// 根据房源动态的楼盘
map[actionTypes.GET_SHOP_DYNAMIC_INFO_DETAIL_END] = function (state, action) {
  let res = action.payload
  let dynamicShopsInfo = { ...state.dynamicShopsInfo }
  if (res.code === '0') {
    switch (res.extension.contentType) {
      case 'Image':
        let attachInfo = { ...state.dynamicShopsInfo.attachInfo }
        attachInfo.fileList = JSON.parse(res.extension.updateContent); break;
      case 'Price':
        dynamicShopsInfo.basicInfo = JSON.parse(res.extension.updateContent); break;
      default: // 附件
        dynamicShopsInfo.ruleInfo = JSON.parse(res.extension.updateContent);
    }
  }
  let newState = Object.assign({}, state, { dynamicShopsInfo: dynamicShopsInfo });
  return newState;
}


// 获取楼盘列表
map[actionTypes.GET_ADD_BUILDING_START] = (state, action) => {
  return Object.assign({}, state, { isLoading: true })
}
map[actionTypes.GET_ADD_BUILDING_END] = (state, action) => {
  return Object.assign({}, state, { isLoading: false, myBuildingList: action.payload || [] })
}

// 获取楼盘列表
map[actionTypes.BUILDING_MYLIST_FINISH] = (state, action) => {
  let shopDetailProjectInfo = [];
  let newState = Object.assign({}, state, { shopDetailProjectInfo: action.payload })
  return newState;
}

// 获取商铺列表
map[actionTypes.GET_ADD_SHOP_LIST_START] = (state, action) => {
  let myBuildingList = state.myBuildingList.concat([])
  delete myBuildingList[action.payload.index].shops
  myBuildingList[action.payload.index].isChecked = false
  return Object.assign({}, state, { myBuildingList: myBuildingList })
}
map[actionTypes.GET_ADD_SHOP_LIST_END] = (state, action) => {
  let myBuildingList = state.myBuildingList.concat([])
  myBuildingList[action.payload.index].shops = action.payload.shops
  myBuildingList[action.payload.index].isChecked = true
  return Object.assign({}, state, { myBuildingList: myBuildingList })
}


// 删除楼盘
map[actionTypes.DELETE_BUILDING_START] = (state, action) => {
  let myBuildingList = []
  let newState = Object.assign({}, state, { myBuildingList: myBuildingList });
  return newState;
}
map[actionTypes.DELETE_BUILDING_FINISH] = (state, action) => {
  let myBuildingList = [...state.myBuildingList]
  console.log(myBuildingList, '原始得数据')
  let res = action.payload.data
  console.log(res, '删除后得数据')
  let body = res.body
  if (res.code === '0') {
    notification.success({
      message: '删除成功',
      duration: 3
    })
  } else {
    notification.error({
      message: '删除失败',
      duration: 3
    })
  }
  let newState = Object.assign({}, state, { myBuildingList: myBuildingList });
  return newState;
}

// 获取切换楼盘列表
map[actionTypes.GET_CHANGE_BUILDING_LIST_START] = (state, action) => {
  return Object.assign({}, state, { indexLoading: true })
}
map[actionTypes.GET_CHANGE_BUILDING_LIST_END] = (state, action) => {
  let changeList = [];
  if (Array.isArray(action.payload)) {
    changeList = action.payload;
  }
  return Object.assign({}, state, { changeList: changeList, indexLoading: false })
}


//商铺简介相关
map[actionTypes.VIEW_SHOP_SUMMARY_INFO] = function (state, action) {
    let shopsInfo = { ...state.shopsInfo };
    let operInfo = Object.assign({}, state.operInfo, { projectOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
    return newState;
}
map[actionTypes.EDIT_SHOP_SUMMARY_INFO] = function (state, action) {
    let shopsInfo = { ...state.shopsInfo };
    let operInfo = Object.assign({}, state.operInfo, { projectOperType: 'edit' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo });
    return newState;
}
// 保存
map[actionTypes.SAVE_SHOP_SUMMARY_INFO_START] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let operInfo = Object.assign({}, state, { shopsInfo: shopsInfo });
  return operInfo;
}

map[actionTypes.SAVE_SHOP_SUMMARY_INFO_END] = (state, action) => {
  let shopsInfo = { ...state.shopsInfo };
  let res = action.payload.data;
  if (res.code === '0') {
    shopsInfo.summary = res.extension
    notification.success({
      message: '保存成功',
      duration: 3
    });
    let operInfo = Object.assign({}, state.operInfo, { projectOperType: 'view' });
    let newState = Object.assign({}, state, { operInfo: operInfo, shopsInfo: shopsInfo,  summaryLoading: false,});
    return newState;
  }

}

export default handleActions(map, initState); 