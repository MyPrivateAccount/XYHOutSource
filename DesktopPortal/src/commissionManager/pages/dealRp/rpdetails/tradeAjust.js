//业绩调整组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradAjustEditor from './tradeAjustEditor'
class TradAjust extends Component {
    appTableColumns = [
        { title: '调整日期', dataIndex: 'orgName', key: 'orgName' },
        { title: '业主调整金额', dataIndex: 'paramName', key: 'paramName' },
        { title: '客户调整金额', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '调整人', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '状态', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '原因', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='查看'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' />
                    </Tooltip>
                </span>
            )
        }
    ];
    render() {
        return (
            <Layout>
                <Table columns={this.appTableColumns}></Table>
                <TradAjustEditor/>
            </Layout>
        )
    }
}
export default TradAjust