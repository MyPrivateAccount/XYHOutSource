import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction from '../../utils/appUtils';

const initState = {
    saleStatus: [],
    saleModel: [],
    shopsTypes: [],
    tradePlannings: [],
    searchPriceXYH: [],
    searchPriceZYW: [],
    serachAreaZYW: [],
    areaList: [],
    shopSaleStatus: [],
};
let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    let saleStatus = [...state.saleStatus], saleModel = [...state.saleModel], shopsTypes = [...state.shopsTypes], tradePlannings = [...state.tradePlannings];
    let searchPriceXYH = [...state.searchPriceXYH], searchPriceZYW = [...state.searchPriceZYW], serachArea = [...state.serachAreaZYW];
    let shopSaleStatus = [...state.shopSaleStatus];
    action.payload.map((group) => {
        if (group.groupId === "SEARCH_PRICE_XYH") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            searchPriceXYH = group.dictionaryDefines;
        }
        else if (group.groupId === "SEARCH_PRICE_ZYW") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            searchPriceZYW = group.dictionaryDefines;
        }
        else if (group.groupId === "PROJECT_SALE_STATUS") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            saleStatus = group.dictionaryDefines;
        }
        else if (group.groupId === "SALE_MODE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            saleModel = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_CATEGORY") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            shopsTypes = group.dictionaryDefines;
        }
        else if (group.groupId === "TRADE_MIXPLANNING") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            tradePlannings = group.dictionaryDefines;
        }
        else if (group.groupId === "SEARCH_AREA") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            serachArea = group.dictionaryDefines;
        }
        else if (group.groupId === "SHOP_SALE_STATUS") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            shopSaleStatus = group.dictionaryDefines;
        }
    });
    return Object.assign({}, state, {
        searchPriceXYH: searchPriceXYH,
        searchPriceZYW: searchPriceZYW,
        serachAreaZYW: serachArea, shopSaleStatus: shopSaleStatus,
        saleStatus: saleStatus, saleModel: saleModel, shopsTypes: shopsTypes, tradePlannings: tradePlannings
    });
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
    //console.log("地区json：", areaList);
    return Object.assign({}, state, { areaList: areaList });
}
export default handleActions(reducerMap, initState);