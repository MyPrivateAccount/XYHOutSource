import { connect } from 'react-redux';
import {
    getXYHBuildingDetail, setLoading, expandSearchbox, recommendBuilding,
    closeTableConfig, editTableConfig, saveTableConfigItem
} from '../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Popconfirm, Row, Col, Tag, Form, Modal, Input, InputNumber, Table, Button, Dropdown, Icon, Checkbox } from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;
const { TextArea } = Input;

class TablePropertyDialog extends Component {
    state = {
        dialogTitle: "配置",
        visible: false,
        fieldName: '',
        tableRowEditInfo: {}
    }
    componentWillReceiveProps(newProps) {
        let configVisible = this.props.tableConfigVisible.find(item => item.visible === true);
        if (configVisible) {
            this.setState({ fieldName: configVisible.id, dialogTitle: this.props.dialogTitle + '配置' });
        }
        console.log("componentWillReceiveProps:", configVisible);
    }

    handleCancel = (e) => {
        if (this.state.fieldName) {
            this.props.dispatch(closeTableConfig(this.state.fieldName));
        }
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
    getHtmlComponent(fieldInfo, configValues, isEdit) {
        let domHtml = null, defaultValue = null;
        if (fieldInfo.isHide === true || fieldInfo.isIgnore === true) {
            return null;
        }
        defaultValue = this.getComponentValue(fieldInfo.fieldName.toLowerCase(), configValues);
        if (fieldInfo.fieldType === "Text") {
            domHtml = isEdit ? <Input disabled={!fieldInfo.canEdit} defaultValue={defaultValue} onChange={(e) => this.handleRowDataChange(fieldInfo, e.target.value)} /> : defaultValue;
        }
        else if (fieldInfo.fieldType === "TextArea") {
            domHtml = isEdit ? <TextArea disabled={!fieldInfo.canEdit} defaultValue={defaultValue} onChange={(e) => this.handleRowDataChange(fieldInfo, e.target.value)} /> : defaultValue;
        }
        else if (fieldInfo.fieldType === "Number") {
            domHtml = isEdit ? <InputNumber min={0} max={90000} disabled={!fieldInfo.canEdit} defaultValue={defaultValue} onChange={(e) => this.handleRowDataChange(fieldInfo, e)} /> : defaultValue;
        }
        else if (fieldInfo.fieldType === "CheckBox") {
            domHtml = isEdit ? <Checkbox disabled={!fieldInfo.canEdit} defaultValue={defaultValue} onChange={(e) => this.handleRowDataChange(fieldInfo, e.target.checked)} /> : defaultValue;
        }
        else if (fieldInfo.fieldType === "Combbox") {
            domHtml = isEdit ? <Select style={{ width: '60px' }} defaultValue={defaultValue} disabled={!fieldInfo.canEdit} onChange={(e) => this.handleRowDataChange(fieldInfo, e)}>{fieldInfo.valueList.map(v => <Option key={v.value} value={v.value}>{v.name}</Option>)}</Select> : defaultValue;
        }
        return domHtml;
    }

    handleOperBtnClick = (operType, recordIndex) => {
        if (operType === "edit") {
            this.props.dispatch(editTableConfig(recordIndex));
            if (recordIndex !== -1) {
                let dataSource = [];
                if (this.state.fieldName) {
                    for (let p in this.props.flowChart.activeStepConfig.defaultServiceConfig) {
                        if (p.toLowerCase() === this.state.fieldName.toLowerCase()) {
                            dataSource = this.props.flowChart.activeStepConfig.defaultServiceConfig[p];
                        }
                    }
                }
                this.setState({ tableRowEditInfo: dataSource[recordIndex] });
            }

        } else if (operType === "cancel") {
            this.props.dispatch(editTableConfig(-1));
        } else if (operType === "save") {
            console.log("保存tableconfig", this.state.tableRowEditInfo);
            this.props.dispatch(saveTableConfigItem(this.state.tableRowEditInfo));
        } else if (operType === "delete") {
            console.log("删除tableconfig");
        }

    }
    //处理表格配置行编辑
    handleRowDataChange = (fieldInfo, value) => {
        let curRowIndex = this.props.flowChart.tableConfigEditIndex;
        if (curRowIndex === -1) {
            return;
        }
        let rowData = { ...this.state.tableRowEditInfo };
        rowData[fieldInfo.fieldName.toLowerCase()] = value;
        this.setState({ tableRowEditInfo: rowData });
        console.log("编辑行数据：", rowData);
    }

    render() {
        let { tableConfigEditIndex } = this.props.flowChart;
        let configTable = this.props.configTable;
        const tableColumn = [];
        if (configTable) {
            configTable.map(item => {
                tableColumn.push({
                    key: item.fieldName.toLowerCase(), title: item.label, dataIndex: item.fieldName.toLowerCase(), render: (text, record, i) => {
                        let isEdit = (tableConfigEditIndex === i);
                        return this.getHtmlComponent(item, record, isEdit)
                    }
                });
            });
            tableColumn.push({
                key: 'oper', title: "操作", dataIndex: 'oper', render: (text, record, i) => {
                    let isEdit = (tableConfigEditIndex === i);
                    let operBtn = isEdit ? <div><Button type="primary" shape="circle" icon="check" size="small" onClick={(e) => this.handleOperBtnClick('save', i)} style={{ margin: '2px' }} />
                        <Popconfirm title="确认要放弃修改吗？" okText="是" cancelText="否" onConfirm={(e) => this.handleOperBtnClick('cancel', i)}>
                            <Button type="primary" shape="circle" icon="close" size="small" />
                        </Popconfirm></div> :
                        <div><Button type="primary" shape="circle" icon="edit" size="small" onClick={(e) => this.handleOperBtnClick('edit', i)} style={{ margin: '2px' }} />
                            <Popconfirm title="确认要删除配置项吗？" okText="是" cancelText="否" onConfirm={(e) => this.handleOperBtnClick('delete', i)}>
                                <Button type="primary" shape="circle" icon="delete" size="small" />
                            </Popconfirm></div>;

                    return (operBtn)
                }
            });
            console.log("tableColumn:", tableColumn);
        }
        let dataSource = [];
        if (this.state.fieldName) {
            for (let p in this.props.flowChart.activeStepConfig.defaultServiceConfig) {
                if (p.toLowerCase() === this.state.fieldName.toLowerCase()) {
                    dataSource = this.props.flowChart.activeStepConfig.defaultServiceConfig[p];
                }
            }
            console.log("dataSource:", dataSource);
        }
        return (
            <Modal width={620} title={this.state.dialogTitle} maskClosable={false} visible={this.props.visible}
                onOk={this.handleCancel} onCancel={this.handleCancel} >
                <Table dataSource={dataSource} columns={tableColumn} />
            </Modal >
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
export default connect(mapStateToProps, mapDispatchToProps)(TablePropertyDialog);