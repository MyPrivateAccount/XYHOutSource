import {handleActions} from 'redux-actions'
import * as actionTypes from '../constants'

const initState = {
    submiting: false,
    firstLevel: {
        list: [],
        loadingList: false,
        current: null
    },
    secondLevel: {
        list: [],
        loadingList: false,
        current: null
    },
    thirdLevel: {
        list: [],
        loadingList: false,
        current: null
    }
}
let map = {};
map[actionTypes.PLANNING_GET_LIST_END] = (state, action) => {
    let {firstLevel, secondLevel, thirdLevel} = state;
    firstLevel.loadingList = false;
    secondLevel.loadingList = false;
    thirdLevel.loadingList = false;
    let result = action.payload;
    if (result.level == '1') {
        firstLevel.list = result.extension || [];
        secondLevel.list = [];
        thirdLevel.list = [];
        secondLevel.current = null;
        thirdLevel.current = null;
    } else if (result.level == '2') {
        secondLevel.list = result.extension || [];
        secondLevel.current = null;
        thirdLevel.list = [];
        thirdLevel.current = null;
    } else if (result.level == '3') {
        thirdLevel.list = result.extension || [];
    }
    return Object.assign({}, state, {firstLevel: firstLevel, secondLevel: secondLevel, thirdLevel: thirdLevel, submiting: false});
}
map[actionTypes.PLANNING_SAVE_END] = (state, action) => {
    let {firstLevel, secondLevel, thirdLevel} = state;
    firstLevel.loadingList = false;
    secondLevel.loadingList = false;
    thirdLevel.loadingList = false;
    let result = action.payload;//op:1 添加 2 修改
    if (result.level == '1') {
        if (result.op === 1) {
            firstLevel.list.push(action.payload);
        } else {
            let index = firstLevel.list.findIndex(item => item.id === result.id);
            if (index != -1) {
                delete result.op;
                firstLevel.list[index] = result;
            }
        }
    } else if (result.level == '2') {
        if (result.op === 1) {
            secondLevel.list.push(action.payload);
        } else {
            let index = secondLevel.list.findIndex(item => item.id === result.id);
            if (index != -1) {
                delete result.op;
                secondLevel.list[index] = result;
            }
        }
    } else if (result.level == '3') {
        if (result.op === 1) {
            thirdLevel.list.push(action.payload);
        } else {
            let index = thirdLevel.list.findIndex(item => item.id === result.id);
            if (index != -1) {
                delete result.op;
                thirdLevel.list[index] = result;
            }
        }
    }
    return Object.assign({}, state, {firstLevel: firstLevel, secondLevel: secondLevel, thirdLevel: thirdLevel, submiting: false})
};

map[actionTypes.PLANNING_REMOVE] = (state, action) => {
    let {firstLevel, secondLevel, thirdLevel} = state;
    let entity = action.payload;
    if (entity.level == '1') {
        let index = firstLevel.list.findIndex(item => item.id === entity.id);
        if (index != -1) {
            firstLevel.list.splice(index, 1);
            if (firstLevel.current.id === entity.id) {
                secondLevel.list = [];
                secondLevel.current = null;
                thirdLevel.list = [];
                thirdLevel.current = null;
            }
        }
    } else if (entity.level == '2') {
        let index = secondLevel.list.findIndex(item => item.id === entity.id);
        if (index != -1) {
            secondLevel.list.splice(index, 1);
            if (secondLevel.current.id === entity.id) {
                thirdLevel.list = [];
                thirdLevel.current = null;
            }
        }
    } else if (entity.level == '3') {
        let index = thirdLevel.list.findIndex(item => item.id === entity.id);
        if (index != -1) {
            thirdLevel.list.splice(index, 1);
        }
    }
    return Object.assign({}, state, {firstLevel: firstLevel, secondLevel: secondLevel, thirdLevel: thirdLevel})
}
//切换选择
map[actionTypes.PLANNING_SELECTED] = (state, action) => {
    let {firstLevel, secondLevel, thirdLevel} = state;
    let selectedPlanning = action.payload;
    if (selectedPlanning.level === "1") {
        firstLevel.current = selectedPlanning;
    } else if (selectedPlanning.level === "2") {
        secondLevel.current = selectedPlanning;
    } else if (selectedPlanning.level === "3") {
        thirdLevel.current = selectedPlanning
    }
    return Object.assign({}, state, {firstLevel: firstLevel, secondLevel: secondLevel, thirdLevel: thirdLevel})
}
//更改加载状态
map[actionTypes.PLANNING_CHANGE_LOADING] = (state, action) => {
    let {firstLevel, secondLevel, thirdLevel, submiting} = state;
    let {level, status} = action.payload;
    if (level === "1") {
        firstLevel.loadingList = status;
    } else if (level === "2") {
        secondLevel.loadingList = status;
    } else if (level === "3") {
        thirdLevel.loadingList = status
    } else {
        submiting = status;
    }
    return Object.assign({}, state, {firstLevel: firstLevel, secondLevel: secondLevel, thirdLevel: thirdLevel, submiting: submiting})
}

export default handleActions(map, initState);