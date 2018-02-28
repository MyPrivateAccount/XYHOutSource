import { handleActions } from 'redux-actions'
import { notification } from 'antd';
import clone from 'clone';
import * as actionTypes from '../constants'

const initState={
    currentGroup:null, //当前组
    loadingGroup:false,
    groupList:[], //参数组列表
    parameterList:{
        group:'',
        list:[],
        pageSize: 15,
        pageIndex:1,
        pageCount:1
    }, 
    loadingParameter:false,
    editGroup:{
        entity:{},
        show:false,
        isAdd:false,
        readonly:false,
        title:'',
        operating:false
    },
    editDic:{
        entity:{},
        show:false,
        isAdd:false,
        readonly:false,
        operating:false
    }
}

let map={};

map[actionTypes.DIC_ADD_GROUP] = (state,action)=>{
    let editGroup = {...state.editGroup};
    editGroup.entity={
        id:'',
        name:'',
        desc:'',
        valueType:0,
        // hasExt1:false,
        // hasExt2:false,
        ext1Desc:'',
        ext2Desc:'',
        deleted:false
    };
    editGroup.show=true;
    editGroup.isAdd=true;
    editGroup.readonly = false;
    editGroup.title='添加字典组';
    
    return Object.assign({},state, {editGroup:editGroup} );
}
map[actionTypes.DIC_EDIT_GROUP] = (state,action)=>{
    let editGroup = {...state.editGroup};
    editGroup.entity=clone(action.payload, false);
    editGroup.show=true;
    editGroup.isAdd=false;
    editGroup.readonly = false;
    editGroup.title='修改字典组';
    
    return Object.assign({},state, {editGroup:editGroup} ); 
}
map[actionTypes.DIC_CANCEL_EDIT] = (state, action)=>{
    let editGroup = {...state.editGroup};
    editGroup.show =false;
    return Object.assign({},state, {editGroup:editGroup} );
}
map[actionTypes.DIC_GET_GROUPLIST_START] = (state, action)=>{
    return Object.assign({}, state, {loadingGroup:true});
}
map[actionTypes.DIC_GET_GROUPLIST_FINISH] = (state, action)=>{
    let res = action.payload;
    let newState = Object.assign({},state);
    newState.loadingGroup=false;
    if(res.code === '0'){
        newState.groupList = res.extension;
    }else{
        notification.error({
            message:'获取字典组失败',
            description:res.message,
            duration:3
        })
    }
    return newState;
}
map[actionTypes.DIC_SET_CURRENT_GROUP] = (state, action)=>{
    let key = action.payload;
    let newState = Object.assign({},state);
    let group = state.groupList.find(x=>x.id === key);
    if(group!== state.currentGroup){
        newState.currentGroup = group;
        newState.parameterList = {...newState.parameterList};
        newState.parameterList.group = group.id;
        newState.parameterList.pageIndex =1;
        newState.parameterList.pageCount=1;
        newState.parameterList.list = [];
    }
    return newState;
}
map[actionTypes.DIC_DEL_GROUP_START] = (state, action)=>{
    let op = action.payload;
    let newList =[];
    state.groupList.forEach((item)=>{
        if(item.code === op.code){
            newList.push(Object.assign({},item,{operating:true}));
        }else{
            newList.push(item);
        }
    });
    
    return Object.assign({},state, {groupList:newList})
}
map[actionTypes.DIC_DEL_GROUP_FINISH] = (state, action)=>{
    let res = action.payload;
	let newList =[];
    state.groupList.forEach((item)=>{
        if(item.id === res.entity.id){
            let newItem = {};
            if(res.data.code === '0'){
                newItem=Object.assign({},item,{operating:false, deleted:true});
                notification.success({
                    message:'删除成功',
                    description:res.message,
                    duration:3
                })
            }else{
                newItem=Object.assign({},item,{operating:false, deleted:false});
                notification.error({
                    message:'删除失败',
                    description:res.message,
                    duration:3
                })
            }
            newList.push(newItem)
        }else{
            item.operating = false;
            newList.push(item);
        }
    });

    

    return Object.assign({},state, {groupList:newList});
}

map[actionTypes.DIC_SAVE_GROUP_START] = (state,action)=>{
    let editGroup = {...state.editGroup};

    editGroup.operating = true;
    editGroup.readonly=true;
    editGroup.entity = {...action.payload}
    return Object.assign({},state, {editGroup:editGroup} ); 
}
map[actionTypes.DIC_SAVE_GROUP_FINISH] = (state, action)=>{
    let editGroup = {...state.editGroup};
    editGroup.operating = false;
    editGroup.readonly=false;
	editGroup.show = false;
    let res= action.payload.data;
    let entity = action.payload.entity;
    let newList = [];
    let isModify = false;
		let cur = null;
    state.groupList.forEach(item=>{
        if(item.id === entity.id){
            isModify= true;
            if(res.code === '0'){
                cur = Object.assign({}, item, entity);
                newList.push(cur)
                notification.success({
                    message:'修改字典组成功',
                    description:res.message,
                    duration:3
                })
            }
        }else{
            newList.push(item)
        }
    });
    if(!isModify){
        cur = clone(entity, false);
        newList.push(cur);
        editGroup.entity = {...cur};
        notification.success({
            message:'添加字典组成功',
            description:res.message,
            duration:3
        })
    }


    return Object.assign({},state, {editGroup:editGroup, groupList: newList,currentGroup: cur } ); 
}


//字典值
map[actionTypes.DIC_GET_PARLIST_START] = (state, action)=>{
    return Object.assign({}, state, {loadingParameter:true});
}
map[actionTypes.DIC_GET_PARLIST_FINISH] = (state, action)=>{
    console.log(action, state, '获取字典项!!!!')
    let res = action.payload;
    let newState = Object.assign({},state);
    newState.loadingParameter=false;
    if(res.code === '0'){
        newState.parameterList = Object.assign({}, newState.parameterList, {list: res.extension});
    }else{
        notification.error({
            message:'获取字典项失败',
            description:res.message,
            duration:3
        })
    }
    return newState;
}


map[actionTypes.DIC_SAVE_VALUE_START] = (state,action)=>{
    let editDic = {...state.editDic};

    editDic.operating = true;
    editDic.readonly=true;
    editDic.entity = {...action.payload}
    return Object.assign({},state, {editDic:editDic} ); 
}
map[actionTypes.DIC_SAVE_VALUE_FINISH] = (state,action)=>{
    console.log(state, action, '请求传过来的数据')
    let editDic = {...state.editDic};
    editDic.operating = false;
    editDic.readonly=false;
    
    let res= action.payload;
    let entity = action.payload.entity;
    
    let newList = [];
    let isModify =false;
    let cur = null;
    state.parameterList.list.forEach(item => {
        if(item.value === entity.value){
            isModify=true;
            if( res.data.code === "0"){
                cur = Object.assign({},item, entity);
                newList.push(cur);
                editDic.show = false;
                notification.success({
                    message:'成功',
                    description:res.message,
                    duration:3
                })                
            }else{

            }
        }else{
            newList.push(item)
        }
    });
    if(!isModify){
        cur = clone(entity,false);
        newList.push(cur);
        editDic.entity = {...cur};
        editDic.show = false;
    }


    return Object.assign({},state, {editDic:editDic, parameterList: {...state.parameterList, list: newList}} ); 
}



map[actionTypes.DIC_ADD_VALUE] = (state,action)=>{
    let editDic = {...state.editDic};
    editDic.entity={
        groupId: action.payload,
        name:'',
        value:'',
        desc:'',
        order:0,
        ext1:'',
        ext2:'',
        deleted:false
    };
    console.log(editDic.entity, 44)
    editDic.show=true;
    editDic.isAdd=true;
    editDic.readonly = false;
    editDic.title='添加字典项';
    
    return Object.assign({},state, {editDic:editDic} );
}
map[actionTypes.DIC_EDIT_VALUE] = (state,action)=>{
    let editDic = {...state.editDic};
    editDic.entity=clone(action.payload, false);
    editDic.show=true;
    editDic.isAdd=false;
    editDic.readonly = false;
    editDic.title='修改字典项';
    
    return Object.assign({},state, {editDic:editDic} ); 
}
map[actionTypes.DIC_CANCEL_EDIT_VALUE] = (state, action)=>{
    let editDic = {...state.editDic};
    editDic.show =false;
    return Object.assign({},state, {editDic:editDic} );
}

// 删除字典定义
map[actionTypes.DIC_DEL_VALUE_START] = (state, action)=>{
    let op = action.payload;
    let newList =[];
    state.parameterList.list.map((item)=>{
        if(item.value === op.value){
            newList.push(Object.assign({},item,{operating:true}));
        }else{
            newList.push(item);
        }
    });
    
    return Object.assign({},state, {parameterList: {...state.parameterList, list:newList}})
}
map[actionTypes.DIC_DEL_VALUE_FINISH] = (state, action)=>{
    let res = action.payload;
    let newList =[];
    state.parameterList.list.map((item)=>{
        if(item.value === res.entity.value){
            let newItem = {};
            if(res.data.code === '0'){
                newItem=Object.assign({},item,{operating:false, isDeleted:true});
                notification.success({
                    message:'禁用成功',
                    description:res.message,
                    duration:3
                })
            }else{
                newItem=Object.assign({},item,{operating:false, isDeleted:false});
                notification.error({
                    message:'禁用失败',
                    description:res.message,
                    duration:3
                })
            }
            newList.push(newItem)
        }else{
            item.operating = false;
            newList.push(item);
        }
    });

    
    let newState = Object.assign({},state, {parameterList: {...state.parameterList, list:newList}})
    return newState
}

map[actionTypes.START_DIC_VALUE_START] = (state, action)=>{
    let op = action.payload;
    let newList =[];
    state.parameterList.list.map((item)=>{
        if(item.value === op.value){
            newList.push(Object.assign({},item,{operating:true}));
        }else{
            newList.push(item);
        }
    });
    
    return Object.assign({},state, {parameterList: {...state.parameterList, list:newList}})
}
map[actionTypes.START_DIC_VALUE_END] = (state, action)=>{
    let res = action.payload;
    let newList =[];
    state.parameterList.list.map((item)=>{
        if(item.value === res.entity.value){
            let newItem = {};
            if(res.data.code === '0'){
                newItem=Object.assign({},item,{operating:false, isDeleted:false});
                notification.success({
                    message:'启用成功',
                    description:res.message,
                    duration:3
                })
            }else{
                newItem=Object.assign({},item,{operating:false, isDeleted:true});
                notification.error({
                    message:'启用失败',
                    description:res.message,
                    duration:3
                })
            }
            newList.push(newItem)
        }else{
            item.operating = false;
            newList.push(item);
        }
    });
    let newState = Object.assign({},state, {parameterList: {...state.parameterList, list:newList}})
    return newState
}



export default handleActions(map,initState);