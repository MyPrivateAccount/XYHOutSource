import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const action = appAction('PrivilegeManagerIndex')

const initState = {
    saleStatus: [],
    saleModel: [],
    shopsTypes: [],
    tradePlannings: [],
    areaList: [],
    shopSaleStatus : [],
    shopTword : [],
    shopStatus : [],
    shopLease : [],
    xkSeller: [],
    xkSellerType: [],
    shopDepositype: [],
    photoCategories: []
};
let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let saleStatus = [], 
    saleModel = [], 
    shopsTypes = [], 
    tradePlannings = [],
    shopSaleStatus = [],
    shopTword = [],
    shopStatus = [],
    shopLease = [],
    xkSeller = [],
    shopDepositype = [],
    photoCategories = [],
    xkSellerType = [];
    action.payload.map((group) => {
        if (group.groupId === "PROJECT_SALE_STATUS") {
            saleStatus = group.dictionaryDefines;
        }
        else if (group.groupId === "SALE_MODE") {
            saleModel = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_CATEGORY") {
            shopsTypes = group.dictionaryDefines;
        }
        else if (group.groupId === "TRADE_MIXPLANNING") {
            tradePlannings = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_TOWARD") {
            shopTword = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_STATUS") {
            shopStatus = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_SALE_STATUS") {
            shopSaleStatus = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_LEASE_PAYMENTTIME") {
            shopLease = group.dictionaryDefines;
        }
        else if (group.groupId === "XK_SELLER") {
            xkSeller = group.dictionaryDefines;
        } 
        else if(group.groupId==='XK_SELLER_TYPE') {
            xkSellerType = group.dictionaryDefines;
        }
        else if(group.groupId==='SHOP_DEPOSITTYPE') {
            shopDepositype = group.dictionaryDefines;
        }
        else if(group.groupId==='PHOTO_CATEGORIES') {
            photoCategories = group.dictionaryDefines;
        }
    });
    return Object.assign({}, state, { saleStatus: saleStatus, saleModel: saleModel, shopsTypes: shopsTypes, tradePlannings: tradePlannings,
        shopLease: shopLease, shopSaleStatus: shopSaleStatus, shopStatus: shopStatus, shopTword: shopTword, xkSeller: xkSeller,xkSellerType:xkSellerType,
        shopDepositype: shopDepositype, photoCategories:photoCategories});
}
reducerMap[actionTypes.DIC_GET_AREA_COMPLETE] = function (state, action) {
    let allAreas = action.payload;
    let areaList = [];
    let firstLevelAreas = allAreas.filter((area) => area.level === "1");
    let secondLevelAreas = allAreas.filter((area) => area.level === "2");
    let thirdLevelAreas = allAreas.filter((area) => area.level === "3");
    firstLevelAreas.map((firstChild) => {
        let firstLevelNode = { value: firstChild.code, label: firstChild.name, children: [] };
        //二级地区
        let secondChilds = secondLevelAreas.filter((child) => child.parentId === firstChild.code);
        secondChilds.map((secondChild) => {
            let secondLevelNode = { value: secondChild.code, label: secondChild.name, children: [] };
            //三级地区
            let thirdChilds = thirdLevelAreas.filter((child) => child.parentId == secondChild.code);
            thirdChilds.map((thirdChild) => {
                secondLevelNode.children.push({ value: thirdChild.code, label: thirdChild.name });
            });
            firstLevelNode.children.push(secondLevelNode);
        });
        areaList.push(firstLevelNode);
    });
    console.log("地区json：", areaList);
    return Object.assign({}, state, { areaList: areaList });
}
export default handleActions(reducerMap, initState);