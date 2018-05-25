import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const initState = {
    bswyTypes:[],//报数物业分类（字典）
    cjbgTypes:[],//成交报告分类（字典）
    tradeTypes:[],//交易类型(字典)
    projectTypes:[],//项目类型
    tradeDetailTypes:[],//详细交易类型
    ownTypes:[],//产权类型
    payTypes:[],//付款类型
    contractTypes:[],//合同类型
    sfzjjgTypes:[]
};
let reducerMap = {};
//字典数据
reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {

    let bswyTypes = [...state.bswyTypes];
    let cjbgTypes = [...state.cjbgTypes];
    let tradeTypes = [...state.tradeTypes];
    let projectTypes = [...state.projectTypes];
    let tradeDetailTypes = [...state.tradeDetailTypes];
    let ownTypes = [...state.ownTypes];
    let payTypes = [...state.payTypes];
    let contractTypes = [...state.contractTypes];
    let sfzjjgTypes = [...state.sfzjjgTypes];
    
    console.log('字典数据：', action.payload);
    action.payload.map((group) => {
        if(group.groupId === 'COMMISSION_BSWY_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            bswyTypes = group.dictionaryDefines;
     
        }
        else if(group.groupId === 'COMMISSION_CJBG_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            cjbgTypes = group.dictionaryDefines;

        }
        else if(group.groupId === 'COMMISSION_JY_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            tradeTypes = group.dictionaryDefines;

        }
        else if(group.groupId === 'COMMISSION_PROJECT_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            projectTypes = group.dictionaryDefines;

        }
        else if(group.groupId === "COMMISSION_OWN_TYPE"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            ownTypes = group.dictionaryDefines;
        }
        else if(group.groupId === "COMMISSION_PAY_TYPE"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            payTypes = group.dictionaryDefines;
        }
        else if(group.groupId === "COMMISSION_CONTRACT_TYPE"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_TRADEDETAIL_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            tradeDetailTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_SFZJJG_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            sfzjjgTypes = group.dictionaryDefines;
        }
        
  

    });
    return Object.assign({}, state, {
        bswyTypes:bswyTypes,
        cjbgTypes:cjbgTypes,
        tradeTypes:tradeTypes,
        projectTypes:projectTypes,
        tradeDetailTypes:tradeDetailTypes,
        ownTypes:ownTypes,
        payTypes:payTypes,
        contractTypes:contractTypes,
        sfzjjgTypes:sfzjjgTypes
    });
}




export default handleActions(reducerMap, initState);