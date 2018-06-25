import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const initState = {
    operInfo: {objType: '', operType: '', snackbar: ''},
    //成交报告基础信息
    bswyTypes:[],//报数物业分类（字典）
    cjbgTypes:[],//成交报告分类（字典）
    tradeTypes:[],//交易类型(字典)
    projectTypes:[],//项目类型
    tradeDetailTypes:[],//详细交易类型
    ownTypes:[],//产权类型
    payTypes:[],//付款类型
    contractTypes:[],//合同类型
    sfzjjgTypes:[],
    ////成交物业信息
    wyCqTypes:[],//城区
    wyPqTypes:[],//片区
    wyWylxTypes:[],//物业类型
    wyKjlxTypes:[],//空间类型
    wyLlTypes:[],//楼龄
    wyZxTypes:[],//装修状况
    wyZxndTypes:[],//装修年代
    wyJjTypes:[],//家具
    wyCxTypes:[],//朝向
    //业主
    yzChtscTypes:[],
    //客户
    khKhxzTypes:[],
    //过户
    ghDknxTypes:[],
    //业绩分配
    sfdxTypes:[],//收付对象
    //成交状态
    cjTypes:[],
    //过户状态
    ghTypes:[],
    //客户来源
    khTypes:[],
    //成交报告审核状态
    spTypes:[],
    //职位等级
    zwTypes:[]
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
    ////
    let wyCqTypes = [...state.wyCqTypes];
    let wyPqTypes = [...state.wyPqTypes];
    let wyWylxTypes = [...state.wyWylxTypes];
    let wyKjlxTypes = [...state.wyKjlxTypes];
    let wyLlTypes = [...state.wyLlTypes];
    let wyZxTypes = [...state.wyZxTypes];
    let wyZxndTypes = [...state.wyZxndTypes];
    let wyJjTypes = [...state.wyJjTypes];
    let wyCxTypes = [...state.wyCxTypes];
    //
    let yzChtscTypes = [...state.yzChtscTypes];
    //
    let khKhxzTypes = [...state.khKhxzTypes];
    //
    let ghDknxTypes = [...state.ghDknxTypes];
    //
    let sfdxTypes = [...state.sfdxTypes];
    //
    let cjTypes = [...state.cjTypes];
    let ghTypes = [...state.ghTypes];
    let khTypes = [...state.khTypes];
    let spTypes = [...state.spTypes];

    let zwTypes = [...state.zwTypes];
    
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
        else if(group.groupId === 'COMMISSION_WY_CQ'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyCqTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_PQ'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyPqTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_WYLX'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyWylxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_KJLX'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyKjlxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_ZXZK'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyZxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_ZXND'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyZxndTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_ZXJJ'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyJjTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_WY_WYCX'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            wyCxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_YZ_QHTSC'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            yzChtscTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_KH_KHXZ'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            khKhxzTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_GH_DKNX'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            ghDknxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_FP_SFDX'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            sfdxTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_DEAL_STATS'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            cjTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_GH_TYPES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            ghTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_KHINFO_SOURCE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            khTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_RP_STATE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            spTypes = group.dictionaryDefines;
        }
        else if(group.groupId === 'COMMISSION_ZW_LEVEL'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            zwTypes = group.dictionaryDefines;
        }
  

    });
    return Object.assign({}, state, {
        operInfo:{operType:'DIC_GET_PARLIST_COMPLETE'},
        bswyTypes:bswyTypes,
        cjbgTypes:cjbgTypes,
        tradeTypes:tradeTypes,
        projectTypes:projectTypes,
        tradeDetailTypes:tradeDetailTypes,
        ownTypes:ownTypes,
        payTypes:payTypes,
        contractTypes:contractTypes,
        sfzjjgTypes:sfzjjgTypes,
        wyCqTypes:wyCqTypes,//城区
        wyPqTypes:wyPqTypes,//片区
        wyWylxTypes:wyWylxTypes,//物业类型
        wyKjlxTypes:wyKjlxTypes,//空间类型
        wyLlTypes:wyLlTypes,//楼龄
        wyZxTypes:wyZxTypes,//装修状况
        wyZxndTypes:wyZxndTypes,//装修年代
        wyJjTypes:wyJjTypes,//家具
        wyCxTypes:wyCxTypes,//朝向
        yzChtscTypes:yzChtscTypes,
        khKhxzTypes:khKhxzTypes,
        ghDknxTypes:ghDknxTypes,
        sfdxTypes:sfdxTypes,
        cjTypes:cjTypes,
        khTypes:khTypes,
        ghTypes:ghTypes,
        spTypes:spTypes,
        zwTypes:zwTypes
    });
}




export default handleActions(reducerMap, initState);