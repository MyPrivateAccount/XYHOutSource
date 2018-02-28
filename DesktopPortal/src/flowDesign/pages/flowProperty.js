import { connect } from 'react-redux';
import { getShopDetail, setLoading, expandSearchbox, openTableConfig, changeStepCommonParam, changeStepBasicParam } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Tabs, Input, Checkbox, InputNumber, Select, Icon, Menu, Dropdown } from 'antd'
import TablePropertyDialog from './tablePropertyConfig'
import configure from '../../index';

const TabPane = Tabs.TabPane;
const Option = Select.Option;
const { TextArea } = Input;

class FlowPropery extends Component {
    state = {

    }
    componentWillMount() {
        document.addEventListener('keyup', (e) => {
            if (e.target.outerHTML.startsWith("<input") && e.keyCode === 8) {
                if (e && e.preventDefault) {
                    e.preventDefault();   //火狐的 事件是传进来的e  
                    e.stopPropagation();
                }
            }
        }, false);
    }

    getComponentValue(fieldName, configValue) {
        let value = null;
        if (configValue) {
            for (let p in configValue) {
                if (p.toLowerCase() === fieldName.toLowerCase()) {
                    value = configValue[p];
                    break;
                }
            }
        }
        return value;
    }
    getHtmlComponent(fieldInfo, configValues, paramType) {
        let domHtml = null;
        if (fieldInfo.isHide === true || fieldInfo.isIgnore === true) {
            return null;
        }
        let defaultValue = this.getComponentValue(fieldInfo.fieldName, configValues);
        if (fieldInfo.fieldType === "Text") {
            domHtml = <Input disabled={!fieldInfo.canEdit} onChange={(e) => this.handleParamChange(e, fieldInfo, paramType)} value={defaultValue} />;
        }
        else if (fieldInfo.fieldType === "TextArea") {
            domHtml = <TextArea disabled={!fieldInfo.canEdit} onChange={(e) => this.handleParamChange(e, fieldInfo, paramType)} value={defaultValue} />;
        }
        else if (fieldInfo.fieldType === "Number") {
            domHtml = <InputNumber min={1} max={10} disabled={!fieldInfo.canEdit} onChange={(e) => this.handleParamChange(e, fieldInfo, paramType)} value={defaultValue} />;
        }
        else if (fieldInfo.fieldType === "CheckBox") {
            domHtml = <Checkbox disabled={!fieldInfo.canEdit} onChange={(e) => this.handleParamChange(e, fieldInfo, paramType)} value={defaultValue} />;
        }
        else if (fieldInfo.fieldType === "Combbox") {
            domHtml = <Select disabled={!fieldInfo.canEdit} value={defaultValue} onChange={(e) => this.handleParamChange(e, fieldInfo, paramType)}>{fieldInfo.valueList.map(v => <Option key={v.value} value={v.value}>{v.name}</Option>)}</Select>;
        } else if (fieldInfo.fieldType === "Parameter") {
            let paramMenu = (
                <Menu onClick={(e) => this.handleParamChange(e, fieldInfo, paramType)}>{
                    this.props.flowChart.commonParamList.map(param => <Menu.Item key={param.value} value={param.value}>{param.name}</Menu.Item>)
                }
                </Menu>
            );
            domHtml = <div>
                <Input addonAfter={<Dropdown overlay={paramMenu}><Icon type="down" /></Dropdown>} disabled={!fieldInfo.canEdit} value={defaultValue}
                    onChange={(e) => this.handleParamChange({ key: e.target.value }, fieldInfo, paramType)} />
            </div>
        } else if (fieldInfo.fieldType === "Table") {
            let tableVisibleInfo = this.props.tableConfigVisible.find(f => f.id === fieldInfo.fieldName) || false;
            domHtml = <div> <Button onClick={(e) => this.handleConfigTableEdit(fieldInfo)}>配置</Button>
                <TablePropertyDialog visible={tableVisibleInfo.visible} configTable={fieldInfo.tableFieldList} dialogTitle={fieldInfo.label} />
            </div>
        }
        return domHtml;
    }
    handleConfigTableEdit = (fieldInfo) => {
        console.log("edit Table:", fieldInfo);
        this.props.dispatch(openTableConfig(fieldInfo.fieldName));
    }
    handleParamChange = (e, fieldInfo, paramType) => {
        if (paramType === 'basic') {
            let fieldValue = null;
            if (typeof (e) === 'string' || typeof (e) === 'number') {
                fieldValue = e;
            } else {
                fieldValue = e.target.value;
            }
            //console.log("item,key,keypath", fieldValue, fieldInfo, paramType);
            this.props.dispatch(changeStepBasicParam({ changeField: fieldInfo, value: fieldValue }));
        } else {
            //console.log("item,key,keypath", e.key, fieldInfo, paramType);
            this.props.dispatch(changeStepCommonParam({ changeField: fieldInfo, value: e.key }));
        }
    }
    //处理基础配置信息更改
    handleBasicConfigChange = (e, fieldInfo) => {
        console.log("value,fieldInfo", e.target.value, fieldInfo);
        let value = e.target.value;
        this.props.dispatch(changeStepCommonParam({ changeField: fieldInfo, value: value }));
    }

    render() {
        let baseConfigFields = [];
        let serverConfigFields = [];
        let defaultServiceConfig = {};
        //console.log("activeStepConfig:", this.props.flowChart.activeStepConfig);
        if (this.props.flowChart.activeStepConfig.basicConfig && this.props.flowChart.activeStepConfig.serviceConfig) {
            baseConfigFields = this.props.flowChart.activeStepConfig.basicConfig.fieldList;
            serverConfigFields = this.props.flowChart.activeStepConfig.serviceConfig.fieldList;
            defaultServiceConfig = this.props.flowChart.activeStepConfig.defaultServiceConfig || {};
            defaultServiceConfig = Object.assign({ ...this.props.flowChart.activeStep }, defaultServiceConfig);
        }

        return (
            <Tabs defaultActiveKey="1">
                <TabPane tab="基本配置" key="1">
                    {
                        baseConfigFields.map((filed, i) =>
                            <Row key={filed.fieldName + i}>
                                <Col span={10}>{filed.label}</Col><Col span={14}>{this.getHtmlComponent(filed, defaultServiceConfig, 'basic')}</Col>
                            </Row>
                        )
                    }
                </TabPane>
                <TabPane tab="服务配置" key="2">
                    {
                        serverConfigFields.map((filed, i) =>
                            <Row key={filed.fieldName + i}>
                                <Col span={10}>{filed.label}</Col><Col span={14}>{this.getHtmlComponent(filed, defaultServiceConfig, 'common')}</Col>
                            </Row>
                        )
                    }

                </TabPane>
            </Tabs>
        )
    }

}

function mapStateToProps(state) {
    return {
        flowChart: state.flowChart,
        tableConfigVisible: state.flowChart.tableConfigVisible
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(FlowPropery);