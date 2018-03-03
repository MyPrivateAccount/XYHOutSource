// 驻场首页sagas

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
  searchFinish, customerTransactionsFinish, customerDealFinish, customerDealStart,
  myListStart, myListFinish, comfirmFinish, lookFinish, countFinish, SearchValphoneStart, SearchValphoneEnd,
  getThisBuildStart, getThisBuildFinish, getThisProjectIndex, getReportCustomerDealStart, getReportCustomerDealFinish
} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);


// 根据楼盘id得到该楼盘基本信息
export function* getThisBuilding(action) { // 在shopSaga里面请求此接口
  let url = WebApiConfig.buildings.GetThisBuildings + action.payload.buildingId;
  let res;
  yield put(actionUtils.action(getThisBuildStart))
  try {
    res = yield call(ApiClient.get, url);
    console.log(res, '获取楼盘基本信息')
    yield put(actionUtils.action(getThisBuildFinish, res));
    if (res.data.code === '0') {
      let ruleInfo = res.data.extension.ruleInfo || {}

      if (ruleInfo.isCompletenessPhone) { // true, 就传valphone = true 
        yield put({ // 获取该楼盘下得所有报备信息
          type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), 
          payload: {
            buildingId: action.payload.buildingId, 
            type: 'id', 
            pageIndex: 0,
            valphone: true
        }});
        yield put({ // 获取批量操作得数据
          type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
          payload: {
            buildingId: res.data.extension.id, valphone: true
          }
        });
      } else {
        yield put({ // 获取该楼盘下得所有报备信息
          type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), 
          payload: {
            buildingId: action.payload.buildingId, 
            type: 'id', 
            pageIndex: 0,
            valphone: false
        }});
        yield put({
          type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
          payload: {
            buildingId: res.data.extension.id, valphone: false
          }
        });
      }

    }
  } catch (e) {
    yield put(actionUtils.action(getThisBuildFinish, { code: '1', message: '失败' }));
  }
}

// 根据是否显示全号码查询
export function* SearchValphone(action) {
  let url = WebApiConfig.customerTransactions.SearchValphone;
  let body = action.payload;
  let res;
  yield put(actionUtils.action(SearchValphoneStart))
  try {
    res = yield call(ApiClient.post, url, body);
    console.log(res, '根据是否显示全号码查询')
    yield put(actionUtils.action(SearchValphoneEnd, res.data));
  } catch (e) {
    yield put(actionUtils.action(SearchValphoneEnd, { code: '1', message: '失败' }));
  }
}
// 根据楼盘id以及其他条件查询该楼盘下得所有报备信息
export function* getMyCustomerInfo(action) { // 在shopSaga里面请求此接口
  let url = WebApiConfig.customerTransactions.Search
  let type = action.payload.type
  // console.log(action.payload, '查询客户报备')

  let body
  switch (type) {
    case 'id':
      body = {
        "buildingId": action.payload.buildingId,
        "pageIndex": action.payload.pageIndex,
        "pageSize": 10,
        'num': 1,
      };
      if (action.payload.valphone) {
        body.valphone = action.payload.valphone
      };
      break;
    case 'keyWord':
      let status = action.payload.status
      switch (status) {
        case 'all': status = null; break;
        case '0': status = [0]; break;
        case '1': status = [1]; break;
        case '2': status = [2]; break;
        case '3': status = [3]; break;
        case '4': status = [4]; break;
        default: status = [5, 6];
      }
      body = {
        "buildingId": action.payload.buildingId,
        "keyWord": action.payload.keyWord,
        "status": status,
        "pageIndex": action.payload.pageIndex,
        "pageSize": 10
      }; 
      if (action.payload.valphone) {
        body.valphone = action.payload.valphone
      };
      if (status && status[0] === 1) {
        body.isToDay = true;
      };
      break;
    default:
      body = {
        "buildingId": action.payload.buildingId,
        "status": action.payload.status,
        "pageIndex": action.payload.pageIndex,
        "pageSize": 10
      }
      if (action.payload.valphone) {
        body.valphone = action.payload.valphone
      };
      if (action.payload.status && action.payload.status[0] === 1) {
        body.isToDay = true;
      }
  }
  // console.log(JSON.stringify(body), url, '请求体')
  try {
    let res = yield call(ApiClient.post, url, body);
    if (type === 'id') {
      res.data.num = body.num
    }
    res.data.entity = body
    console.log(res, '报备查询res')
    yield put(actionUtils.action(customerTransactionsFinish, res.data));
  } catch (e) {
    yield put(actionUtils.action(customerTransactionsFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 根据楼盘id查询确认报备和向开放商报备状态的数据总数
export function* statusCount(action) {
  console.log(action, '根据楼盘id查询确认报备和向开放商报备状态的数据总数')
  let url = WebApiConfig.customerTransactions.StatusCount
  let body = {
    buildingId: action.payload.buildingId,
    completephone: action.payload.valphone
  }
  console.log(url, body, 'xxxx')
  try {
    let res = yield call(ApiClient.post, url, body);
    console.log(res, '状态的数据总数res')
    yield put(actionUtils.action(countFinish, res));
  } catch (e) {
    yield put(actionUtils.action(countFinish, { code: '1', message: '失败' }));
  }
}

// 搜索我的楼盘
export function* searchMyBuildingList(action) {
  let url = WebApiConfig.buildings.getBuildingSreach
  let body = {
    "keyWord": action.payload,
    "examineStatus": [8]
  }
  try {
    let res = yield call(ApiClient.post, url, body);
    yield put(actionUtils.action(searchFinish, res.data));
  } catch (e) {
    yield put(actionUtils.action(searchFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 批量确认报备
export function* comfirmAsync(action) {
  let url = WebApiConfig.customerTransactions.PLComfirmReport
  let body = action.payload.transactionsids
  let type = action.payload.type
  try {
    let res = yield call(ApiClient.post, url, body);
    res.data.body = action.payload
    // console.log(res, '报备res')
    yield put(actionUtils.action(comfirmFinish, res.data));
    if (res.data.code === '0') {
      if (type) {
        if (type === 'all') { // 切换tab=全部情况下的操作
          // console.log(1)
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: null, type: 'status' }
          });
          yield put({
            type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
            payload: { buildingId: action.payload.buildingId, valphone: action.payload.valPhone  }
          });
        } else if (type === '0') {  // 切换tab=确认报备情况下的操作
          // console.log(2)
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: [0], type: 'status' }
          });
          yield put({
            type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
            payload: { buildingId: action.payload.buildingId, valphone: action.payload.valPhone  }
          });
        }
      } else { // 第一次加载根据楼盘id进行的操作
        // console.log(3)
        yield put({
          type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
          payload: { buildingId: action.payload.buildingId, status: [0], type: 'status' }
        });
        yield put({
          type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
          payload: { buildingId: action.payload.buildingId, valphone: action.payload.valPhone  }
        });
      }
    }
  } catch (e) {
    yield put(actionUtils.action(comfirmFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 批量向开发商报备
export function* reportAsync(action) {
  let url = WebApiConfig.customerTransactions.PLReport
  let body = action.payload.transactionsids
  let type = action.payload.type
  try {
    let res = yield call(ApiClient.post, url, body);
    res.data.body = action.payload
    // console.log(res, '向开发商报备res')
    yield put(actionUtils.action(comfirmFinish, res.data));
    if (res.data.code === '0') {
      if (type) {
        if (type === 'all') { // 切换tab=全部情况下的操作
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: null, type: 'status' }
          });
          yield put({
            type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
            payload: { buildingId: action.payload.buildingId }
          });
        } else if (type === '1') { // 切换tab=确认报备情况下的操作
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: [1], type: 'status' }
          });
          yield put({
            type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
            payload: { buildingId: action.payload.buildingId }
          });
        }
      } else { // 第一次加载根据楼盘id进行的操作
        yield put({
          type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
          payload: { buildingId: action.payload.buildingId, status: [1], type: 'status' }
        });
        yield put({
          type: actionUtils.getActionType(actionTypes.STATUS_COUNT),
          payload: { buildingId: action.payload.buildingId }
        });
      }
    }

  } catch (e) {
    yield put(actionUtils.action(comfirmFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 确认带看
export function* lookAsync(action) {
  // console.log(action, 'zhizhizhi')
  let url = WebApiConfig.customerTransactions.PLLook
  let body = action.payload.transactionsids
  let type = action.payload.type
  // console.log(JSON.stringify(body), url, '确认带看请求体')
  try {
    let res = yield call(ApiClient.post, url, body);
    res.data.body = body
    // console.log(res, '确认带看res')
    yield put(actionUtils.action(lookFinish, res.data));
    if (res.data.code === '0') {
      if (type) {
        if (type === 'all') { // 切换tab=全部情况下的操作
          // console.log('all')
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: null, type: 'status' }
          });
        } else if (type === '2') {  // 切换tab=确认报备情况下的操作
          // console.log(2)
          yield put({
            type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
            payload: { buildingId: action.payload.buildingId, status: [2], type: 'status' }
          });
        }
      }
    }
  } catch (e) {
    yield put(actionUtils.action(lookFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 保存成交信息成功后 ==> 成交确认
export function* customerDealAsync(action) {
  // console.log(action, '保存成交信息action')
  let url = WebApiConfig.customerDeal.CustomerDeal
  let body = action.payload.body
  let page = action.payload.page
  let type = action.payload.type
  console.log(url, JSON.stringify(body), '请求体')
  yield put(actionUtils.action(customerDealStart));
  try {
    let res = yield call(ApiClient.post, url, body);
    res.data.body = body
    console.log(res, '保存成交信息RES')
    yield put(actionUtils.action(customerDealFinish, res.data));
    if (res.data.code === '0') {
      notification.success({
        message: '发送审核成功',
        duration: 3
      })
      // if (page === 'report') {
      //   if (type) {
      //     if (type === 'all') { // 切换tab=全部情况下的操作
      //       // console.log('all')
      //       yield put({
      //         type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
      //         payload: { buildingId: action.payload.buildingId, status: null, type: 'status' }
      //       });
      //     } else if (type === '3') {  // 切换tab=成交确认情况下的操作
      //       console.log(2)
      //       yield put({
      //         type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS),
      //         payload: { buildingId: body.projectId, status: [3], type: 'status' }
      //       });
      //     }
      //   } else {
      //     // console.log('销控中心')
      //     yield put({ type: actionUtils.getActionType(actionTypes.GET_SHOPS_SALE_STATUS), 
      //     payload: { buildingId: [body.projectId], saleStatus: [], type:'current' }});
      //   }
      } else {
        notification.error({
          message: '发送审核失败',
          duration: 3
        })
      }
  } catch (e) {
    yield put(actionUtils.action(customerDealFinish, { data: { code: '1', message: '失败' } }));
  }
}

// 点击查看成交信息
export function* getReportCustomerDeal(action) {
  let url = WebApiConfig.customerDeal.GetReportCustomerDeal + action.payload;
  console.log(url, '请求体')
  yield put(actionUtils.action(getReportCustomerDealStart))
  try {
      let res = yield call(ApiClient.get, url);
      console.log(res, '点击查看成交信息res')
      yield put(actionUtils.action(getReportCustomerDealFinish, res.data));
  } catch (e) {
      yield put(actionUtils.action(getReportCustomerDealFinish, { data: { code: '1', message: '失败'} }));
  }
}



// 更新当前用户的历史操作切换楼盘
export function* upDateUserTypeValue(action) {
  let url = WebApiConfig.userTypeValue.Base
  // console.log(url, action.payload.entity.id, '更新历史操作切换楼盘请求体')
  let userType={
    type: 'ZC_CURRENT_PROJECT',
    value: action.payload.entity.id
  }
  try {
     let res = yield call(ApiClient.post, url, userType, null, 'PUT');
    //  console.log(res, 'res更新')
     if (res.data && res.data.code==="0") {
       res.data.entity = action.payload.entity
       yield put(actionUtils.action(getThisProjectIndex, {index: action.payload.index}));
       yield put({type: actionUtils.getActionType(actionTypes.GET_THIS_BUILDING), 
                  payload: {buildingId: res.data.entity.id}});
       yield put({type: actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), 
                  payload: {buildingId: res.data.entity.id, type: 'id', pageIndex: 0}});
     }
  } catch (e) {
 
  }
 }


export function* watchIndexAllAsync() {
  yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_MYBUILDING_LIST), searchMyBuildingList);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_MYCUSTOMOR_TRANSACTIONS), getMyCustomerInfo);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_THIS_BUILDING), getThisBuilding);
  yield takeLatest(actionUtils.getActionType(actionTypes.COMFIRM_ASYNC), comfirmAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.LOOK_ASYNC), lookAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.CUSTOMER_DEAL_ASYNC), customerDealAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.REPORT_ASYNC), reportAsync);
  yield takeLatest(actionUtils.getActionType(actionTypes.STATUS_COUNT), statusCount);
  yield takeLatest(actionUtils.getActionType(actionTypes.SEARCH_VALPHONE), SearchValphone);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_REPORT_CUSTOMER_DEAL), getReportCustomerDeal);
  yield takeLatest(actionUtils.getActionType(actionTypes.UPDATE_USER_TYPE_VALUE), upDateUserTypeValue);
}