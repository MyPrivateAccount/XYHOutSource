//按揭过户组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

class TradeTransfer extends Component {
    render() {
        return (
            <Layout>
                <div>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>所属区域：</span>
                            <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                        <Col span={6}/>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>付款方式：</span>
                            <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>是否担保：</span>
                            <Select style={{width:100,marginLeft:5}}></Select>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>担保公司：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>贷款银行：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>保费金额：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6}/>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>贷款支行：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>贷款成数：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>贷款金额：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>贷款年限：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>评估公司：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>监管金额：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>业主欠款：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>赎楼罚息：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>赎楼利息：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>担保金额：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center',marginLeft:-8}}>
                            <span>抵押回执号：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>过户回执号：</span>
                            <Input style={{width:100,marginLeft:7}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center',marginLeft:8}}>
                            <span>自找银行：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>放款日期：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>解保日期：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6}>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>过户日期：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={12} style={{textAlign:'center',marginLeft:-42}}>
                            <span>过户产证地址：</span>
                            <Input style={{width:300,marginLeft:5}}></Input>
                        </Col>
                        <Col span={1}>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>过户价格：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center',marginLeft:-10}}>
                            <span>过户业主电话：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                        <Col span={6} style={{textAlign:'center'}}>
                            <span>过户客户电话：</span>
                            <Input style={{width:100,marginLeft:5}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:5,marginLeft:60}}>
                        <Col span={24}>
                            <label>备注：</label>
                            <Input.TextArea rows={4} style={{width:510}}></Input.TextArea>
                        </Col>
                    </Row>
                </div>
            </Layout>
        )
    }
}
export default TradeTransfer