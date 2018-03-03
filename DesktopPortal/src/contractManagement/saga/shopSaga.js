import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import {
  saveShopBasicStart, saveShopBasicFinish, mySearchFinish,
  shopsListStart, shopsListFinish, gotoShopPage,submitShopsInfoStart,
  gotoThisShopStart, gotoThisShopFinish,
  myBuildingListStart, myBuildingListFinish,
  saveShopLeaseStart, saveShopLeaseFinish,
  buildingsListStart, buildingsListFinish,
  deleteStart, deleteFinish, submitShopsInfoFinish,
  saveShopSupportStart, saveShopSupportFinish, uploadPicFinish,
  getAddBuildingStart,getAddBuildingEnd,geAddtShopStart,getAddShopsEnd,
  deleteBuildingStart,deleteBuildingFinish,getChangeBuildingListStart,getChangeBuildingListEnd,
  changeShowGroup,saveShopSummaryInfoEnd,saveShopSummaryInfoStart,
} from '../actions/actionCreator'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

// 保存商铺基本信息

export function* saveShopBasicAsync(action) {
  // console.log(action, '保存基本信息')
  let body = action.payload.entity;
  let url = WebApiConfig.shopBasic.Base + '/' + body.buildingId + '/' + body.id;
  let basicOperType = action.payload.basicOperType;
  let res;
  // console.log(basicOperType, 'type')
  // console.log(JSON.stringify(body), url, '请求体')
  yield put(actionUtils.action(saveShopBasicStart, body))
  try {
    res = yield call(ApiClient.post, url, body, null, 'PUT')
    if (!res.data.extension) {
      res.data.extension = body
    }
    yield put(actionUtils.action(saveShopBasicFinish, res));
  } catch (e) {
    yield put(actionUtils.action(saveShopBasicFinish, { code: '1', message: '保存失败', entity: body }));
  }
}

// 保存商铺租约信息
export function* saveShopLeaseAsync(action) {
  let body = action.payload.entity;
  let url = WebApiConfig.shopLease.Base + '/' + body.buildingId + '/' + body.id;
  let leaseOperType = action.payload.leaseOperType;
  body.startDate = moment(body.dateRange[0]._d).format('YYYY-MM-DD')
  body.endDate = moment(body.dateRange[1]._d).format('YYYY-MM-DD')
  let res;
  yield put(actionUtils.action(saveShopLeaseStart, body))
  try {
    res = yield call(ApiClient.post, url, body, null, 'PUT')
    if (!res.data.extension) {
      res.data.extension = body
    }
    res.data.extension.dateRange = body.dateRange
    yield put(actionUtils.action(saveShopLeaseFinish, res));
  } catch (e) {
    yield put(actionUtils.action(saveShopLeaseFinish, { code: '1', message: '保存失败', entity: body }));
  }
}

// 保存商铺配套信息
export function* saveShopSupportAsync(action) {
  let body = action.payload.entity;
  let url = WebApiConfig.shopSupport.Base + '/' + body.buildingId + '/' + body.id;
  let supportOperType = action.payload.supportOperType;
  let res;
  yield put(actionUtils.action(saveShopSupportStart, body))
  try {
    res = yield call(ApiClient.post, url, body, null, 'PUT')
    if (!res.data.extension) {
      res.data.extension = body
    }
    res.data.extension.basic = body.basic
    yield put(actionUtils.action(saveShopSupportFinish, res));
  } catch (e) {
    yield put(actionUtils.action(saveShopSupportFinish, { code: '1', message: '保存失败', entity: body }));
  }
}

// 提交商铺信息
export function* submitShopsInfo(action) {
  let body = action.payload.entity;
  let url = WebApiConfig.shopsInfo.Base + body.id;
  let res;
  yield put(actionUtils.action(submitShopsInfoStart, res));
  // console.log(JSON.stringify(body), url, '提交商铺信息请求体')
  try {
    res = yield call(ApiClient.post, url);
    yield put(actionUtils.action(submitShopsInfoFinish, res));
    // console.log(body, res, '提交商铺信息res')
    
  } catch (e) {
    yield put(actionUtils.action(submitShopsInfoFinish, { code: '1', message: '提交失败', entity: body }));
  }
}
// 点击新增商铺
export function* gotoAddShop(action) {
  let build = action.payload;
  yield put(actionUtils.action(gotoShopPage, build))
  yield put(actionUtils.action(changeShowGroup, {type: 2}));
}

// 编辑商铺
export function* gotoThisShop(action) {
  let buildName = action.payload.build.basicInfo.name;
  let url; 
  if (action.payload.type === 'dynamic') {
      url = WebApiConfig.shopsInfo.GetThisShops + action.payload.id;
  } else {
     url = WebApiConfig.shopsInfo.GetThisShops + action.payload.shopsInfo.id;
  }
  let res;
  // console.log(url, '这个商铺》？？')
  yield put(actionUtils.action(gotoThisShopStart))
  try {
    res = yield call(ApiClient.get, url);
    res.data.extension.buildName = buildName
    
    if (action.payload.type === 'dynamic') {
      res.data.dynamic = 'dynamic';
      yield put(actionUtils.action(gotoThisShopFinish, res));
      yield put(actionUtils.action(changeShowGroup, {type: 1}));
    } else {
      res.data.build = action.payload.build
      yield put(actionUtils.action(gotoThisShopFinish, res));
      yield put(actionUtils.action(changeShowGroup, {type: 2}));
    }
  } catch (e) {
    notification.error({
      message: '加载商铺失败',
      duration: 3
    })
  }
}

// 删除商铺
export function* deleteShop(action) {
  let body = action.payload.body;
  let url = WebApiConfig.shopsList.DeleteShop + body.id;
  let res;
  yield put(actionUtils.action(deleteStart))
  try {
    res = yield call(ApiClient.post, url, null, null, 'DELETE');
    if (res.data.code === '0') {
      yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) });
    }
    yield put(actionUtils.action(deleteFinish, res));
  } catch (e) {
    yield put(actionUtils.action(deleteFinish, { code: '1', message: '删除失败', entity: body }));
  }
}



// 搜索我的楼盘
export function* searchMyBuildingList(action) {
  // console.log(action.payload, 'key')
  let url = WebApiConfig.buildings.buildingSearch
  let list = []
  let body = {
    "keyWord": action.payload.value, 
  }
  yield put(actionUtils.action(getAddBuildingStart))
  try {
    let res = yield call(ApiClient.post, url, body);
    // console.log(res, 'resaaaa')
    res.data.type = action.payload.type
    if (res.data.code === '0') {
      let buildingArr = res.data.extension
      // console.log(buildingArr, '搜索我的楼盘')
      if (action.payload.type === 1) { // 因为切换楼盘和新增房源数据结构不同，所以type来区别，1 是切换楼盘得搜索 2 是新增房源得搜索
        // console.log(1)
        yield put(actionUtils.action(mySearchFinish, buildingArr))
      } else {
        // console.log(2)
        for (let i = 0; i < buildingArr.length; i++) {
          list.push({ build: buildingArr[i], isChecked: false })
        }
        yield put(actionUtils.action(getAddBuildingEnd, list))
      }
      
    } 
    
  } catch (e) {
    yield put(actionUtils.action(mySearchFinish, { data: { code: '1', message: '失败' } }));
  }
}



function* getShops(val) {
  let extension = []
  let url = WebApiConfig.shopsList.List
  let body = {
    buildingIds: [val.id],
    pageIndex: 0,
    pageSize: 9999
  }
  // console.log(url, JSON.stringify(body), '获取商铺列表请求体')
  try {
    let res = yield call(ApiClient.post, url, body)
    // console.log(res, '获取商铺列表res')
    if (res.data.code === '0') {
      extension = res.data.extension
    } else {
      extension = []
    }
  } catch (e) {
    extension = []
  }
  return extension
}

// 点击楼盘箭头图标获取商铺列表
export function* geAddtShopList(action) {
  // console.log(action, '新增点击楼盘箭头图标获取商铺列表')
  let value = action.payload.value
  if (value.isChecked) {
    // console.log(value.isChecked, 'aaaa')
    yield put(actionUtils.action(geAddtShopStart, {index: action.payload.index}));
    return
  }
  let shops = yield getShops(value.build)
  // console.log(shops, '新增列表商铺')
  yield put(actionUtils.action(getAddShopsEnd, {shops: shops, index: action.payload.index}));
}

// 获取楼盘列表
export function* getAddBuilding(action) {
  let dynamicData = [];
  let url = WebApiConfig.buildings.buildingSearch;
  let body = {};
  if (action.payload.city) {
    body = {
      city: action.payload.city
    }
  }
  let res;
  console.log(url, '获取新增楼盘列表地址', body)
  yield put(actionUtils.action(getAddBuildingStart))
  try {
    let res = yield call(ApiClient.post, url, body)
    // console.log(res, '获取新增楼盘列表res')
    if (res.data.code === '0') {
      let buildingArr = res.data.extension
      for (let i = 0; i < buildingArr.length; i++) {
        dynamicData.push({ build: buildingArr[i], isChecked: false })
      }
      // console.log(dynamicData, '新增列表楼盘00000')
      yield put(actionUtils.action(getAddBuildingEnd, dynamicData))
      yield put(actionUtils.action(myBuildingListFinish, buildingArr))
    } 
  } catch (e) {
    yield put(actionUtils.action(getAddBuildingEnd, { code: '1', message: '获取楼盘列表失败'}))
  }
} 

// 获取切换楼盘列表
export function* getChangeBuildingList(action) {
  let list = []
  let url = WebApiConfig.buildings.buildingSearch;
  let body = {
    city: action.payload.city
  }
  let res;
  // console.log(url, '获取获取切换楼盘列表地址')
  yield put(actionUtils.action(getChangeBuildingListStart))
  try {
    let res = yield call(ApiClient.post, url, body)
    // console.log(res, 8888)
    if (res.data.code === '0') {
      let buildingArr = res.data.extension.filter(v => {
        return v.examineStatus === 8
      })
      yield put(actionUtils.action(getChangeBuildingListEnd, buildingArr))
      if (buildingArr.length !== 0 ) {
        yield put({type: actionUtils.getActionType(actionTypes.GET_USER_TYPE_VALUE), payload: {buildingId: buildingArr[0].id}}); // 获取用户历史操作过的楼盘
      }
    }
  } catch (e) {
    yield put(actionUtils.action(getChangeBuildingListEnd, { code: '1', message: '获取楼盘列表失败'}))
  }
}

// 获取当前用户的历史操作切换楼盘
export function* getUserTypeValue(action) {
  let url = WebApiConfig.userTypeValue.Base + '/ZC_CURRENT_PROJECT'
  let pid = ''
  try {
     let res = yield call(ApiClient.get, url);
     if(res && res.data && res.data.code==="0" && res.data.extension.length>0){
        pid = res.data.extension[0].value
     }
     if(pid){
       //取最新的楼盘
       yield put({type: actionUtils.getActionType(actionTypes.GET_THIS_BUILDING), payload: {buildingId: pid}});
      //  yield put({type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), payload: {buildingId: pid, type: 'id', pageIndex: 0}});
     } else {
       // 如果没有历史操作，默认取数组第一条
       yield put({type: actionUtils.getActionType(actionTypes.GET_THIS_BUILDING), payload: {buildingId: action.payload.buildingId}});
      //  yield put({type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), payload: {buildingId: action.payload.buildingId, type: 'id', pageIndex: 0}});
     }
  } catch (e) {
 
  }
}



// 删除楼盘
export function* deleteBuilding(action) {
  let body = action.payload.body;
  let url = WebApiConfig.buildings.DeleteBuilding + body.id;
  let res;
  yield put(actionUtils.action(deleteBuildingStart))
  try {
    res = yield call(ApiClient.post, url, null, null, 'DELETE');
    if (res.data.code === '0') {
      yield put({ type: actionUtils.getActionType(actionTypes.GET_ADD_BUILDING) });
    }
    yield put(actionUtils.action(deleteBuildingFinish, res));
  } catch (e) {
    yield put(actionUtils.action(deleteBuildingFinish, { code: '1', message: '删除失败', entity: body }));
  }
}


// 保存商铺简介信息
export function* saveShopSummaryInfo(action) {
  let body = action.payload.entity;
  let url = WebApiConfig.shopSummary.Base + '/' + body.buildingId + '/' + body.id;
  let projectOperType = action.payload.projectOperType;
  let res;
  // console.log(JSON.stringify(body), url, ' 保存商铺简介信息请求体')
  yield put(actionUtils.action(saveShopSummaryInfoStart, body))
  try {
    res = yield call(ApiClient.post, url, body, null, 'PUT')
    if (!res.data.extension) {
      res.data.extension = body.summary
    }
    // console.log(res, 'res')
    yield put(actionUtils.action(saveShopSummaryInfoEnd, res));
  } catch (e) {
    yield put(actionUtils.action(saveShopSummaryInfoEnd, { code: '1', message: '保存失败', entity: body }));
  }
}

export function* watchGetShop() {
  yield takeLatest(actionUtils.getActionType(actionTypes.SHOP_BASIC_SAVE_ASYNC), saveShopBasicAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.SHOP_LEASE_SAVE_ASYNC), saveShopLeaseAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.SHOP_SUPPORT_SAVE_ASYNC), saveShopSupportAsync);
  // yield takeLatest(actionUtils.getActionType(actionTypes.BUILDING_GET_LIST_ASYNC), getBuildingsListAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.SHOPS_INFO_SUBMIT), submitShopsInfo);
  // yield takeEvery(actionUtils.getActionType(actionTypes.SHOPS_GET_LIST_ASYNC), getShopsListAsync);
  // yield takeEvery(actionUtils.getActionType(actionTypes.BUILDING_GET_MYLIST_ASYNC), getMyBuildingsListAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_THIS_SHOP), gotoThisShop);
  yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_SHOP), deleteShop);
  yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_ADD_SHOP), gotoAddShop);
  // yield takeLatest(actionUtils.getActionType(actionTypes.LOOK_MORE), lookMore);
  yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_MYBUILDING_LIST), searchMyBuildingList);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_ADD_SHOP_LIST), geAddtShopList);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_ADD_BUILDING), getAddBuilding);
  yield takeLatest(actionUtils.getActionType(actionTypes.DELETE_BUILDING), deleteBuilding);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_CHANGE_BUILDING_LIST), getChangeBuildingList);
  yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_SHOP_SUMMARY_INFO), saveShopSummaryInfo);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_USER_TYPE_VALUE), getUserTypeValue);
}

