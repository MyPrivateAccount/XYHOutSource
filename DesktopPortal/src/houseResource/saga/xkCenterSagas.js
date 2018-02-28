import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import {saveTimeStart, saveTimeFinish,
   getShopsSaleStatusStart, getShopsSaleStatusFinish,getMakeDealCustomerInfoStart,
   getCustomerDealStart,getCustomerDealFinish,getMakeDealCustomerInfoEnd,
   getSalestatisticsStart, getSalestatisticsFinish} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);


// 保存小定时间 || 解除锁定 （不传lockTime）|| 开始售卖
export function* saveTimeAsync(action) {
  let url = WebApiConfig.xk.UpdateSalestatus,
      current = action.payload.current,
      body;
  if (action.payload.time) { // 变成锁定
    body = {
      'lockTime': action.payload.time,
      "shopsIds": action.payload.id,
      'saleStatus': '3',
    }
  } else if (action.payload.stop) {
    body = {
      "shopsIds": action.payload.id,
      'saleStatus': '1',
    }
  } else { // 解锁以及开始售卖 ==> 状态为在售
    body = {
      "shopsIds": action.payload.id,
      'saleStatus': '2',
    }
  }
  // console.log(url, JSON.stringify(body), '请求体')
  yield put(actionUtils.action(saveTimeStart));
  try {
      let res = yield call(ApiClient.post, url, body, null, 'PUT');
      // console.log(res, 'res')
      yield put(actionUtils.action(saveTimeFinish, res.data));
      if (current === '0') {
        yield put({ type: actionUtils.getActionType(actionTypes.GET_SHOPS_SALE_STATUS), 
        payload: { buildingId: [action.payload.buildingId], saleStatus: [], type:'current' }});
      } else {
        yield put({ type: actionUtils.getActionType(actionTypes.GET_SHOPS_SALE_STATUS), 
        payload: { buildingId: [action.payload.buildingId], saleStatus: [`${current}`], type:'current'  }});
      }
  } catch (e) {
      yield put(actionUtils.action(saveTimeFinish, { data: { code: '1', message: '失败'} }));
  }
}

// 获取商铺销售状态
export function* getShopsSaleStatus(action) {
  let url = WebApiConfig.xk.Base, body;
  if (action.payload.who) { // 请求此接口的是看板列表
    body = {
      "saleStatus": action.payload.saleStatus,
      "pageIndex": 0,
      "pageSize": 100000000,
      "buildingIds": action.payload.buildingId,
    } 
  } else {
    body = {
      "saleStatus": action.payload.saleStatus,
      "pageIndex": action.payload.pageIndex,
      "pageSize": 10,
      "buildingIds": action.payload.buildingId,
    }
  }
  // console.log(url, JSON.stringify(body), '获取商铺销售状态请求体')
  action.payload.type ? // type==='current ' 说明是点击的销售类别切换
  yield put(actionUtils.action(getShopsSaleStatusStart, action.payload.type))
  :
  yield put(actionUtils.action(getShopsSaleStatusStart))
  try {
      let res = yield call(ApiClient.post, url, body);
      if (action.payload.who) {
        res.data.who = action.payload.who
      }
      res.data.extension.totalCount = res.data.totalCount
      console.log(res.data, 'success')
      yield put(actionUtils.action(getShopsSaleStatusFinish, res.data));
  } catch (e) {
      yield put(actionUtils.action(getShopsSaleStatusFinish, { data: { code: '1', message: '失败'} }));
  }
}

// 获取楼盘下的商铺销售状态统计
export function* getSalestatistics(action) {
  let url = WebApiConfig.xk.Salestatistics + action.payload.buildingId;
  // console.log(url, '获取楼盘下的商铺销售状态统计')
  yield put(actionUtils.action(getSalestatisticsStart))
  try {
      let res = yield call(ApiClient.get, url);
      // console.log(res.data, '获取楼盘下的商铺销售状态统计success')
      yield put(actionUtils.action(getSalestatisticsFinish, res.data));
  } catch (e) {
      yield put(actionUtils.action(getSalestatisticsFinish, { data: { code: '1', message: '失败'} }));
  }
}

// 点击查看成交信息
export function* getCustomerDeal(action) {
  let url = WebApiConfig.customerDeal.GetCustomerDeal + action.payload;
  // console.log(url, '请求体')
  yield put(actionUtils.action(getCustomerDealStart))
  try {
      let res = yield call(ApiClient.get, url);
      // console.log(res, '12')
      yield put(actionUtils.action(getCustomerDealFinish, res.data));
  } catch (e) {
      yield put(actionUtils.action(getCustomerDealFinish, { data: { code: '1', message: '失败'} }));
  }
}

// 获取成交客户等等信息
export function* getMakeDealCustomerInfo(action) {
  let url = WebApiConfig.customerTransactions.Search
  let body = {
    "buildingId": action.payload.buildingId,
    "status": action.payload.status,
    "pageIndex": 0,
    "pageSize": 99999
  }
  // console.log(url, JSON.stringify(body), '获取成交客户等等信息')
  yield put(actionUtils.action(getMakeDealCustomerInfoStart));
  try {
    let res = yield call(ApiClient.post, url, body);
    res.data.entity = body
    // console.log(res, '获取成交客户等等信息res')
    yield put(actionUtils.action(getMakeDealCustomerInfoEnd, res.data));
  } catch (e) {
    yield put(actionUtils.action(getMakeDealCustomerInfoEnd, { data: { code: '1', message: '失败'} }));
  }
}
export function* watchCenterAllAsync() {
  yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_TIME_ASYNC), saveTimeAsync);
  yield takeEvery(actionUtils.getActionType(actionTypes.GET_SHOPS_SALE_STATUS), getShopsSaleStatus);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_CUSTOMER_DEAL), getCustomerDeal);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_SALES_TATISTICS), getSalestatistics);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_MAKE_DEAL_CUSTOMER_INFO), getMakeDealCustomerInfo);
  
}