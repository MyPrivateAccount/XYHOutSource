import { takeEvery, takeLatest } from 'redux-saga'
import { put, call } from 'redux-saga/effects'
import ApiClient from '../utils/apiClient'
import * as actionTypes from '../constants/actionTypes';
import { ApplicationTypes } from '../constants/baseConfig';
import WebApiConfig from '../utils/webapiConfig';
import { notification } from 'antd';
import { resetPassword, close } from '../actions/actionCreators';


export function* resetPwd(state) {
  let body = {
    oldPassword : state.payload.entity.oldPassword,
    password: state.payload.entity.newPassword
  }
  let url = WebApiConfig.user.RestPwd;
  let res;
  try {
      res = yield call(ApiClient.post, url, body);
      notification[res.data.code === "0" ? 'success' : 'error']({
        message: res.data.code === "0" ? '修改成功':'修改失败',
        description:res.message,
        duration:3
      })
    } catch (e) {
      notification.error({
        message:'修改失败',
        description:res.message,
        duration:3
      })
  }
}


export function* watchResetPassword() { // 监听调用resetPwd函数
  yield takeLatest(actionTypes.PASSWORD_RESET, resetPwd)
}