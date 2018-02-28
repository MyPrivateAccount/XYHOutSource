import {handleActions} from 'redux-actions';
import * as actionTypes from '../constants/actionTypes';
import {globalAction} from 'redux-subspace';

const initState = {
    orgnazitionType: []
};
let reducerMap = {};

reducerMap[actionTypes.DIC_GET_PARLIST_COMPLETE] = function (state, action) {
    console.log("最外层reducer获取到字典:", action.payload);
    let orgnazitionType = [...state.orgnazitionType];
    action.payload.map((group) => {
        if (group.groupId === "ORGNAZATION_TYPE") {
            group.dictionaryDefines = group.dictionaryDefines.sort((aItem, bItem) => aItem.order - bItem.order);
            orgnazitionType = group.dictionaryDefines;
        }
    });
    return Object.assign({}, state, {
        orgnazitionType: orgnazitionType
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