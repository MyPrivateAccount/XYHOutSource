import { handleActions } from 'redux-actions'
import clone from 'clone'
import { notification } from 'antd';
import * as actionTypes from '../constants'

const initState = {
    city: {
        list: [],
        loadingList: false,
        current: null
    },
    district: {
        list: [],
        loadingList: false,
        current: null
    },
    tradingArea: {
        list: [],
        loadingList: false,
        current: null
    },
    edit: {
        level: 1,
        parent: '',
        entity: {},
        operating: false,
        readonly: false,
        isAdd: false,
        show: false,
        title: ''
    }
}
const mapKey = {
    1: 'city',
    2: 'district',
    3: 'tradingArea'
}
let map = {};
map[actionTypes.AREA_GET_LIST_START] = (state, action) => {
    let level = action.payload.level;
    let area = state[mapKey[level]];
    let newArea = clone(area, false);
    newArea.loadingList = true;

    let newState = Object.assign({}, state);
    newState[mapKey[level]] = newArea;

    return newState;
}

map[actionTypes.AREA_GET_LIST_FINISH] = (state, action) => {
    let res = action.payload;
    let level = res.level;
    let area = state[mapKey[level]];
    let newArea = clone(area, false);
    newArea.loadingList = false;

    if (res.data.code === '0') {
        newArea.list = [...res.data.extension];
    } else {
        notification.error({
            title: '区域',
            description: '获取区域数据失败',
            duration: 3
        })
    }

    let newState = Object.assign({}, state);
    newState[mapKey[level]] = newArea;

    return newState;
}

map[actionTypes.AREA_SET_CURRENT] = (state, action) => {
    let level = action.payload.level;
    let code = action.payload.code;
    let area = state[mapKey[level]];
    let newArea = clone(area, false);

    let cur = area.list.find(x => x.code === code);
    newArea.current = clone(cur, false);



    let newState = Object.assign({}, state);
    newState[mapKey[level]] = newArea;
    if (level === 1) {
        newState.district = Object.assign({}, newState.district, { current: null, list: [] });
    }
    if (level <= 2) {
        newState.tradingArea = Object.assign({}, newState.tradingArea, { current: null, list: [] })
    }
    return newState;
}

map[actionTypes.AREA_DEL_START] = (state, action) => {
    let level = action.payload.level;
    let code = action.payload.code;
    let area = state[mapKey[level]];
    let newArea = clone(area, false);

    let cur = newArea.list.find(x => x.code === code);
    if (cur) {
        cur.operating = true;
    }

    let newState = Object.assign({}, state);
    newState[mapKey[level]] = newArea;

    return newState;
}

map[actionTypes.AREA_DEL_FINISH] = (state, action) => {
    console.log(action, '删除传过来的请求数据')
    let res = action.payload;
    let level = res.level;
    let code = res.areaCode;
    let area = state[mapKey[level]];
    let newArea = clone(area, false);

    let cur = newArea.list.find(x => x.code === code);
    if (cur) {
        cur.operating = false;
        if (res.data.code === '0') {
            cur.deleted = true;
            notification.success({
                title: '成功',
                description: '删除成功',
                duration: 3
            })
        }
    }

    let newState = Object.assign({}, state);
    newState[mapKey[level]] = newArea;

    return newState;
}

map[actionTypes.AREA_ADD] = (state, action) => {
    let level = action.payload.level;
    let parent = action.payload.parent;

    let newEdit = clone(state.edit, false);
    newEdit.entity = {
        code: '',
        level: level,
        name: '',
        abbreviation: '',
        desc: '',
        order: 0
    };
    newEdit.show = true;
    newEdit.isAdd = true;
    newEdit.level = level;
    newEdit.parent = parent;
    newEdit.readonly = false;
    newEdit.title = '添加';
    newEdit.operating = false;

    return Object.assign({}, state, { edit: newEdit });
}
map[actionTypes.AREA_EDIT] = (state, action) => {
    let level = action.payload.level;
    let code = action.payload.code;
    let area = state[mapKey[level]];

    let newEdit = clone(state.edit, false);
    newEdit.show = true;
    newEdit.isAdd = false;
    newEdit.level = level;

    newEdit.readonly = false;
    newEdit.title = '修改';
    newEdit.operating = false;

    let cur = area.list.find(x => x.code === code);

    if (cur) {
        newEdit.parent = cur.parent;
        newEdit.entity = { ...cur };
    }

    return Object.assign({}, state, { edit: newEdit });
}
map[actionTypes.AREA_VIEW] = (state, action) => {
    let level = action.payload.level;
    let code = action.payload.code;
    let area = state[mapKey[level]];

    let newEdit = clone(state.edit, false);

    newEdit.show = true;
    newEdit.isAdd = false;
    newEdit.level = level;

    newEdit.readonly = true;
    newEdit.title = '查看';
    newEdit.operating = false;

    let cur = area.list.find(x => x.code === code);

    if (cur) {
        newEdit.parent = cur.parent;
        newEdit.entity = { ...cur };
    }

    return Object.assign({}, state, { edit: newEdit });
}
map[actionTypes.AREA_CANCEL] = (state, action) => {
    let newEdit = Object.assign({}, state.edit, { show: false, operating: false })

    return Object.assign({}, state, { edit: newEdit });
}

map[actionTypes.AREA_SAVE_START] = (state, action) => {

    let newEdit = Object.assign({}, state.edit, { operating: true })
    newEdit.entity = {...action.payload.entity};
    return Object.assign({}, state, { edit: newEdit });
}

map[actionTypes.AREA_SAVE_FINISH] = (state, action) => {
    let res = action.payload;
    let entity = action.payload.entity;
    let level = action.payload.level;
    let area = state[mapKey[level]];

    let newEdit = Object.assign({}, state.edit, { operating: false })

    let newArea = Object.assign({}, area);
    let newList = [];
    let isAdd = true;
    if (res.data.code === '0') {
        newArea.list.forEach(item => {
            if (item.code === entity.code) {
                let cur = Object.assign({}, item, entity);
                newList.push(cur)
                isAdd = false;
                notification.success({
                    title: '成功',
                    description: '修改城市成功',
                    duration: 3
                })
            } else {
                newList.push(item)
            }
        });
        if (isAdd) {
            newList.push(entity);
            notification.success({
                title: '成功',
                description: '新增城市成功',
                duration: 3
            })
            newEdit.isAdd=false;
            
        }
        newEdit.show = false;
        newEdit.entity = {...entity};
    } else {
        newList = [...newArea.list];
    }
    newArea.list = newList;

    let newState = Object.assign({}, state, { edit: newEdit });
    newState[mapKey[level]] = newArea;

    return newState;
}
export default handleActions(map, initState);