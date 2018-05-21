//客户信息组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

class TradeCustomer extends Component{
    render(){
        return (
            <Layout>
                <div style={{marginLeft:20}}>
                    <Row style={{margin:10}}>
                        <Col span={8}>
                          <span>名称:</span>
                          <Input style={{width:120,marginLeft:5}}></Input>
                        </Col>
                        <Col span={8}>
                          <span>身份证:</span>
                          <Input style={{width:200,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8}>
                          <span>地址:</span>
                          <Input style={{width:250,marginLeft:5}}></Input>
                        </Col>
                        <Col span={8} style={{marginLeft:10}}>
                          <span>手机:</span>
                          <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{marginLeft:25}}>
                          <span>客户来源:</span>
                          <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8} style={{marginLeft:-22}}>
                          <span>客户性质:</span>
                          <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                        <Col span={8} style={{marginLeft:20}}>
                          <span>客源号:</span>
                          <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{marginLeft:35}}>
                          <span>置业次数:</span>
                          <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8} style={{marginLeft:-11}}>
                          <span>代理人:</span>
                          <Input style={{width:250,marginLeft:5}}></Input>
                        </Col>
                        <Col span={8} style={{marginLeft:12}}>
                          <span>代理人联系方式:</span>
                          <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6}>
                          <span>代理人证件号码:</span>
                          <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8} style={{marginLeft:-21}}>
                          <span>成交原因:</span>
                          <Input style={{width:250,marginLeft:5}}></Input>
                        </Col>
                        <Col span={8}>
                          <span>从登记至签合同时长:</span>
                          <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                    </Row>
                </div>
            </Layout>
        )
    }
}
export default TradeCustomer
