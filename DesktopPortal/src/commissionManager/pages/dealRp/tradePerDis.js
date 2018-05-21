//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

class TradePerDis extends Component{

    appTableColumns = [
        { title: '款项类型', dataIndex: 'orgName', key: 'orgName' },
        { title: '收付对象', dataIndex: 'paramName', key: 'paramName' },
        { title: '备注', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '金额', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' />
                    </Tooltip>
                </span>
            )
        }
    ];
    appTable2Columns = [
        { title: '部门', dataIndex: 'orgName', key: 'orgName' },
        { title: '员工', dataIndex: 'paramName', key: 'paramName' },
        { title: '工号', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '金额', dataIndex: 'paramVal', key: 'paramVal' },
        { title: '比例', dataIndex: 'orgName', key: 'orgName' },
        { title: '单数', dataIndex: 'paramName', key: 'paramName' },
        { title: '身份', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' />
                    </Tooltip>
                </span>
            )
        }
    ];

    render(){
        return (
            <Layout>
                <div>
                    <Row style={{margin:10}}>
                        <Col span={12} style={{textAlign:'center'}}>
                          <span>业主应收：</span>
                          <Input style={{width:100}}></Input>
                        </Col>
                        <Col span={12} style={{textAlign:'center'}}>
                          <span>业主佣金到期日：</span>
                          <Input style={{width:100}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}> 
                        <Col span={12} style={{textAlign:'center'}}>
                          <span>客户应收：</span>
                          <Input style={{width:100}}></Input>
                        </Col>
                        <Col span={12} style={{textAlign:'center'}}>
                          <span>客户佣金到期日：</span>
                          <Input style={{width:100}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={12} style={{textAlign:'center'}}>
                          <span>总成交佣金：</span>
                          <Input style={{width:100}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={3}><Button type='primary'>新增外佣</Button></Col>
                    </Row>
                    <Row>
                        <Table columns={this.appTableColumns}></Table>
                    </Row>
                    <Row>
                        <Col span={3}><Button type='primary'>新增内部分配</Button></Col>
                    </Row>
                    <Row>
                        <Table columns={this.appTable2Columns}></Table>
                    </Row>
                </div>
            </Layout>
        )
    }
}
export default TradePerDis
