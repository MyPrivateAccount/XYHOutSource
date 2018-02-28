import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction, { NewGuid } from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import {gotoShopDetailPageStart, gotoShopDetailPageFinish,getId,getShopsStart,getShopsFinish,getShopsClear,getShopsEnd,
  gotoProjectDetailPageStart, gotoProjectDetailPageFinish, submitDynamicEnd, submitDynamicStart,getBuildingEnd,getBuildingStart,
  getExaminesStatusEnd,getExaminesStatusStart, clearDescription,getDynamicStatusStart,clearTitle,
  priceView,youhuiView,viewBatchBuilding,rulesView,rulesTemplateView,getDynamicStatusEnd,changeShowGroup,
  commissionView,shopPicView,getProjectDynamicInfoDetailEnd,getDynamicInfoListEnd,getShopDynamicInfoDetailEnd} from '../actions/actionCreator'

const actionUtils = appAction(actionTypes.ACTION_ROUTE);

// 点击商铺列表进入对应的详情页面
export function* gotoShopDetailPage(action) {
  console.log(action.payload, '点击商铺进入页面')
  let id = action.payload;
  yield put(actionUtils.action(gotoShopDetailPageStart))
  try {
    yield put({ type: actionUtils.getActionType(actionTypes.GOTO_THIS_SHOP), payload: { id: id, type: 'dynamic'}});
    yield put(actionUtils.action(gotoShopDetailPageFinish));
    yield put({ type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), payload: { id: id }});
    yield put(actionUtils.action(getId, {id: id, type: 'shop'}));
  } catch (e) {
    notification.error({
      message: '加载失败',
      duration: 3
    })
  }
}

// 点击楼盘列表进入对应的详情页面
export function* gotoProjectDetailPage(action) {
  console.log(action.payload, '点击楼盘进入页面')
  let id = action.payload;
  yield put(actionUtils.action(gotoProjectDetailPageStart))
  try {
    yield put(actionUtils.action(gotoProjectDetailPageFinish));
    yield put({ type: actionUtils.getActionType(actionTypes.GOTO_THIS_BUILD), payload: { id: id, type: 'dynamic'}});
    yield put({ type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), payload: { id: id }});
    yield put(actionUtils.action(getId, {id: id, type: 'project'}));
  } catch (e) {
    notification.error({
      message: '加载失败',
      duration: 3
    })
  }
}

// 楼盘热卖户型页面（在售） ||   楼盘加推页面 (待售)
export function* getShops(action) {
  // console.log(action.payload, '楼盘热卖户型页面')
  let url = WebApiConfig.xk.Base,
      // type = action.payload.type, // true 热卖  false 加推
      body = {
        "buildingIds": [action.payload.id],
        "saleStatus": action.payload.saleStatus,
        "pageIndex": 0,
        "pageSize": 99999
      },
      res;
  // console.log(url, JSON.stringify(body), '请求体啊啊啊')
  yield put(actionUtils.action(getShopsStart));
  try {
    res = yield call(ApiClient.post, url, body)
    // console.log(res, 'hahah 成功没有')
    yield put(actionUtils.action(getShopsFinish, res.data));
  } catch (e) {
    yield put(actionUtils.action(getShopsFinish, {data: {code: '1', message: '失败'} } ));
  }
} 

// 保存信息
export function* submitDynamic(action) {
  let url, body = action.payload.obj;
  body.id = NewGuid();
  url = WebApiConfig.dynamic.Base + '/submit'
  console.log(url, body, JSON.stringify(body), '请求体啊啊啊')
  yield put(actionUtils.action(submitDynamicStart))
  try {
    let res = yield call(ApiClient.post,url, body)
    console.log(res, '保存成功没有？？？')
    res.data.entity = body
    res.data.isBtn = action.payload.isBtn || null
    yield put(actionUtils.action(submitDynamicEnd, res.data));
    if (res.data.code === '0') { 
      yield put(actionUtils.action(clearDescription)); //清空文本框
      yield put(actionUtils.action(clearTitle)); //清空文本框
      switch(body.contentType) {
      case 'ReportRule':
      console.log(JSON.parse(body.updateContent), '哟哟哦哟')
            yield put({ 
              type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
              payload: { id: body.contentId }
            }); 
            yield put({ 
              type: actionUtils.getActionType(actionTypes.RULES_VIEW), 
              payload:{ body: JSON.parse(body.updateContent), type: 'dynamic'}
            });
            // yield put({ 
            //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
            //   payload: { id: body.contentId }
            // });
            break;
      case 'CommissionType': 
            yield put({ 
              type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
              payload: { id: body.contentId }
            });
            yield put({ 
              type: actionUtils.getActionType(actionTypes.COMMISSION_VIEW), 
              payload: {body: JSON.parse(body.updateContent), type: 'dynamic'}
            });
            // yield put({ 
            //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
            //   payload: { id: body.contentId }
            // });
            break;
      case 'BuildingNo': 
            yield put({ 
              type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
              payload: { id: body.contentId }
            });
            yield put({
              type: actionUtils.getActionType(actionTypes.BATCH_BUILDING_VIEW), 
              payload: { body: JSON.parse(body.updateContent), type: 'dynamic' }
            });
            // yield put({ 
            //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
            //   payload: { id: body.contentId }
            // });
            break;
      case 'DiscountPolicy': 
            yield put({ 
              type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
              payload: { id: body.contentId }
            });
            yield put({
              type: actionUtils.getActionType(actionTypes.YOU_HUI_VIEW), 
              payload: { body: JSON.parse(body.updateContent) }
            });
            //  yield put({ 
            //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
            //   payload: { id: body.contentId }
            // });
            break;
      case 'Image': 
            if (body.updateType === 1) { // 1 楼盘 2 商铺
              yield put({ 
                type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
                payload: { id: body.contentId }
              });
              yield put({ 
                type: actionUtils.getActionType(actionTypes.BUILDING_PIC_VIEW), 
                payload: {filelist: JSON.parse(body.updateContent), dynamic: 'dynamic',type: 'add'}
              });
              // yield put({ 
              //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
              //   payload: { id: body.contentId }
              // });
            } else {
              yield put({ 
                type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
                payload: { id: body.contentId }
              });
              yield put({ 
                type: actionUtils.getActionType(actionTypes.SHOP_PIC_VIEW), 
                payload: {filelist: JSON.parse(body.updateContent), dynamic: 'dynamic',type: 'add'}
              });
              // yield put({ 
              //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
              //   payload: { id: body.contentId }
              // });
            };break;
      // case 'Attachment': 
      //       yield put(actionUtils.action(commissionView, {}));break;
      case 'Price':  // 价格有总价有指导价
              yield put({ 
                type: actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), 
                payload: { id: body.contentId }
              });
              yield put({ 
                type: actionUtils.getActionType(actionTypes.PRICE_VIEW), 
                payload: { body: JSON.parse(body.updateContent) }
              });
              // yield put({ 
              //   type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), 
              //   payload: { id: body.contentId }
              // });
              break;
      }
    }
  } catch (e) {
    yield put(actionUtils.action(submitDynamicEnd, {data: {code: '1', message: '失败'} } ));
  }
}

// 获取该楼盘有无审核状态的板块
export function* getExaminesStatus(action) {
  let url =  WebApiConfig.dynamic.Examines + action.payload.id
  console.log(url,  '请求体啊啊啊')
  yield put(actionUtils.action(getExaminesStatusStart))
  try {
    let res = yield call(ApiClient.get, url)
    console.log(res, '获取该楼盘有无审核状态的板块成功没有？？？')
    yield put(actionUtils.action(getExaminesStatusEnd, res.data));
  } catch (e) {
    yield put(actionUtils.action(getExaminesStatusEnd, {data: {code: '1', message: '失败'} } ));
  }
}

 //获取最后一次审核信息
 export function* getDynamicInfoList(action) {
   let url =  WebApiConfig.dynamic.List
   let condition = action.payload.condition
   let updateType = action.payload.updateType
  //  console.log(url, JSON.stringify(condition), '获取最后一次审核信息请求体啊啊啊')
   try {
    let res = yield call(ApiClient.post, url, condition)
    console.log(res, '获取最后一次审核信息!!!!')
    if (res.data.code === '0') {
      yield put(actionUtils.action(getDynamicInfoListEnd, res.data));
      let recordList = res.data.extension || []
      if (recordList.length > 0) {
        yield put({ 
          type: actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_DETAIL), 
          payload: { id: recordList[0].id }
        });
      } 
      else {
        yield put(actionUtils.action(getDynamicStatusStart, {isLoading: false}));
        if (updateType === 1) { // 楼盘
          console.log('无审核list，获取楼盘的')
          yield put({ 
            type: actionUtils.getActionType(actionTypes.GOTO_THIS_BUILD), 
            payload: { id: action.payload.id , type: 'dynamic' }
          });
        } else { 
          yield put({ // 商铺
            type: actionUtils.getActionType(actionTypes.GOTO_THIS_SHOP), 
            payload: { id: action.payload.id, type: 'dynamic' }
          });
        }
      }
    } 
    // yield put(actionUtils.action(getDynamicInfoListEnd, res.data));
  } catch (e) {
    notification.error({
      message: '获取动态详情失败',
      duration: 3
    })
  }
}
 //获取获取动态详细
 export function* getDynamicInfoDetail(action) {
  let url =  WebApiConfig.dynamic.Base + '/' + action.payload.id
  // console.log(url, '获取获取动态详细请求体啊啊啊')
  yield put(actionUtils.action(getDynamicStatusStart, {isLoading: true}));
  try {
    let res = yield call(ApiClient.get, url)
    // console.log(res, '获取获取动态详细!!!!')
    yield put(actionUtils.action(getDynamicStatusEnd, res.data)); // 动态详情状态
    if (res.data.code === '0') {
      if (res.data.extension.updateType === 1) {
        // console.log(1,'999')
        yield put(actionUtils.action(getProjectDynamicInfoDetailEnd, res.data)); // 在buildingsReducer里面
      } else {
        yield put(actionUtils.action(getShopDynamicInfoDetailEnd, res.data)); // 在shopReducer里面
      }
    }
 } catch (e) {
 }
}


function* getmyShops(val) {
  let extension = []
  let url = WebApiConfig.xk.Base
  let body = {
    buildingIds: [val.id],
    pageIndex: 0,
    pageSize: 9999
  }
  // console.log(url, JSON.stringify(body), '获取商铺列表加推热卖')
  try {
    let res = yield call(ApiClient.post, url, body)
    // console.log(res, '获取商铺列表加推热卖res')
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
export function* getShopList(action) {
  // console.log(action, '点击楼盘箭头图标获取商铺列表')
  let value = action.payload.value
  if (value.isChecked) {
    // console.log(value.isChecked, 'aaaa')
    yield put(actionUtils.action(getShopsClear, {index: action.payload.index}));
    return
  }
  let shops = yield getmyShops(value.build)
  // console.log(shops, '列表商铺')
  yield put(actionUtils.action(getShopsEnd, {shops: shops, index: action.payload.index}));
}

// 获取楼盘列表
export function* getBuilding(action) {
  let dynamicData = []
  let url = WebApiConfig.buildings.buildingSearch;
  let body = {
    city: action.payload.city
  }
  let res;
  // console.log(url, '获取楼盘列表地址')
  yield put(actionUtils.action(getBuildingStart, dynamicData))
  try {
    let res = yield call(ApiClient.post, url, body)
    // console.log(res, '获取楼盘列表res')
    if (res.data.code === '0') {
      let buildingArr = res.data.extension.filter(v => {
        return v.examineStatus === 8
      })
      for (let i = 0; i < buildingArr.length; i++) {
        dynamicData.push({ build: buildingArr[i], isChecked: false })
      }
      // console.log(dynamicData, '列表楼盘00000')
      yield put(actionUtils.action(getBuildingEnd, dynamicData))
    } 
  } catch (e) {
    yield put(actionUtils.action(getBuildingEnd, { code: '1', message: '获取楼盘列表失败'}))
  }
}




export function* watchActiveAllAsync() {
  yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_PROJECT_DETAIL_PAGE), gotoProjectDetailPage);
  yield takeLatest(actionUtils.getActionType(actionTypes.GOTO_SHOP_DETAIL_PAGE), gotoShopDetailPage);
  yield takeEvery(actionUtils.getActionType(actionTypes.GET_SHOPS), getShops);
  yield takeEvery(actionUtils.getActionType(actionTypes.DYNAMIC_SUBMIT_DETAILS_VALUES), submitDynamic)
  yield takeEvery(actionUtils.getActionType(actionTypes.GET_EXAMINES_STATUS), getExaminesStatus)
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_LIST), getDynamicInfoList);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_DYNAMIC_INFO_DETAIL), getDynamicInfoDetail);
  yield takeLatest(actionUtils.getActionType(actionTypes.DYNAMIC_SET_SHOPS_VALUE), getShopList);
  yield takeLatest(actionUtils.getActionType(actionTypes.DYNAMIC_SET_BUILDING_VALUE), getBuilding);
}