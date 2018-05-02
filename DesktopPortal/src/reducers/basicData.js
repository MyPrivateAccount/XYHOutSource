import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';
import {globalAction} from 'redux-subspace';

const initState = {
    orgnazitionType: [],

    contractAttachTypes:[],//合同附件分类（字典）
    contractCategories:[],//合同类型分类（字典）
    firstPartyCatogories:[],//甲方类型
    commissionCatogories:[],
    contractProjectCatogories:[],
    settleAccountsCatogories:[],
};
let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    console.log("最外层reducer获取到字典:", action.payload);
    let orgnazitionType = [...state.orgnazitionType];
    let contractAttachTypes = [...state.contractAttachTypes];
    let contractCategories = [...state.contractCategories];
    let firstPartyCatogories = [...state.firstPartyCatogories];
    let commissionCatogories = [...state.commissionCatogories];
    let contractProjectCatogories = [...state.contractProjectCatogories];
    let contractSettleAccountTypes = [...state.settleAccountsCatogories];
    action.payload.map((group) => {
        if (group.groupId === "ORGNAZATION_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            orgnazitionType = group.dictionaryDefines;
        }
        else if(group.groupId === 'CONTRACT_ATTACHMENT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractAttachTypes = group.dictionaryDefines;
     
        }
        else if(group.groupId === 'CONTRACT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractCategories = group.dictionaryDefines;

        }
        else if(group.groupId === 'FIRST_PARTT_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            firstPartyCatogories = group.dictionaryDefines;

        }
        else if(group.groupId === 'COMMISSION_CATEGORIES'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            commissionCatogories = group.dictionaryDefines;

        }
        else if(group.groupId === 'XK_SELLER_TYPE'){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractProjectCatogories = group.dictionaryDefines;

        }
        else if(group.groupId === "CONTRACT_SETTLEACCOUNTS"){
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            contractSettleAccountTypes = group.dictionaryDefines;
        }
    });
    return Object.assign({}, state, {
        orgnazitionType: orgnazitionType,
        //合同相关
        contractAttachTypes:contractAttachTypes,
        contractCategories:contractCategories,
        firstPartyCatogories:firstPartyCatogories,
        commissionCatogories:commissionCatogories,
        contractProjectCatogories:contractProjectCatogories,
        settleAccountsCatogories:contractSettleAccountTypes,
    });
}
reducerMap[actionTypes.DIC_GET_AREA_COMPLETE] = function (state, action) {
    let allAreas = action.payload;
    let areaList = [];
    let firstLevelAreas = allAreas.filter((area) => area.level === "1");
    let secondLevelAreas = allAreas.filter((area) => area.level === "2");
    let thirdLevelAreas = allAreas.filter((area) => area.level === "3");
    firstLevelAreas.map((firstChild) => {
        let firstLevelNode = {value: firstChild.code, label: firstChild.name, children: []};
        //二级地区
        let secondChilds = secondLevelAreas.filter((child) => child.parentId === firstChild.code);
        secondChilds.map((secondChild) => {
            let secondLevelNode = {value: secondChild.code, label: secondChild.name, children: []};
            //三级地区
            let thirdChilds = thirdLevelAreas.filter((child) => child.parentId == secondChild.code);
            thirdChilds.map((thirdChild) => {
                secondLevelNode.children.push({value: thirdChild.code, label: thirdChild.name});
            });
            firstLevelNode.children.push(secondLevelNode);
        });
        areaList.push(firstLevelNode);
    });
    //console.log("地区json：", areaList);
    return Object.assign({}, state, {areaList: areaList});
}
export default handleActions(reducerMap, initState);