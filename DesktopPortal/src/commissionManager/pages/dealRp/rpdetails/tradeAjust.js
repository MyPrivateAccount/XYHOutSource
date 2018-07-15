//业绩调整组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import moment from 'moment'
import { examineStatusMap } from '../../../constants/const'


class TradAjust extends Component {
    _formatDate = (date) => {
        if (!date) {
            return '';
        }
        return moment(date).format('YYYY-MM-DD');
    }


    view = (distribute) => {
        if (this.props.view) {
            this.props.view(distribute);
        }
    }

    _columns = () => {
        let appTableColumns = [
            {
                title: '调整日期', dataIndex: 'time', key: 'time',
                align: 'center',
                width: '8rem',
                render: (text, record) => {
                    return this._formatDate(text);
                }
            },
            {
                title: '业主调整金额', dataIndex: 'ownerMoney', key: 'ownerMoney', width: '16rem',
                className: 'column-money'
            },
            {
                title: '客户调整金额', dataIndex: 'customMoney', key: 'customMoney', width: '16rem',
                className: 'column-money'
            },
            {
                title: '部门', dataIndex: 'sectionName', key: 'sectionName', width: '16rem', render: (text, record) => {
                    return <span title={text}>{text}</span>
                }
            },
            { title: '调整人', dataIndex: 'username', key: 'username', width: '8rem' },
            {
                title: '状态', dataIndex: 'status', key: 'status',
                render: (text, record) => {
                    return examineStatusMap[record.status] || '';
                }
            },
            { title: '原因', dataIndex: 'updateReason', key: 'updateReason' }

        ];

        if (!this.props.hideYjtzCk) {
            appTableColumns.push({
                title: '操作', dataIndex: 'edit', key: 'edit', width: '5rem', render: (text, record) => (
                    <span>

                        <Tooltip title='查看'>
                            <a style={{ marginLeft: 4 }} onClick={(e) => this.view(record)}>查看</a>
                        </Tooltip>
                    </span>
                )
            })
        }

        return appTableColumns;
    }

    render() {
        let list = this.props.entity || [];
        let columns = this._columns();
        return (
            <Layout>
                <Table pagination={false} size="middle" bordered columns={columns} dataSource={list}></Table>
            </Layout>
        )
    }
}
export default TradAjust