import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';


const initState = {
    id: 0,
    zIndex: 10,
    list: []
};
let windowsReducerMap = {};
windowsReducerMap[actionTypes.WINDOWS_OPEN_NEW] = (state, action) => {
    let wndInstance = Object.assign({}, action.payload);
    wndInstance.id = state.id + 1;
    wndInstance.zIndex = state.zIndex + 1;
    wndInstance.isMaximized = false;
    wndInstance.isMinimized = false;
    wndInstance.isWindowFocused = true;
    wndInstance.isRestoring = false;

    let wndList = state.list.slice();
    wndList.push(wndInstance);
    return Object.assign({}, state, { id: wndInstance.id, zIndex: wndInstance.zIndex, list: wndList });
}

function activeWindow(id, state){
    let activeList = state.list.filter(wnd => wnd.isWindowFocused === true);
    activeList.forEach(wnd => {
        wnd.isWindowFocused = false;
    });
    let cur = state.list.find(wnd => wnd.id === id);
    if (cur) {

        if (cur.zIndex !== state.zIndex) {
            cur.zIndex = state.zIndex + 1;
            state.zIndex = cur.zIndex;
        }
        cur.isWindowFocused = true;
    }
}

windowsReducerMap[actionTypes.WINDOWS_ACTIVE] = (state, action) => {
    let id = action.payload;

    activeWindow(id,state);

    let wndList = state.list.slice();
    return Object.assign({}, state, { zIndex: state.zIndex, list: wndList });
}

function getMaxZIndex(list) {
    let zIndex = 0;
    list.forEach(wnd => {
        if (wnd.zIndex > zIndex)
            zIndex = wnd.zIndex;
    });
    return zIndex;
}

windowsReducerMap[actionTypes.WINDOWS_CLOSE] = (state, action) => {
    let id = action.payload;
    let wndList = state.list.filter(wnd => wnd.id !== id);
    let zIndex = initState.zIndex;
    if (wndList.length > 0) {
        zIndex = getMaxZIndex(wndList);
    }
    return Object.assign({}, state, { zIndex: zIndex, list: wndList });
}

windowsReducerMap[actionTypes.WINDOWS_RESTORE] = (state, action) => {
    let id = action.payload;
    let cur = state.list.find(wnd => wnd.id === id);
    if (cur) {
        if (cur.isMinimized ||  cur.isMaximized) {
            cur.isMaximized = false;
            cur.isMinimized = false;

            activeWindow(id,state);
        }
    }

    let wndList = state.list.slice();
    return Object.assign({}, state, { list: wndList });
}

windowsReducerMap[actionTypes.WINDOWS_MAXIMIZE] = (state, action) => {
    let id = action.payload;
    let cur = state.list.find(wnd => wnd.id === id);
    if (cur) {
        if (!cur.isMaximized) {
            cur.isMaximized = true;
         
        }
        cur.isMinimized = false;
    }

    let wndList = state.list.slice();
    return Object.assign({}, state, { list: wndList });
}

windowsReducerMap[actionTypes.WINDOWS_MINIMIZE] = (state, action) => {
    let id = action.payload;
    let cur = state.list.find(wnd => wnd.id === id);
    if (cur) {
        if (!cur.isMinimized) {
            cur.isMinimized = true;
        }
    }

    let wndList = state.list.slice();
    return Object.assign({}, state, { list: wndList });
}

windowsReducerMap[actionTypes.WINDOWS_RESTORE_X] = (state, action) => {
    let id = action.payload.id;
    let cur = state.list.find(wnd => wnd.id === id);
    if (cur ) {
        if(cur.isMinimized){
            cur.isRestoring = action.payload.start;
        }else{
            activeWindow(id,state);
        }
       
    }

    let wndList = state.list.slice();
    return Object.assign({}, state, { list: wndList });
}

windowsReducerMap[actionTypes.WINDOWS_OPENORACTIVE] = (state, action)=>{
    let type = action.payload.type;
    let tool = action.payload.tool;
    if(type){
        let cur = state.list.find(wnd=> wnd.type === type && wnd.tool === tool );
        if(cur){
            
            //激活
            return windowsReducerMap[actionTypes.WINDOWS_RESTORE_X](state,{type: actionTypes.WINDOWS_RESTORE_X,payload:{ id:cur.id, start:true}});
        }
    }
    
     return   windowsReducerMap[actionTypes.WINDOWS_OPEN_NEW](state,action);
    

}

export default handleActions(windowsReducerMap, initState)