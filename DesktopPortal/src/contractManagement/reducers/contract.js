import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import { notification } from 'antd';
import moment from 'moment'

const initState = {
    contractInfo:{//合同信息
        basicInfo:{},
    },
}
