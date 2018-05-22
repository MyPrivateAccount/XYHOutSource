//付款组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Select, Row, Col, Form, Input, Tooltip, Button, Modal, Layout,Tabs,DatePicker } from 'antd'

class FKCp extends Component{
    render(){
        return(
            <Layout>
                <Layout.Content>
                    <Row style={{margin:5}}>
                        <Col span={8} style={{marginLeft:25}}>
                          <label>
                              <span style={{ marginRight: '10px' }}>部门</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                        <Col span={8} style={{marginLeft:110}}>
                          <label>
                              <span style={{ marginRight: '10px' }}>收款人</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>收据日期</span>
                              <DatePicker style={{width:120}}></DatePicker>
                          </label>
                        </Col>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>付款日期</span>
                              <DatePicker style={{width:120}}></DatePicker>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={8} style={{marginLeft:24}}>
                          <label>
                              <span style={{ marginRight: '10px' }}>金额</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                        <Col span={8} style={{marginLeft:124}}>
                          <label>
                              <span style={{ marginRight: '10px' }}>用途</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>付款帐号</span>
                              <Select style={{width:80}}></Select>
                          </label>
                        </Col>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>付款方式</span>
                              <Select style={{width:80}}></Select>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={24} style={{marginLeft:10}}>
                          <label>
                              <span style={{ marginRight: '10px' }}>付款人</span>
                              <Input style={{width:80}}></Input>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={24} style={{marginLeft:22}}>
                            <label><span style={{ marginRight: '10px' }}>备注</span>
                            <Input.TextArea rows={4} style={{width:510}}></Input.TextArea>
                            </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>买方/承租方</span>
                              <Input style={{width:120}}></Input>
                          </label>
                        </Col>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>[买]/证件号码</span>
                              <Select style={{width:120}}></Select>
                          </label>
                        </Col>
                    </Row>
                    <Row style={{margin:5}}>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>卖方/承租方</span>
                              <Input style={{width:120}}></Input>
                          </label>
                        </Col>
                        <Col span={12}>
                          <label>
                              <span style={{ marginRight: '10px' }}>[卖]/证件号码</span>
                              <Select style={{width:120}}></Select>
                          </label>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
export default FKCp