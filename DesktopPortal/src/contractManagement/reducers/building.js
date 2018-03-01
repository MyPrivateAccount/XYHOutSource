import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import appAction, { NewGuid } from '../../utils/appUtils';
import moment from 'moment';


const initState = {
    buildInfo: {
        id: NewGuid(),
        buildingBasic: {},
        supportInfo: {},
        relShopInfo: {},
        projectInfo: {},
        attachInfo: {},
        buildingNoInfos: [], // 楼栋批次数据源
        ruleInfo: {}, //报备规则
        commissionPlan: '', // 佣金方案
        isDisabled: true,
    },
    dynamicBuildInfo: {
        basicInfo: {},
        supportInfo: {},
        relShopInfo: {},
        projectInfo: {},
        attachInfo: {},
        buildingNoInfos: [], // 楼栋批次数据源
        ruleInfo: {}, //报备规则
        commissionPlan: '', // 佣金方案
    },
    eidtFlagYhui: false, // 是否点击优惠政策编辑按钮默认是展示页面
    operInfo: {
        basicOperType: 'add',
        supportOperType: 'add',
        relShopOperType: 'add',
        projectOperType: 'add',
        attachPicOperType: 'add',
        attachFileOperType: 'add',
        batchBuildOperType: 'add',
        rulesOperType: 'add',
        rulesTemplateOperType: 'add',
        commissionOperType: 'add',
    },
    submitLoading: false, // 提交按钮
    buildDisplay: 'block', // 点击提交商铺后，展示view页面， 所有操作按钮隐藏。
    mySelectedRows: [], // 楼栋批次选择的数据
    templateData: [], // 模板数据
    basicloading: false,
    supportloading: false,
    relShoploading: false,
    projectloading: false,
    attachloading: false,
    batchBuildloading: false,
    rulesloading: false,
    rulesTemplateloading: false,
    commissionloading: false
};
let reducerMap = {};
// 保存各个模板的loading
reducerMap[actionTypes.LOADING_START_BASIC] = function (state, action) {
    return Object.assign({}, state, { basicloading: true });
}
reducerMap[actionTypes.LOADING_END_BASIC] = function (state, action) {
    return Object.assign({}, state, { basicloading: false });
}

reducerMap[actionTypes.LOADING_START_BATCH_BUILDING] = function (state, action) {
    return Object.assign({}, state, { batchBuildloading: true });
}
reducerMap[actionTypes.LOADING_END_BATCH_BUILDING] = function (state, action) {
    return Object.assign({}, state, { batchBuildloading: false });
}

reducerMap[actionTypes.LOADING_START_SUPPORT] = function (state, action) {
    return Object.assign({}, state, { supportloading: true });
}
reducerMap[actionTypes.LOADING_END_SUPPORT] = function (state, action) {
    return Object.assign({}, state, { supportloading: false });
}

reducerMap[actionTypes.LOADING_START_RELSHOP] = function (state, action) {
    return Object.assign({}, state, { relShoploading: true });
}
reducerMap[actionTypes.LOADING_END_RELSHOP] = function (state, action) {
    return Object.assign({}, state, { relShoploading: false });
}

reducerMap[actionTypes.LOADING_START_PROJECT] = function (state, action) {
    return Object.assign({}, state, { projectloading: true });
}
reducerMap[actionTypes.LOADING_END_PROJECT] = function (state, action) {
    return Object.assign({}, state, { projectloading: false });
}

reducerMap[actionTypes.LOADING_START_RULE] = function (state, action) {
    return Object.assign({}, state, { rulesloading: true });
}
reducerMap[actionTypes.LOADING_END_RULE] = function (state, action) {
    return Object.assign({}, state, { rulesloading: false,  rulesTemplateloading: false});
}

reducerMap[actionTypes.LOADING_START_RULE_TEMPLATE] = function (state, action) {
    return Object.assign({}, state, { rulesTemplateloading: true });
}
reducerMap[actionTypes.LOADING_END_RULE_TEMPLATE] = function (state, action) {
    return Object.assign({}, state, { rulesTemplateloading: false });
}

reducerMap[actionTypes.LOADING_START_COMMISSION] = function (state, action) {
    return Object.assign({}, state, { commissionloading: true });
}
reducerMap[actionTypes.LOADING_END_COMMISSION] = function (state, action) {
    return Object.assign({}, state, { commissionloading: false });
}

reducerMap[actionTypes.LOADING_START_ATTCH] = function (state, action) {
    return Object.assign({}, state, { attachloading: true });
}
reducerMap[actionTypes.LOADING_END_ATTCH] = function (state, action) {
    return Object.assign({}, state, { attachloading: false });
}


// 存放模板数据
reducerMap[actionTypes.SAVE_TEMPLATE_ROWS] = function (state, action) {
    let templateData = [...state.templateData];
    templateData = action.payload.templateData
    return Object.assign({}, state, { templateData: templateData });
}
// 存放我的楼栋批次数组
reducerMap[actionTypes.SAVE_SELECTED_ROWS] = function (state, action) {
    // let mySelectedRows = [...state.mySelectedRows];
    // mySelectedRows = action.payload.mySelectedRows
    // return Object.assign({}, state, { mySelectedRows: mySelectedRows });
    let dynamicBuildInfo = { ...state.dynamicBuildInfo };
    console.log(action.payload, '存放我的楼栋批次数组')
    dynamicBuildInfo.buildingNoInfos = action.payload.mySelectedRows
    return Object.assign({}, state, {dynamicBuildInfo: dynamicBuildInfo});
}

//楼盘基础信息相关
reducerMap[actionTypes.BUILDING_BASIC_ADD] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    buildInfo.id = NewGuid();
    
    let operInfo = Object.assign({}, state, { operInfo: { basicOperType: 'add' }, buildInfo: buildInfo });
    return Object.assign({}, state, { operInfo: operInfo });
}

reducerMap[actionTypes.BUILDING_BASIC_EDIT] = function (state, action) {
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'edit' });
    return Object.assign({}, state, { operInfo: operInfo });
}

reducerMap[actionTypes.BUILDING_BASIC_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    buildInfo.isDisabled = false
    if (action.payload) {
        buildInfo.buildingBasic = action.payload;
    }
    let operInfo = Object.assign({}, state.operInfo, { basicOperType: 'view' });
    return Object.assign({}, state, { operInfo: operInfo, buildInfo: buildInfo });
}

//配套信息相关
reducerMap[actionTypes.BUILDING_SUPPORT_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    if (action.payload) {
        buildInfo.supportInfo = action.payload;
    }
    let operInfo = Object.assign({}, state.operInfo, { supportOperType: 'view' });
    return Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo });
}

reducerMap[actionTypes.BUILDING_SUPPORT_EDIT] = function (state, action) {
    let operInfo = { ...state.operInfo, supportOperType: 'edit' };
    return Object.assign({}, state, { operInfo: operInfo });
}

//楼盘商铺概况信息相关
reducerMap[actionTypes.BUILDING_RELSHOP_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    if (action.payload) {
        buildInfo.relShopInfo = action.payload;
    }
    let operInfo = Object.assign({}, state.operInfo, { relShopOperType: 'view' });
    return Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo });
}
reducerMap[actionTypes.BUILDING_RELSHOP_EDIT] = function (state, action) {
    let operInfo = { ...state.operInfo, relShopOperType: 'edit' };
    return Object.assign({}, state, { operInfo: operInfo });
}
//项目简介相关
reducerMap[actionTypes.BUILDING_PROJECT_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    if (action.payload) {
        buildInfo.projectInfo = action.payload;
    }
    let operInfo = Object.assign({}, state.operInfo, { projectOperType: 'view' });
    return Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo });
}
reducerMap[actionTypes.BUILDING_PROJECT_EDIT] = function (state, action) {
    let operInfo = { ...state.operInfo, projectOperType: 'edit' };
    return Object.assign({}, state, { operInfo: operInfo });
}

// 报备规则
reducerMap[actionTypes.RULES_EDIT] = function (state, action) {
    let operInfo = Object.assign({}, state.operInfo, { rulesOperType: 'edit' });
    return Object.assign({}, state, { operInfo: operInfo });
}
reducerMap[actionTypes.RULES_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    let dynamicBuildInfo = { ...state.dynamicBuildInfo };
    let operInfo = {...state.operInfo};
    let info = {}, liberatingStart, liberatingEnd;
    console.log(buildInfo, action.payload, '展示页面的报备规则')
    if (action.payload) {
        if (action.payload.type === 'dynamic') {
            dynamicBuildInfo.ruleInfo = action.payload.body
            buildInfo.ruleInfo = action.payload.body
            console.log(dynamicBuildInfo.ruleInfo, buildInfo.ruleInfo , 888)
            operInfo = Object.assign({}, state.operInfo, { rulesTemplateOperType: 'view',rulesOperType: 'view' });
        } else {
            buildInfo.ruleInfo = action.payload.body
            console.log(buildInfo.ruleInfo, 'zzz')
            if (action.payload.template) {
                operInfo = Object.assign({}, state.operInfo, { rulesTemplateOperType: 'view' });
            } else {
                operInfo = Object.assign({}, state.operInfo, { rulesOperType: 'view' });
            }
        }
        
    } else {
        console.log(2)
        operInfo = Object.assign({}, state.operInfo, { rulesOperType: 'view' });
    }
    
    return Object.assign({}, state, { operInfo: operInfo, buildInfo: buildInfo , dynamicBuildInfo: dynamicBuildInfo});
} 

// 报备模板
reducerMap[actionTypes.RULES_TEMPLATE_EDIT] = function (state, action) {
    let operInfo = Object.assign({}, state.operInfo, { rulesTemplateOperType: 'edit' });
    return Object.assign({}, state, { operInfo: operInfo });
}
reducerMap[actionTypes.RULES_TEMPLATE_VIEW] = function (state, action) {
    let operInfo = Object.assign({}, state.operInfo, { rulesTemplateOperType: 'view'});
    return Object.assign({}, state, { operInfo: operInfo });
}

// 佣金方案
reducerMap[actionTypes.COMMISSION_EDIT] = function (state, action) {
    let operInfo = Object.assign({}, state.operInfo, { commissionOperType: 'edit' });
    return Object.assign({}, state, { operInfo: operInfo });
}
reducerMap[actionTypes.COMMISSION_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    let dynamicBuildInfo = { ...state.dynamicBuildInfo };
    // console.log(action.payload, '展示页面的佣金方案')
    if (action.payload) {
        if (action.payload.type === 'dynamic') {
            dynamicBuildInfo.commissionPlan = action.payload.body.commissionPlan
        } else {
            buildInfo.commissionPlan = action.payload.commissionPlan;
        }
    }
    let operInfo = Object.assign({}, state.operInfo, { commissionOperType: 'view' });
    return Object.assign({}, state, { operInfo: operInfo, buildInfo: buildInfo, dynamicBuildInfo: dynamicBuildInfo});
}

// 点击新增楼盘
reducerMap[actionTypes.GOTO_BUILD_PAGE] = (state, action) => {
    let buildInfo = {
        id: NewGuid(),
        buildingBasic: {},
        supportInfo: {},
        relShopInfo: {},
        projectInfo: {},
        attachInfo: {},
        buildingNoInfos: [],
        ruleInfo: {},
        commissionPlan: '',
        isDisabled: true,
    }
    let operInfo = {
        basicOperType: 'add',
        supportOperType: 'add',
        relShopOperType: 'add',
        projectOperType: 'add',
        attachPicOperType: 'add',
        attachFileOperType: 'add',
        batchBuildOperType: 'add',
        rulesOperType: 'add',
        rulesTemplateOperType: 'add',
        commissionOperType: 'add',
    }
    let newState = Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo, buildDisplay: 'block' });
    return newState;
}

// 点击进入这个楼盘
reducerMap[actionTypes.GOTO_THIS_BUILD_START] = (state, action) => {
    let newState = Object.assign({}, state, { myAdd: '' });
    return newState;
}
reducerMap[actionTypes.GOTO_THIS_BUILD_FINISH] = (state, action) => {
    // console.log(state.buildInfo, '旧值', action)
    let buildInfo = { ...state.buildInfo };
    let operInfo = { ...state.operInfo };
    let res = action.payload.data
    // console.log(res, '楼盘')
    if (res.code === '0') {
        buildInfo = res.extension
        buildInfo.buildingBasic = buildInfo.basicInfo
        buildInfo.buildingBasic.location = [buildInfo.basicInfo.city, buildInfo.basicInfo.district, buildInfo.basicInfo.area]
        buildInfo.supportInfo = buildInfo.facilitiesInfo
        buildInfo.relShopInfo = buildInfo.shopInfo
        buildInfo.projectInfo = { summary: buildInfo.summary };
        buildInfo.attachInfo = { fileList: buildInfo.fileList, attachmentList: buildInfo.attachmentList }
        if ([1, 8].includes(res.extension.examineStatus)) {
            operInfo = {
                basicOperType: 'view',
                supportOperType: 'view',
                relShopOperType: 'view',
                projectOperType: 'view',
                attachPicOperType: 'view',
                attachFileOperType: 'view',
                batchBuildOperType: 'view',
                rulesOperType: 'view',
                rulesTemplateOperType: 'view',
                commissionOperType: 'view',
            }
        } else {
            operInfo = {
                basicOperType: 'edit',
                supportOperType: 'edit',
                relShopOperType: 'edit',
                projectOperType: 'edit',
                attachPicOperType: 'edit',
                attachFileOperType: 'edit',
                batchBuildOperType: 'edit',
                rulesOperType: 'edit',
                rulesTemplateOperType: 'edit',
                commissionOperType: 'edit',
            }
        }

    }
    let newState = Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo, buildDisplay: 'block' });
    console.log(newState, '新值')
    return newState;
}


// 楼栋批次相关
reducerMap[actionTypes.BATCH_BUILDING_VIEW] = function (state, action) {
    let buildInfo = { ...state.buildInfo };
    let dynamicBuildInfo = { ...state.dynamicBuildInfo };
    // console.log(action.payload, '楼栋批次展示页面')
    if (action.payload) {
        if (action.payload.type === 'dynamic') {
            dynamicBuildInfo.buildingNoInfos = action.payload.body
        } else {
            buildInfo.buildingNoInfos = action.payload;
        }
    }
    let operInfo = Object.assign({}, state.operInfo, { batchBuildOperType: 'view' });
    return Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo, dynamicBuildInfo: dynamicBuildInfo});
}
reducerMap[actionTypes.BATCH_BUILDING_EDIT] = function (state, action) {
    // console.log(state, action, 'edit')
    let operInfo = { ...state.operInfo, batchBuildOperType: 'edit' };
    let newState = Object.assign({}, state, { operInfo: operInfo });
    // console.log(newState, 'edit')
    return newState;
}
reducerMap[actionTypes.BATCH_BUILDING_ADD] = function (state, action) {
    // console.log(state, action, 'add')
    let buildInfo = { ...state.buildInfo };
    let operInfo = { ...state.operInfo, batchBuildOperType: 'add' };
    let newState = Object.assign({}, state, { buildInfo: buildInfo, operInfo: operInfo });
    // console.log(newState, 'add')
    return newState;
}

// 点击确定批量生成楼栋模态框
reducerMap[actionTypes.COMFIRM_CREATE_BUDINGNO] = function (state, action) {
    // console.log(state, action, '批量生成')
    let buildInfo = { ...state.buildInfo };
    let buildingNoInfo = buildInfo.buildingNoInfos;
    let newArr = [];
    let { firstDate, lastDate, buildingNum, selectNum, defineStart, defineEnd, defineCode,
        numStartValue, numEndValue, engStartValue, engEndValue } = action.payload
    if (selectNum === "1") {
        for (let i = numStartValue; i <= numEndValue; i++) {
            let num = buildingNoInfo.findIndex((v, index) => {
                return v.storied === i + '栋'
            })
            if (num === -1) {
                buildingNoInfo.push({
                    key: i,
                    storied: i + '栋',
                    openDate: firstDate,
                    deliveryDate: lastDate
                })
            } else {
                buildingNoInfo[num] = {
                    key: i,
                    storied: i + '栋',
                    openDate: firstDate,
                    deliveryDate: lastDate
                }
            }
        }
    } else if (selectNum === "2") {
        for (let i = engStartValue.charCodeAt(); i <= engEndValue.charCodeAt(); i++) {
            let num = buildingNoInfo.findIndex((v, index) => {
                return v.storied === i + '栋'
            })
            if (num === -1) {
                buildingNoInfo.push({
                    key: String.fromCharCode(i),
                    storied: String.fromCharCode(i) + '栋',
                    openDate: firstDate,
                    deliveryDate: lastDate
                })
            } else {
                buildingNoInfo[num] = {
                    key: String.fromCharCode(i),
                    storied: String.fromCharCode(i) + '栋',
                    openDate: firstDate,
                    deliveryDate: lastDate
                }
            }
        }
    } else { // 自定义
        if (defineCode.indexOf('[数字]') !== -1) {
            for (let i = defineStart; i <= defineEnd; i++) {
                let num = buildingNoInfo.findIndex((v, index) => {
                    return v.storied === i + '栋'
                })
                if (num === -1) {
                    buildingNoInfo.push({
                        key: 'a' + i,
                        storied: defineCode.replace(/\[数字\]/, i),
                        openDate: firstDate,
                        deliveryDate: lastDate
                    })
                } else {
                    buildingNoInfo[num] = {
                        key: 'a' + i,
                        storied: defineCode.replace(/\[数字\]/, i),
                        openDate: firstDate,
                        deliveryDate: lastDate
                    }
                }
            }
        } else if (defineStart.indexOf('[字母]') !== -1) {
            for (let i = defineStart.charCodeAt(); i <= defineEnd.charCodeAt(); i++) {
                let num = buildingNoInfo.findIndex((v, index) => {
                    return v.storied === i + '栋'
                })
                if (num === -1) {
                    buildingNoInfo.push({
                        key: 'a' + String.fromCharCode(i),
                        storied: defineCode.replace(/\[字母\]/, String.fromCharCode(i)),
                        openDate: firstDate,
                        deliveryDate: lastDate
                    })
                } else {
                    buildingNoInfo[num] = {
                        key: 'a' + String.fromCharCode(i),
                        storied: defineCode.replace(/\[字母\]/, String.fromCharCode(i)),
                        openDate: firstDate,
                        deliveryDate: lastDate
                    }
                }
            }
        }
    }
    buildInfo = Object.assign({}, state.buildInfo, { buildingNoInfos: buildingNoInfo });
    let newState = Object.assign({}, state, { buildInfo: buildInfo });
    return newState;
}

// 改变楼栋批次时间控件
reducerMap[actionTypes.CHANGE_CELL] = function (state, action) {
    let { index, key, value } = action.payload
    let buildInfo = { ...state.buildInfo };
    let buildingNoInfo = [...buildInfo.buildingNoInfos];
    buildingNoInfo[index][key] = value;
    let newState = Object.assign({}, state, { buildingNoInfos: buildingNoInfo });
    return newState;
}

// 楼盘的照片展示
reducerMap[actionTypes.BUILDING_PIC_VIEW] = function (state, action) {
    // console.log(action.payload.filelist, 'hjshdash', state.buildInfo.attachInfo)
    const type = action.payload.type // add' 新增， 'delete'删除， 'cancel' 取消
    let buildInfo = { ...state.buildInfo }
    // let dynamicBuildInfo = { ...state.dynamicBuildInfo }
    let attachInfo, oldFileList, nowFileList
    // console.log(oldFileList, '旧数组')
    // if (action.payload.dynamic === 'dynamic') {
    //     attachInfo = { ...state.dynamicBuildInfo.attachInfo }
    //     oldFileList = [...state.dynamicBuildInfo.attachInfo.fileList]
    //     nowFileList = action.payload.filelist
    // } 
    // else {
    //     console.log(1)
        attachInfo = { ...state.buildInfo.attachInfo }
        oldFileList = [...state.buildInfo.attachInfo.fileList]
        nowFileList = action.payload.filelist
    // }
   
    let operInfo = { ...state.operInfo, attachPicOperType: 'view' };
    if (type === 'delete') {
        nowFileList.forEach((v, i) => {
            let num = oldFileList.findIndex((item) => {
                return v.uid === item.fileGuid
            })
            if (num !== -1) {
                let myIndex = num;
                oldFileList.splice(myIndex, 1)
            }
        })
    } else {
        // console.log(1)
        nowFileList.forEach((v, i) => {
            let num = oldFileList.findIndex((item) => {
                return v.fileGuid === item.fileGuid
            })
            if (num === -1) {
                if (type === 'add') {
                    console.log('add');
                    oldFileList.push(v);
                }
            } else {
                if (type === 'cancel') {
                    console.log('cancel');
                    oldFileList = oldFileList;
                }
            }
        })
    }
    // console.log(oldFileList, '?????')
    // if (action.payload.dynamic === 'dynamic') {
    //     console.log('dynamic')
    //     attachInfo = Object.assign({}, state.dynamicBuildInfo.attachInfo, { fileList: oldFileList })
    //     dynamicBuildInfo = Object.assign({}, state.dynamicBuildInfo, { attachInfo: attachInfo })
    // } 
    // else {
    //     console.log('house')
        attachInfo = Object.assign({}, state.buildInfo.attachInfo, { fileList: oldFileList })
        buildInfo = Object.assign({}, state.buildInfo, { attachInfo: attachInfo })
    // }
    let newState = Object.assign({}, state, { operInfo: operInfo, buildInfo: buildInfo });
    // console.log(newState, '展示的新数据')
    return newState;
}
reducerMap[actionTypes.BUILDING_PIC_EDIT] = function (state, action) {
    let operInfo = { ...state.operInfo, attachPicOperType: 'edit' };
    let newState = Object.assign({}, state, { operInfo: operInfo });
    return newState;
}

// 点击提交按钮
reducerMap[actionTypes.BUILD_INFO_SUBMIT_START] = function (state, action) {
    let newState = Object.assign({}, state, { submitLoading: true });
    return newState;
}
reducerMap[actionTypes.BUILD_INFO_SUBMIT_FINISH] = function (state, action) {
    let operInfo = {
        basicOperType: 'view',
        supportOperType: 'view',
        relShopOperType: 'view',
        projectOperType: 'view',
        attachPicOperType: 'view',
        attachFileOperType: 'view',
        batchBuildOperType: 'view',
        rulesOperType: 'view',
        rulesTemplateOperType: 'view',
        commissionOperType: 'view',
    }
    let newState = Object.assign({}, state, { submitLoading: false, operInfo: operInfo, buildDisplay: 'none' });
    return newState;
}



// 优惠编辑
reducerMap[actionTypes.YOU_HUI_EDIT] = (state, action) => {
    let newState = Object.assign({}, state, { eidtFlagYhui: true })
    return newState;
}
reducerMap[actionTypes.YOU_HUI_VIEW] = (state, action) => {
    let dynamicBuildInfo = {...state.dynamicBuildInfo}
    if (action.payload) {
        // console.log(action.payload.body, '优惠编辑')
        dynamicBuildInfo.relShopInfo = action.payload.body
    }
    let newState = Object.assign({}, state, { eidtFlagYhui: false, dynamicBuildInfo: dynamicBuildInfo})
    return newState;
}


// 根据房源动态的楼盘
reducerMap[actionTypes.GET_PROJECT_DYNAMIC_INFO_DETAIL_END] = function (state, action) {
    let res = action.payload
    // console.log(res, '楼盘的东西')
    let dynamicBuildInfo = { ...state.dynamicBuildInfo }
    // console.log(JSON.parse(res.extension.updateContent), '实体楼盘info')
    if (res.code === '0') {
        switch (res.extension.contentType) {
            // case 'ShopsAdd':
            // dynamicBuildInfo.ruleInfo = res.extension.contentType;break;
            case 'ReportRule':
            // console.log('ReportRule没改？？');
            dynamicBuildInfo.ruleInfo = JSON.parse(res.extension.updateContent);break;
            case 'CommissionType':
            dynamicBuildInfo.commissionPlan = JSON.parse(res.extension.updateContent).commissionPlan;break;
            case 'BuildingNo':
            dynamicBuildInfo.buildingNoInfos = JSON.parse(res.extension.updateContent);break;
            case 'DiscountPolicy':
            // console.log('DiscountPolicy')
            dynamicBuildInfo.relShopInfo = JSON.parse(res.extension.updateContent);break;
            // case 'Image':
            // let attachInfo = { ...state.dynamicBuildInfo.attachInfo }
            //     attachInfo.fileList = JSON.parse(res.extension.updateContent);break;
            // case 'Price':
            // dynamicBuildInfo.ruleInfo = JSON.parse(res.extension.updateContent);break;
            // default: // 附件
            // dynamicBuildInfo.ruleInfo = JSON.parse(res.extension.updateContent);
        }
    } 
    // console.log(dynamicBuildInfo, '现在改没有？？？')
    let newState = Object.assign({}, state, { dynamicBuildInfo: dynamicBuildInfo });
    // console.log(newState, 'aaa新值')
    return newState;
}


export default handleActions(reducerMap, initState);