import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../../utils/apiClient'
import * as actionTypes from '../constants/actionType';
import WebApiConfig from '../constants/webapiConfig';
import appAction from '../../utils/appUtils';
import getApiResult from './sagaUtil';
import { notification } from 'antd';
import moment from 'moment';
import {savePersonFinish, savePersonStart,getExaminesListEnd,getExaminesListStart,
    getBuildingsiteStart, getBuildingsiteEnd,
    getsiteuserlistEnd,getsiteuserlistStart} from '../actions/actionCreator';

const actionUtils = appAction(actionTypes.ACTION_ROUTE);



// 获取驻场列表
export function* getBuildingsite(action) {
  let url = WebApiConfig.buildings.buildingSearch
  yield put(actionUtils.action(getBuildingsiteStart));
  let body = {
    "examineStatus": [8]
  }
  try {
      let res = yield call(ApiClient.post, url, body);
    //   console.log(res, '???')
      yield put(actionUtils.action(getBuildingsiteEnd, res.data));
  } catch (e) {
      yield put(actionUtils.action(getBuildingsiteEnd, { data: { code: '1', message: '失败'} }));
  }
}

// 获取驻场用户
export function* getsiteuserlist(action) {
    let url = WebApiConfig.zcManagement.siteuserlist
    let id = action.payload
    console.log(url, '获取驻场用户请求体')
    yield put(actionUtils.action(getsiteuserlistStart));
    try {
        let res = yield call(ApiClient.get, url);
        console.log(res.data, '获取驻场用户')
        if (res.data.code === '0') {
          if (res.data.extension.length > 0) {
            let condition = {
              pageSize: 1,
              contentTypes: 'BuildingsOnSite',
              examinAction: 'BuildingsOnSite',
              contentId: id
            } 
            yield put({type: actionUtils.getActionType(actionTypes.GET_EXAMINESLIST_LIST), 
            payload: {condition: condition, zcData: res.data.extension} });
          }
        }
        yield put(actionUtils.action(getsiteuserlistEnd, res.data));
    } catch (e) {
        yield put(actionUtils.action(getsiteuserlistEnd, { data: { code: '1', message: '失败'} }));
    }
  }

// 指派驻场
export function* savePerson(action) {
  let url = WebApiConfig.zcManagement.saveonsite;
  let body = action.payload;
  console.log(url, JSON.stringify(body), '保存驻场')
  yield put(actionUtils.action(savePersonStart));
  try {
      let res = yield call(ApiClient.post, url, body, null, 'PUT');
      console.log(res.data, 'success')
      yield put(actionUtils.action(savePersonFinish, res.data));
      if (res.data.code === '0') {
        notification.success({
          message: '指派驻场，发送审核成功',
          duration: 3
        })
      //   yield put({ type: actionUtils.getActionType(actionTypes.GET_BUILDING_SITE) });
      } else{
        notification.error({
          message: '指派驻场，发送审核失败',
          duration: 3
        })
      }
  } catch (e) {
      yield put(actionUtils.action(savePersonFinish, { data: { code: '1', message: '失败'} }));
  }
}

// 获取指派驻场的审核
export function* getExaminesList(action) {
  let url = WebApiConfig.zcManagement.ExamineFlow;
  let body = action.payload.condition;
  let zcData = action.payload.zcData;
  console.log(url, JSON.stringify(body), '获取指派驻场的审核')
  yield put(actionUtils.action(getExaminesListStart));
  try {
      let res = yield call(ApiClient.post, url, body);
      console.log(res.data, 'success')
      if (res.data.code === '0') {
        let list = res.data.extension || []
        if (list.length > 0) {
          if ([1, 3].includes(list[0].examineStatus)) {
            yield put(actionUtils.action(getExaminesListEnd, { type: 2, data: list[0].content, zcData: zcData, examineStatus: list[0].examineStatus}));
          } else {
            yield put(actionUtils.action(getExaminesListEnd, { type: 1, zcData: zcData }));
          }
        } else {
          yield put(actionUtils.action(getExaminesListEnd, { type: 1, zcData: zcData }));
        }
      }
  } catch (e) {
      yield put(actionUtils.action(getExaminesListEnd, { data: { code: '1', message: '失败'} }));
  }
}



export function* watchManagerAllAsync() {
  yield takeLatest(actionUtils.getActionType(actionTypes.SAVE_PERSON), savePerson);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_BUILDING_SITE), getBuildingsite);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_SITEUSER_LIST), getsiteuserlist);
  yield takeLatest(actionUtils.getActionType(actionTypes.GET_EXAMINESLIST_LIST), getExaminesList);
}