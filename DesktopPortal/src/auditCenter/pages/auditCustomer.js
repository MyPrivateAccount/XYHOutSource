import { connect } from 'react-redux';
import { } from '../actions/actionCreator';
import React, { Component } from 'react';
import { Input, Steps, Icon, Button, Row, Col, Table, Spin, Popconfirm } from 'antd';
import AuditForm from './item/auditForm';
import moment from 'moment';
import AuditHistory from './auditHistory'

const Step = Steps.Step;

class AuditCustomer extends Component {
    state = {

    }
    componentWillMount() {

    }
    componentWillReceiveProps(newProps) {
    }
    //获取表格列定义
    getColunms() {
        let columns = [{
            title: '客户信息',
            children: [{
                title: '姓名',
                dataIndex: 'name',
                key: 'name'
            }]
        },
        {
            title: '客户归属信息',
            children: [{
                title: '归属部门',
                dataIndex: 'sourceDepartmentName',
                key: 'sourceDepartmentName'
            }, {
                title: '业务员',
                dataIndex: 'sourceUserName',
                key: 'sourceUserName'
            }]
        }, {
            title: '客户调客信息',
            children: [{
                title: '调客部门',
                dataIndex: 'terDepartmentName',
                key: 'terDepartmentName'
            }, {
                title: '业务员',
                dataIndex: 'terUserName',
                key: 'terUserName'
            }]
        }];
        return columns;
    }
    //格式化客户审核数据
    formateDataSource() {
        let dataSource = [];
        let activeAuditHistory = this.props.activeAuditHistory;
        if (this.props.contentInfo) {
            activeAuditHistory = this.props.contentInfo;
        }
        if (activeAuditHistory.content && activeAuditHistory.content.startsWith("{")) {
            try {
                let jsonObj = JSON.parse(activeAuditHistory.content) || {};
                if (jsonObj.customers) {
                    jsonObj.customers.map(customer => {
                        dataSource.push({
                            id: customer.id, name: customer.name,
                            sourceDepartmentName: jsonObj.sourceDepartmentName,
                            sourceUserName: jsonObj.sourceUserName,
                            terDepartmentName: jsonObj.terDepartmentName,
                            terUserName: jsonObj.terUserName
                        });
                    });
                }
            } catch (e) { }
        }
        return dataSource;
    }
    //对取消的调客客户进行回调
    handleCustomerCancel = (e) => {
        if (this.props.removeCallback) {
            this.props.removeCallback(e);
        }
    }

    render() {
        let columns = this.getColunms();
        const dataSource = this.formateDataSource();
        let activeAuditHistory = this.props.activeAuditHistory;
        if (this.props.contentInfo) {
            activeAuditHistory = this.props.contentInfo;
            columns.push({
                title: '操作',
                children: [{
                    title: '删除',
                    dataIndex: 'oper',
                    render: (text, record) => {
                        return (<Popconfirm title="确定要删除该调客请求吗?" onConfirm={(e) => this.handleCustomerCancel(record)}>
                            <Button type="primary" size="small" shape="circle" icon="delete"></Button>
                        </Popconfirm>)
                    }
                }]
            });
        }

        return (
            <div>
                <Table columns={columns} dataSource={dataSource} bordered={true} style={{ width: '80%', marginTop: '5px' }} />
                {
                    this.props.contentInfo ? <AuditHistory contentInfo={activeAuditHistory} /> : <AuditHistory />
                }
                {
                    activeAuditHistory.examineStatus === 1 && !this.props.contentInfo ? <AuditForm /> : null
                }
            </div>
        )
    }
}

function mapStateToProps(state) {
    console.log("activeAuditHistory:", state.audit.activeAuditHistory);
    return {
        activeAuditInfo: state.audit.activeAuditInfo,
        activeAuditHistory: state.audit.activeAuditHistory
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AuditCustomer);