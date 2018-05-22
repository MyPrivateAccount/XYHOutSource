//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'

class TradePerDis extends Component{
    
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
                        <TradeWyTable/>
                    </Row>
                    <Row>
                        <Col span={3}><Button type='primary'>新增内部分配</Button></Col>
                    </Row>
                    <Row>
                        <TradeNTable/>
                    </Row>
                </div>
            </Layout>
        )
    }
}
export default TradePerDis
