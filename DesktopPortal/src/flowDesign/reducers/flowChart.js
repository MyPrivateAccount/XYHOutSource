import { handleActions } from 'redux-actions';
import * as actionTypes from '../constants/actionType';
import NwfCommand from '../constants/commandDefine';
import { notification } from 'antd';
import { NewGuid } from '../../utils/appUtils'


const initState = {
    allDefineFlowSteps: [],//所有后台插件定义的流程步骤
    flowList: [],
    commonParamList: [],//公共配置参数
    activeFlowId: '',//当前选中流程的ID
    activeFlowDefine: {},//当前选中流程
    activeStep: {},//当前选中步骤
    activeStepConfig: {},//当前选中步骤的配置
    diagramEngine: {},
    showLoading: false,//是否显示加载中
    tableConfigVisible: [],//配置表格显示、隐藏记录
    tableConfigEditIndex: -1,//配置表格编辑行索引
    importDialog: {//导入对话框
        visible: false,
        dataSource: []
    }
};
let reducerMap = {};
String.prototype.firstLowerCase = function () {
    return this.toString()[0].toLowerCase() + this.toString().slice(1);
}
// 设置遮罩层
reducerMap[actionTypes.SET_LOADING] = function (state, action) {
    return Object.assign({}, state, { showLoading: action.payload });
}
reducerMap[actionTypes.ACTIVE_FLOW] = function (state, action) {
    console.log("activeID:", action.payload);
    if (action.payload === "") {
        return Object.assign({}, state, {
            activeFlowId: action.payload,
            activeFlowDefine: { workflowID: '', workflowName: '', autoRetryCount: '3', stepList: [] }, activeStep: {}, activeStepConfig: {}
        });
    }
    let activeFlowDefine = state.flowList.find(f => f.workflowID === action.payload) || {};
    return Object.assign({}, state, { activeFlowId: action.payload, activeFlowDefine: activeFlowDefine, showLoading: true });
}
reducerMap[actionTypes.ACTIVE_FLOW_STEP] = function (state, action) {
    let activeStep = { ...state.activeStep };
    if (state.activeFlowDefine.stepList) {
        activeStep = state.activeFlowDefine.stepList.find(s => s.stepID === action.payload);
    }
    console.log("当前选中step:", activeStep);
    return Object.assign({}, state, { activeStep: activeStep });
}
//流程列表返回
reducerMap[actionTypes.EXECUTE_NWF_COMMAND_COMPLETE] = function (state, action) {
    let initStat = { ...state };
    if (action.payload.command === NwfCommand.GetWorkflowList.CommandName) {
        initStat = Object.assign(initStat, { flowList: action.payload.extension.workflowList });
    } else if (action.payload.command === NwfCommand.GetStepList.CommandName) {
        let flowSteps = [];
        action.payload.extension.serviceList.map((step, i) => {
            let findStep = flowSteps.find(s => s.category === step.category);
            if (!findStep) {
                let children = action.payload.extension.serviceList.filter(s => s.category === step.category);
                flowSteps.push({ id: step + i, category: step.category, children: children });
            }
        });
        initStat = Object.assign(initStat, { allDefineFlowSteps: flowSteps });
    }
    else if (action.payload.command === NwfCommand.GetWorkflowDefine.CommandName) {
        let activeFlowId = '';
        let activeFlowDefine = action.payload.extension.workflow;
        if (activeFlowDefine) {
            activeFlowId = activeFlowDefine.workflowID;
            activeFlowDefine.stepList.map(step => {
                if (typeof (step.serviceConfig) === "string") {
                    step.serviceConfig = JSON.parse(step.serviceConfig);
                }
            })
        }
        initStat = Object.assign(initStat, { activeFlowDefine: activeFlowDefine, activeFlowId: activeFlowId });
    }
    else if (action.payload.command === NwfCommand.GetSetpConfig.CommandName) {//获取步骤配置详细
        let tableConfigVisible = [];
        let activeStepConfig = action.payload.extension;
        if (activeStepConfig.serviceConfig) {
            let serverConfigFields = activeStepConfig.serviceConfig.fieldList;
            if (serverConfigFields) {
                serverConfigFields.filter(c => c.fieldType === "Table").map(c => {
                    tableConfigVisible.push({ id: c.fieldName, visible: false });
                });
            }
        }
        //由于nwf的步骤详细定义接口返回的配置数据还没有流程步骤中返回的数据详细，所以这里做一个合并
        if (typeof (activeStepConfig.defaultServiceConfig) === "string") {
            activeStepConfig.defaultServiceConfig = JSON.parse(activeStepConfig.defaultServiceConfig);
            activeStepConfig.defaultServiceConfig = Object.assign(activeStepConfig.defaultServiceConfig, state.activeStep.serviceConfig);
        }
        initStat = Object.assign(initStat, { activeStepConfig: activeStepConfig, tableConfigVisible: tableConfigVisible });
    }
    else if (action.payload.command === NwfCommand.GetCommonParamList.CommandName) {

        initStat = Object.assign(initStat, { commonParamList: action.payload.extension.parameterList });
    }
    else if (action.payload.command === NwfCommand.SaveWorkflow.CommandName) {
        notification.success({
            description: "流程保存成功!",
            duration: 3
        });
    }
    else if (action.payload.command === NwfCommand.DeleteWorkFlow.CommandName) {
        notification.success({
            description: "流程删除成功!",
            duration: 3
        });
        initStat = Object.assign(initStat, { activeFlowDefine: {}, activeStep: {}, activeStepConfig: {} });
    }
    else if (action.payload.command === NwfCommand.ImportWorkFLow.CommandName) {
        notification.success({
            description: "流程导入成功!",
            duration: 3
        });
        initStat = Object.assign(initStat, { importDialog: { visible: false, dataSource: [] } });
    }
    //console.log("processList:", initStat, action.payload.command, NwfCommand.GetWorkflowList.CommandName);
    return Object.assign({}, state, initStat);
}
//打开配置对话框
reducerMap[actionTypes.OPEN_TABLE_CONFIG] = function (state, action) {
    let tableConfigVisible = state.tableConfigVisible.slice();
    for (let i in tableConfigVisible) {
        if (tableConfigVisible[i].id === action.payload) {
            tableConfigVisible[i].visible = true;
        }
    }
    return Object.assign({}, state, { tableConfigVisible: tableConfigVisible });
}
//关闭配置对话框
reducerMap[actionTypes.CLOSE_TABLE_CONFIG] = function (state, action) {
    let tableConfigVisible = state.tableConfigVisible.slice();
    for (let i in tableConfigVisible) {
        if (tableConfigVisible[i].id === action.payload) {
            tableConfigVisible[i].visible = false;
        }
    }
    return Object.assign({}, state, { tableConfigVisible: tableConfigVisible });
}

reducerMap[actionTypes.EDIT_CONFIG_TABLE] = function (state, action) {
    return Object.assign({}, state, { tableConfigEditIndex: action.payload });
}
reducerMap[actionTypes.EDIT_CONFIG_TABLE_COMPLETE] = function (state, action) {
    return Object.assign({}, state, { tableConfigEditIndex: action.payload });
}
//保存配置表的配置项
reducerMap[actionTypes.SAVE_CONFIG_TABLE_ITEM] = function (state, action) {
    let activeStepConfig = { ...state.activeStepConfig };
    let activeFlowDefine = { ...state.activeFlowDefine };
    let rowIndex = state.tableConfigEditIndex;
    let tableProperty = state.tableConfigVisible.find(t => t.visible === true);
    if (tableProperty) {
        let fieldName = tableProperty.id;
        activeStepConfig.defaultServiceConfig[fieldName.firstLowerCase()][rowIndex] = action.payload;
        activeFlowDefine.stepList.map(step => {
            if (step.stepID === state.activeStep.stepID) {
                step.serviceConfig = activeStepConfig.defaultServiceConfig;
            }
        });
    }
    return Object.assign({}, state, { tableConfigEditIndex: -1, activeStepConfig: activeStepConfig, activeFlowDefine: activeFlowDefine });
}
//移除配置表的配置项
reducerMap[actionTypes.REMOVE_CONFIG_TABLE_ITEM] = function (state, action) {
    return Object.assign({}, state, {});
}
reducerMap[actionTypes.CHANGE_STEP_COMMON_PARAM] = function (state, action) {
    let { changeField, value } = action.payload;
    let activeStepConfig = { ...state.activeStepConfig };
    if (activeStepConfig) {
        for (let p in activeStepConfig.defaultServiceConfig) {
            if (p.toLowerCase() === changeField.fieldName.toLowerCase()) {
                activeStepConfig.defaultServiceConfig[p] = value;
                break;
            }
        }
    }
    let activeStep = state.activeStep;
    let activeFlowDefine = state.activeFlowDefine;
    activeStep.serviceConfig = activeStepConfig.defaultServiceConfig;
    for (let i in activeFlowDefine.stepList) {
        if (activeFlowDefine.stepList[i].stepID === state.activeStep.stepID) {
            activeFlowDefine.stepList[i] = activeStep;
        }
    }
    return Object.assign({}, state, { activeFlowDefine: activeFlowDefine, activeStep: activeStep, activeStepConfig: activeStepConfig });
}

reducerMap[actionTypes.CHANGE_STEP_BASIC_PARAM] = function (state, action) {
    let { changeField, value } = action.payload;
    let activeStep = { ...state.activeStep };
    let hasField = false;
    if (activeStep) {
        for (let p in activeStep) {
            if (p.toLowerCase() === changeField.fieldName.toLowerCase()) {
                activeStep[p] = value;
                hasField = true;
                break;
            }
        }
    }
    if (!hasField) {
        activeStep[changeField.fieldName] = value;
    }
    let activeFlowDefine = state.activeFlowDefine;
    for (let i in activeFlowDefine.stepList) {
        if (activeFlowDefine.stepList[i].stepID === activeStep.stepID) {
            activeFlowDefine.stepList[i] = activeStep;
        }
    }
    return Object.assign({}, state, { activeStep: activeStep, activeFlowDefine: activeFlowDefine });
}
//添加拖动到设计器中的流程步骤
reducerMap[actionTypes.DRAG_FLOW_STEP_ADD] = function (state, action) {
    let activeFlow = { ...state.activeFlowDefine };
    activeFlow.stepList.push(action.payload);
    return Object.assign({}, state, { activeFlowDefine: activeFlow, activeStep: action.payload });
}
//编辑流程定义
reducerMap[actionTypes.EDIT_FLOW_DEFINE] = function (state, action) {
    let { changeField, value } = action.payload;
    let activeFlowDefine = { ...state.activeFlowDefine };
    let activeFlowId = state.activeFlowId;
    activeFlowDefine.stepList = activeFlowDefine.stepList || [];
    let hasField = false;
    if (changeField.fieldName === "workflowID") {
        activeFlowId = value;
    }
    for (let p in activeFlowDefine) {
        if (p.toLowerCase() === changeField.fieldName.toLowerCase()) {
            activeFlowDefine[p] = value;
            hasField = true;
        }
    }
    if (!hasField) {
        activeFlowDefine[changeField.fieldName] = value;
    }
    return Object.assign({}, state, { activeFlowDefine: activeFlowDefine, activeFlowId: activeFlowId });
}
//打开导入对话框
reducerMap[actionTypes.OPEN_IMPORT_FLOW] = function (state, action) {
    return Object.assign({}, state, { importDialog: action.payload });
}
//更新流程对应步骤的属性，如：x，y
reducerMap[actionTypes.EDIT_FLOW_STEP] = function (state, action) {
    let activeFlowDefine = { ...state.activeFlowDefine };
    activeFlowDefine.stepList.map(step => {
        if (state.activeStep && step.stepID === state.activeStep.stepID
            || action.payload.target && step.stepID === action.payload.target.sourceStepID) {
            if (action.payload.command === "region") {
                step.layout.region.left = Math.round(action.payload.region.x);
                step.layout.region.top = Math.round(action.payload.region.y);
                console.log("变更location：", step);
            } else if (action.payload.command === "target") {
                step.jointpoint = step.jointpoint || [];
                if (action.payload.type === "add") {
                    if (!step.jointpoint.find(p => p.targetStep === action.payload.target.targetStepID)) {
                        let newPoint = { joinID: NewGuid(), protocolType: "Output" };
                        newPoint.targetStep = action.payload.target.targetStepID;
                        step.jointpoint.push(newPoint);
                    }
                } else {
                    for (let i = step.jointpoint.length - 1; i > -1; i--) {
                        if (step.jointpoint[i].targetStep === action.payload.target.targetStepID) {
                            step.jointpoint.splice(i, 1);
                            break;
                        }
                    }
                }
            }
        }
    });
    return Object.assign({}, state, { activeFlowDefine: activeFlowDefine });
}
//删除流程步骤
reducerMap[actionTypes.DELETE_FLOW_SETP] = function (state, action) {
    let activeFlowDefine = { ...state.activeFlowDefine };
    for (let i = (activeFlowDefine.stepList.length - 1); i > -1; i--) {
        if (activeFlowDefine.stepList[i].stepID === action.payload) {
            activeFlowDefine.stepList.splice(i, 1);
            break;
        }
    }
    return Object.assign({}, state, { activeFlowDefine: activeFlowDefine });
}

export default handleActions(reducerMap, initState);