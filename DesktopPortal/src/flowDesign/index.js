import React, { Component } from 'react';
import { withReducer } from 'react-redux-dynamic-reducer'
import { Layout, Menu, Icon, Button, Row, Col, Input, InputNumber, Checkbox, Spin, Popconfirm, Upload, notification } from 'antd'
import { connect } from 'react-redux'
import reducers from './reducers'
import { sagaMiddleware } from '../'
import rootSaga from './saga/rootSaga';
import './index.less';
import AllFlowStepDefine from './pages/allFlowStepDefine'
import NwfCommand from './constants/commandDefine';
import { NewGuid } from '../utils/appUtils'
import {
    selectedFlow, selectedFlowStep,
    executeNwfCommand, setLoading, dragFlowStepAdd, deleteFlowDefine,
    saveFlowDefine, editFlowDefine, openImportFlow, updateFlowStep
} from './actions/actionCreator'
import {
    DiagramEngine,
    DefaultNodeFactory,
    DefaultLinkFactory,
    DiagramModel,
    DefaultNodeModel,
    LinkModel,
    DefaultPortModel, PortModel,
    DiagramWidget
} from "storm-react-diagrams";
import { DiamondNodeModel } from './pages/diamondComponent/DiamondNodeModel';
import { DiamondWidgetFactory } from './pages/diamondComponent/DiamondWidgetFactory';
import '../../node_modules/storm-react-diagrams/dist/style.min.css';
import AddFlowDialog from './pages/addFlowDialog';
import FlowPropery from './pages/flowProperty';
import JsonDownload, { ReadUploadFile } from './utils';
import ImportFlowDialog from './pages/importFlowDialog';

sagaMiddleware.run(rootSaga);

const { Header, Sider, Content } = Layout;
const SubMenu = Menu.SubMenu;
class FlowDesignIndex extends Component {
    state = {
        collapsed: false,
        diagramEngine: {},
        operInfo: 'add',
        existNodeMapList: [],//已保存步骤对应的nodeid
        checkedFlowList: []//选中
    }
    componentWillMount() {
        this.props.dispatch(executeNwfCommand(NwfCommand.GetWorkflowList));
        this.props.dispatch(executeNwfCommand(NwfCommand.GetStepList));
        this.props.dispatch(executeNwfCommand(NwfCommand.GetCommonParamList));
        var engine = new DiagramEngine();
        engine.registerNodeFactory(new DefaultNodeFactory());
        engine.registerLinkFactory(new DefaultLinkFactory());
        engine.registerNodeFactory(new DiamondWidgetFactory());

        var model = new DiagramModel();
        model.addListener({ linksUpdated: this.handleLinksUpdated, controlsUpdated: () => { console.log("ctrl update") } });
        engine.setDiagramModel(model);
        this.setState({ diagramEngine: engine });
    }
    componentWillReceiveProps(newProps) {
        let { diagramEngine } = this.state;
        let existNodeMapList = [];
        let allLinks = diagramEngine.getDiagramModel().getLinks();
        let allNodes = diagramEngine.getDiagramModel().getNodes();
        let nodeGuidList = [], linkGuidList = [];
        for (let linkGuid in allLinks) {
            linkGuidList.push(linkGuid);
        }
        for (let nodeGuid in allNodes) {
            nodeGuidList.push(nodeGuid);
        }
        //重新绘制
        let activeFlowInfo = newProps.flowChart.activeFlowDefine;
        if (activeFlowInfo.stepList != undefined) {
            if (nodeGuidList.length != activeFlowInfo.stepList.length
                || newProps.flowChart.activeFlowId !== activeFlowInfo.workflowID) {
                this.clearDiagramEngine();
                activeFlowInfo.stepList.map(step => {
                    step.x = step.layout.region.left;
                    step.y = step.layout.region.top;
                    step.name = step.stepName;
                    let stepNode = nodeGuidList.find(guid => guid === step.stepID);
                    if (!stepNode) {
                        let addedNode = this.createModel(step);
                        existNodeMapList.push({ stepID: step.stepID, nodeID: addedNode.id });
                    }
                });
                this.createLink(allNodes, existNodeMapList);
                this.setState({ existNodeMapList: existNodeMapList });
                setTimeout(() => {
                    this.bindNodeEvent(newProps);
                }, 500);

            }
        }
    }

    createModel(node) {//创建步骤模型
        let addedNode;//添加后的步骤节点
        let { diagramEngine } = this.state;
        if (node.serviceType === "Begin") {
            var stepNode = new DefaultNodeModel(node.name, "rgb(0,192,255)");
            stepNode.extend = node;
            var portIn = stepNode.addPort(new DefaultPortModel(false, "out-1", "输出"));
            stepNode.x = node.x;
            stepNode.y = node.y;
            stepNode.addListener({ selectionChanged: this.handleNodeSelectionChanged, entityRemoved: this.handleNodeRemove });
            addedNode = diagramEngine.getDiagramModel().addNode(stepNode);
        }
        else if (node.serviceType === "End") {
            var stepNode = new DefaultNodeModel(node.name, "rgb(192,255,0)");
            stepNode.extend = node;
            var portIn = stepNode.addPort(new DefaultPortModel(true, "in-1", "输入"));
            stepNode.x = node.x;
            stepNode.y = node.y;
            stepNode.addListener({ selectionChanged: this.handleNodeSelectionChanged, entityRemoved: this.handleNodeRemove });
            addedNode = diagramEngine.getDiagramModel().addNode(stepNode);
        } else if (node.serviceType === "judge") {
            var stepNode = new DiamondNodeModel(node.name);
            stepNode.extend = node;
            stepNode.x = node.x;
            stepNode.y = node.y;
            stepNode.addListener({ selectionChanged: this.handleNodeSelectionChanged, entityRemoved: this.handleNodeRemove });
            addedNode = diagramEngine.getDiagramModel().addNode(stepNode);
        } else {
            var stepNode = new DefaultNodeModel(node.name, "rgb(0,192,255)");
            stepNode.extend = node;
            var portIn = stepNode.addPort(new DefaultPortModel(true, "in-1", "输入"));
            var portOut = stepNode.addPort(new DefaultPortModel(false, "out-1", "输出"));
            stepNode.x = node.x;
            stepNode.y = node.y;
            stepNode.addListener({ selectionChanged: this.handleNodeSelectionChanged, entityRemoved: this.handleNodeRemove });
            addedNode = diagramEngine.getDiagramModel().addNode(stepNode);
        }
        this.setState({ diagramEngine: diagramEngine });
        return addedNode;
    }
    createLink(allNodes, existNodeMapList) {
        let { diagramEngine } = this.state;
        if (allNodes) {
            for (let guid in allNodes) {
                let curNode = allNodes[guid];
                if (curNode.extend.jointpoint) {
                    for (let i in curNode.extend.jointpoint) {
                        let joinPoint = curNode.extend.jointpoint[i];
                        if (joinPoint.targetStep) {
                            let targetStep = existNodeMapList.find(m => m.stepID === joinPoint.targetStep);
                            if (targetStep) {
                                let sourcePort = curNode.ports['out-1'];
                                let targetPort = allNodes[targetStep.nodeID].ports['in-1'];
                                var link = new LinkModel();
                                link.setSourcePort(sourcePort);
                                link.setTargetPort(targetPort);
                                diagramEngine.getDiagramModel().addLink(link);
                            }
                        }

                    }
                }
            }
        }
    }
    bindNodeEvent(newProps) {
        let nodeList = document.getElementsByClassName("node");
        if (nodeList && nodeList.length > 0) {
            for (let i = 0; i < nodeList.length; i++) {
                if (!nodeList[i].onmouseup) {
                    nodeList[i].onmouseup = (e) => {
                        let point = this.state.diagramEngine.getRelativeMousePoint(e);
                        console.log('释放node:', e, point);
                        this.props.dispatch(updateFlowStep({ command: 'region', region: { x: point.x, y: point.y } }));
                    };
                }
            }
        }
    }
    clearDiagramEngine() {
        let { diagramEngine } = this.state;
        //清空当前流程绘制的图形
        let allLinks = diagramEngine.getDiagramModel().getLinks();
        let allNodes = diagramEngine.getDiagramModel().getNodes();
        let nodeGuidList = [], linkGuidList = [];
        for (let linkGuid in allLinks) {
            diagramEngine.getDiagramModel().removeLink(linkGuid);
        }
        for (let nodeGuid in allNodes) {
            diagramEngine.getDiagramModel().removeNode(nodeGuid);
        }
    }
    //选中流程变更
    handleFlowChange = (item) => {
        this.setState({ operInfo: 'edit' });
        this.props.dispatch(selectedFlow(item.key));
        let flowInfo = this.props.flowChart.flowList.find(f => f.workflowID === item.key);
        if (flowInfo) {
            let reqeustDefine = NwfCommand.GetWorkflowDefine;
            reqeustDefine.workflowID = item.key;
            this.props.dispatch(setLoading(true));
            this.props.dispatch(executeNwfCommand(reqeustDefine));
        }
    }

    handleFlowDropAllow = (e) => {
        e.preventDefault();
    }
    //节点拖放添加flowStepAdd
    handleFlowDrop = (e) => {
        var points = this.state.diagramEngine.getRelativeMousePoint(e);
        try {
            let newAddNode = JSON.parse(e.dataTransfer.getData("dropStep"));
            console.log("newAddNode:", newAddNode);
            let model = { ...newAddNode, x: points.x, y: points.y, serviceID: newAddNode.id };
            let addedNode = this.createModel(model);
            let stepNode = { ...newAddNode };
            stepNode.stepID = addedNode.id;
            stepNode.stepName = stepNode.name;
            stepNode.serviceID = stepNode.id;
            delete stepNode.id;
            delete stepNode.name;
            stepNode.layout = { region: { left: model.x, top: model.y } };
            this.props.dispatch(dragFlowStepAdd(stepNode));
            let existNodeMapList = this.state.existNodeMapList.slice();
            existNodeMapList.push({ stepID: stepNode.stepID, nodeID: addedNode.id });
            this.setState({ existNodeMapList: existNodeMapList });
            setTimeout(() => { this.bindNodeEvent(this.props); }, 500);
            //新增的步骤node需要更新扩展信息
            let { diagramEngine } = this.state;
            let allNodes = diagramEngine.getDiagramModel().getNodes();
            for (let guid in allNodes) {
                if (guid === addedNode.id) {
                    allNodes[guid].extend = stepNode;
                }
            }
        } catch (e) {
            console.log("拖放异常", e);
        }
    }
    //选中节点变更
    handleNodeSelectionChanged = (node, isSelected) => {
        if (isSelected) {
            console.log("node selected:", node, isSelected);
            let cacheStep = this.state.existNodeMapList.find(m => m.nodeID === node.id);
            let cacheNodeID = cacheStep ? cacheStep.stepID : '';
            this.props.dispatch(selectedFlowStep(node.extend.stepID || cacheNodeID));
            let getStepConfigRequest = NwfCommand.GetSetpConfig;

            getStepConfigRequest.serviceID = node.extend.serviceID;
            this.props.dispatch(executeNwfCommand(getStepConfigRequest));
        }
    }

    handleNodeRemove = (entity) => {
        console.log("移除node：", entity);
        this.props.dispatch(deleteFlowDefine(entity.extend.stepID));
    }

    handleFlowAdd = (e) => {
        this.props.dispatch(selectedFlow(""));
        this.setState({ operInfo: 'add' });
    }
    //保存当前流程的定义
    handleSaveFlowDefine = () => {
        console.log("保存定义", this.props.flowChart.activeFlowDefine);
        let saveRequest = NwfCommand.SaveWorkflow;
        saveRequest.Workflow = { ...this.props.flowChart.activeFlowDefine };
        if (saveRequest.Workflow.workflowID && saveRequest.Workflow.workflowID !== '') {
            let allLinks = this.state.diagramEngine.getDiagramModel().getLinks();
            console.log("所有连线：", allLinks, this.state.existNodeMapList);
            for (let guid in allLinks) {
                let link = allLinks[guid];
                if (link.sourcePort !== null && link.targetPort !== null) {
                    console.log("link.sourcePort.parentNode.extend", link.sourcePort.parentNode.extend);
                    console.log("link.targetPort.parentNode.extend", link.targetPort.parentNode.extend);
                    let sourcePortStepID = link.sourcePort.parentNode.extend.stepID;
                    let targetPortStepID = link.targetPort.parentNode.extend.stepID;
                    let stepInfo = saveRequest.Workflow.stepList.find(step => step.stepID === sourcePortStepID);
                    if (stepInfo) {
                        stepInfo.jointpoint = stepInfo.jointpoint || [];
                        if (!stepInfo.jointpoint.find(p => p.targetStep === targetPortStepID)) {
                            let newPoint = { joinID: NewGuid(), protocolType: "Output" };
                            newPoint.targetStep = targetPortStepID;
                            stepInfo.jointpoint.push(newPoint);
                        }
                    }
                }
            }
            saveRequest.Workflow.stepList.map(step => {
                if (step.serviceConfig) {
                    step.serviceConfig = JSON.stringify(step.serviceConfig);
                }
            });
            this.props.dispatch(executeNwfCommand(saveRequest));
            this.props.dispatch(setLoading(true));
        }
    }
    //处理基础配置信息更改
    handleBasicConfigChange = (value, field) => {
        console.log("value,fieldInfo", value, field);
        this.props.dispatch(editFlowDefine({ changeField: { fieldName: field }, value: value }));
    }
    //流程checked处理
    handleFlowChecked = (e, flow) => {
        console.log("e,flow", e.target.checked, flow.workflowID);
        let checkedList = this.state.checkedFlowList.slice();
        let hasChild = false;
        for (let i in checkedList) {
            if (checkedList[i].flowID === flow.workflowID) {
                checkedList[i].checked = e.target.checked;
                hasChild = true;
            }
        }
        if (!hasChild) {
            checkedList.push({ flowID: flow.workflowID, checked: e.target.checked });
        }
        this.setState({ checkedFlowList: checkedList });
    }
    //处理流程删除
    handleFlowDelete = (e) => {
        if (this.props.flowChart.activeFlowId !== "") {
            let request = NwfCommand.DeleteWorkFlow;
            request.WorkflowID = this.props.flowChart.activeFlowId;
            this.props.dispatch(executeNwfCommand(request));
            this.clearDiagramEngine();
        }
    }
    //处理流程下载
    handleFlowDownload = (e) => {
        let downloadList = [];
        let flowList = this.props.flowChart.flowList;
        let checkedList = this.state.checkedFlowList.filter(f => f.checked === true);
        if (checkedList.length > 0) {
            flowList.map(flow => {
                if (checkedList.find(c => c.flowID === flow.workflowID)) {
                    downloadList.push(flow);
                }
            })
        }
        JsonDownload(JSON.stringify(downloadList), 'Export.Json');
    }

    handleFlowUpload = (fileInfo) => {
        console.log("上传信息", fileInfo);
        ReadUploadFile(fileInfo, (templateContext) => {
            try {
                console.log("文件内容：", templateContext);
                let importFlowList = JSON.parse(templateContext);
                this.props.dispatch(openImportFlow({
                    visible: true,
                    dataSource: importFlowList
                }));
            } catch (e) {
                notification.error({
                    description: "不是有效的模板文件!",
                    duration: 3
                });
            }
        });
        return false;
    }
    //连线更新
    handleLinksUpdated = (entity, isadded, continueCount) => {
        if (!isadded) {
            if (entity.sourcePort && entity.targetPort) {
                console.log("成功删除连线：", entity, isadded);
                let sourceStepID = entity.sourcePort.parentNode.extend.stepID;
                let targetStepID = entity.targetPort.parentNode.extend.stepID;
                this.props.dispatch(updateFlowStep({ command: 'target', type: 'delete', target: { sourceStepID: sourceStepID, targetStepID: targetStepID } }));
            }
        }
    }

    toggle = () => {
        this.setState({
            collapsed: !this.state.collapsed,
        });
    }

    render() {
        //const allFlowSteps = this.props.flowChart.allDefineFlowSteps || [];
        const flowList = this.props.flowChart.flowList;
        const selectedFlowKey = this.props.flowChart.activeFlowId;
        let activeFlowInfo = this.props.flowChart.activeFlowDefine;
        let checkedList = this.state.checkedFlowList.filter(f => f.checked === true);
        const submitDisable = (!activeFlowInfo.workflowID || activeFlowInfo.workflowID === '');
        return (
            <Layout className="page flowDesign">
                <Sider collapsible collapsed={this.state.collapsed} onCollapse={this.toggle} >
                    <div className="subTitle">
                        流程列表<Button type="primary" shape="circle" icon="plus" size="small" onClick={this.handleFlowAdd} />
                        <Popconfirm title="确认删除当前流程？" onConfirm={this.handleFlowDelete}>
                            <Button type="primary" shape="circle" icon="delete" size="small" disabled={this.props.flowChart.activeFlowId !== "" ? false : true} />
                        </Popconfirm>
                        <Button type="primary" shape="circle" icon="download" size="small" onClick={this.handleFlowDownload} disabled={checkedList.length > 0 ? false : true} />
                        <Upload beforeUpload={(file) => { return false; }} onChange={this.handleFlowUpload} showUploadList={false}>
                            <Button type="primary" shape="circle" icon="upload" size="small" />
                        </Upload>
                    </div>
                    <Menu mode="inline" theme="dark" selectedKeys={[selectedFlowKey]} onClick={this.handleFlowChange} style={{ width: '100%' }}>
                        {
                            flowList.map(flow => <Menu.Item key={flow.workflowID}><Checkbox onChange={(e) => this.handleFlowChecked(e, flow)} />{flow.workflowName}</Menu.Item>)
                        }
                    </Menu>
                </Sider>
                <Layout>
                    <Header>
                        流程ID：<Input value={activeFlowInfo.workflowID} onChange={(e) => this.handleBasicConfigChange(e.target.value, 'workflowID')} disabled={this.state.operInfo !== 'add'} style={{ width: '150px' }} />流程名称：<Input value={activeFlowInfo.workflowName} onChange={(e) => this.handleBasicConfigChange(e.target.value, 'workflowName')} style={{ width: '150px' }} />
                        失败重试： <InputNumber min={1} max={10} defaultValue={3} onChange={(e) => this.handleBasicConfigChange(e, 'autoRetryCount')} />次
                        <Button type="primary" icon="save" size='large' onClick={this.handleSaveFlowDefine} disabled={submitDisable}>保存</Button>
                    </Header>
                    <Content className="content" style={{ height: '100%' }}>
                        <Spin spinning={this.props.flowChart.showLoading} style={{ height: '100%' }}>
                            <Row style={{ height: '100%' }}>
                                <Col span={4} style={{ height: '100%' }}>
                                    <AllFlowStepDefine />
                                </Col>
                                <Col span={16} style={{ paddingLeft: "10px", height: '97%' }}>
                                    设计器
                                <div onDragOver={this.handleFlowDropAllow} onDrop={this.handleFlowDrop} style={{ height: '100%' }}>
                                        <DiagramWidget diagramEngine={this.state.diagramEngine} />
                                    </div>
                                </Col>
                                <Col span={4}>
                                    流程属性
                                <FlowPropery />
                                </Col>
                            </Row>
                        </Spin>
                        <ImportFlowDialog />
                    </Content>
                </Layout>
            </Layout>
        )
    }
}
function mapStateToProps(state, props) {
    return {
        flowChart: state.flowChart
    }
}
function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

export default withReducer(reducers, 'SearchToolIndex')(connect(mapStateToProps, mapDispatchToProps)(FlowDesignIndex));