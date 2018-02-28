import { LOAD_SUBSCRIPTIONS_SUCCESS } from '../constants/actionTypes';
import { SESSION_TERMINATED, USER_EXPIRED } from 'redux-oidc';
import {handleActions} from 'redux-actions';

const initialState = {
  channels: []
};

let sessionReducerMap = {};
sessionReducerMap[SESSION_TERMINATED] = (state, action)=>{
    return Object.assign({}, {...state}, {channels: []});
}
sessionReducerMap[USER_EXPIRED] = (state,action)=>{
    return Object.assign({}, {...state}, {channels: []});
}
sessionReducerMap[LOAD_SUBSCRIPTIONS_SUCCESS] = (state,action)=>{
    return Object.assign({}, {...state}, {channels: action.payload});
}
export  default  handleActions(sessionReducerMap, initialState)