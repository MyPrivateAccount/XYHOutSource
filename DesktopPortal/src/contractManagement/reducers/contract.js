import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    contractInfo:{//合同信息
        id: NewGuid(),
        contractBasicInfo:{},
        contractAttachInfo:{},

    },
    operInfo: {
        basicOperType: 'add',
        attachPicOperType: 'add',
    },
    submitLoading: false, // 提交按钮
    contractDisplay: 'block', // 点击提交商铺后，展示view页面， 所有操作按钮隐藏。
    basicloading: false,
    supportloading: false,
    attachloading: false,
}

let reduceMap = {};

export default handleActions(reduceMap, initState);



