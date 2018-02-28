import { connect } from 'react-redux';
import { openImportFlow, saveImportFlow, executeNwfCommand } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Table, Modal, Input, notification } from 'antd'
import NwfCommand from '../constants/commandDefine';

class ImportFlowDialog extends Component {
    state = {
        selectedRowKeys: [],//选中流程列表
        dataSource: []
    }

    componentWillReceiveProps(newProps) {
        let dataSource = newProps.importDialog.dataSource;
        dataSource.map(flow => {
            flow.newWorkflowID = flow.workflowID;
            flow.newWorkflowName = flow.workflowName;
        });
        this.setState({ dataSource: dataSource });
    }

    handleOk = (e) => {
        console.log("选中流程：，", this.state.selectedRowKeys);
        if (this.state.selectedRowKeys.length == 0) {
            notification.warning({
                description: "没有需要导入的流程!",
                duration: 3
            });
            return;
        }
        let importList = [];
        this.state.dataSource.map(flow => {
            if (this.state.selectedRowKeys.find(key => key === flow.workflowID)) {
                let newFlow = { ...flow, workflowID: flow.newWorkflowID, workflowName: flow.newWorkflowName };
                newFlow.stepList.map(step => {
                    if (step.serviceConfig) {
                        step.serviceConfig = JSON.stringify(step.serviceConfig);
                    }
                });
                importList.push(newFlow);
            }
        });
        let saveRequest = NwfCommand.ImportWorkFLow;
        saveRequest.WorkflowList = importList;
        console.log("流程导入：", JSON.stringify(saveRequest));
        this.props.dispatch(executeNwfCommand(saveRequest));
    }

    handleCancel = (e) => {
        this.setState({ selectedRowKeys: [] });
        this.props.dispatch(openImportFlow({//导入对话框
            visible: false,
            dataSource: []
        }))
    }
    handleRowEdit = (rowIndex, fieldName, e) => {//处理行编辑
        console.log("edit", rowIndex, fieldName, e.target.value);
        let dataSource = [...this.state.dataSource];
        dataSource[rowIndex][fieldName] = e.target.value;
        this.setState({ dataSource: dataSource });
    }
    onSelectChange = (selectedRowKeys, selectedRows) => {
        console.log("change:", selectedRowKeys, selectedRows);
        this.setState({ selectedRowKeys: selectedRowKeys });
    }

    render() {
        let tableColumns = [{
            title: '流程ID',
            dataIndex: 'workflowID',
            key: 'workflowID'
        }, {
            title: '流程名称',
            dataIndex: 'workflowName',
            key: 'workflowName'
        }, {
            title: '新流程ID',
            dataIndex: 'newWorkflowID',
            key: 'newWorkflowID',
            render: (text, record, rowIndex) => {
                return (<Input value={record.newWorkflowID} style={{ width: '100px' }} onChange={(e) => this.handleRowEdit(rowIndex, "newWorkflowID", e)} />)
            }
        }, {
            title: '新流程名称',
            dataIndex: 'newWorkflowName',
            key: 'newWorkflowName', render: (text, record, rowIndex) => {
                return (<Input value={record.newWorkflowName} style={{ width: '150px' }} onChange={(e) => this.handleRowEdit(rowIndex, 'newWorkflowName', e)} />)
            }
        }];
        let visible = this.props.importDialog.visible;
        let dataSource = this.state.dataSource;
        const { selectedRowKeys } = this.state;
        const rowSelection = {
            selectedRowKeys,
            onChange: this.onSelectChange,
        };

        return (<Modal width={600} title="导入流程" maskClosable={false} visible={visible}
            onOk={this.handleOk} onCancel={this.handleCancel} >
            <Table rowKey={record => record.workflowID} rowSelection={rowSelection} dataSource={dataSource} columns={tableColumns} />
        </Modal>);
    }
}
function mapStateToProps(state) {
    return {
        importDialog: state.flowChart.importDialog,
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(ImportFlowDialog);