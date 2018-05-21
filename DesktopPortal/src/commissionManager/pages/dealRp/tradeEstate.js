//成交物业组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

class TradeEstate extends Component {
    render() {
        return (
            <Layout>
                <div>
                    <Row style={{ textAlign: 'center', marginLeft: 30 }}>
                        <Col span={3}><span>城区</span></Col>
                        <Col span={3}><span>片区</span></Col>
                        <Col span={3}><span>物业名称</span></Col>
                        <Col span={4}><span>位置/栋/座/单元</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>楼层</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>房号</span></Col>
                        <Col span={3} style={{ textAlign: 'left' }}><span>总楼层</span></Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:33 }}>
                        <Col span={4}>
                            <span style={{ marginRight: 5 }}>物业地址:</span>
                            <Select style={{ width: 80 }}></Select>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Select style={{ width: 80 }}></Select>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Input style={{ width: 80 }}></Input>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Input style={{ width: 80 }}></Input>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Input style={{ width: 80 }}></Input>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Input style={{ width: 80 }}></Input>
                        </Col>
                        <Col span={3} style={{ textAlign: 'left' }}>
                            <Input style={{ width: 80 }}></Input>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10 }}>
                        <Col span={24}>
                            <span style={{ marginRight: 5 }}>产证物业地址:</span>
                            <Input style={{ width: 500 }}></Input>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:57 }}>
                        <Col span={24} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>户型:</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>房</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>厅</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>卫</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>阳台</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>露台</span>
                            <Input style={{ width: 40 }}></Input>
                            <span>景观房</span>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={24} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>物业类型:</span>
                            <Select style={{ width: 80 }}></Select>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={24} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>空间类型:</span>
                            <Select style={{ width: 80 }}></Select>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>建筑面积:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>均价:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:47 }}>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>电梯数:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginLeft:-28 }}>每层户数:</span>
                            <Input style={{ width: 120,marginLeft:3 }}></Input>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>装修年代:</span>
                            <Select style={{ width: 120 }}></Select>
                        </Col>
                        <Col span={12} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>家具:</span>
                            <Select style={{ width: 120 }}></Select>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={8} style={{ textAlign: 'left',marginLeft:-10}}>
                            <span style={{ marginRight: 5 }}>产权证取得时间:</span>
                            <Input style={{ width: 120}}></Input>
                        </Col>
                        <Col span={8} style={{ textAlign: 'left',marginLeft:10}}>
                            <span style={{ marginRight: 5 }}>房产按揭号码:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={8} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>是否合租:</span>
                            <Select style={{ width: 120 }}></Select>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={8} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>房源付款方式:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={8} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>房源贷款年限:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={8} style={{ textAlign: 'left',marginLeft:-50}}>
                            <span style={{ marginRight: 5 }}>房源贷款剩余年限:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                    </Row>
                    <Row style={{ textAlign: 'left', margin: 10,marginLeft:35 }}>
                        <Col span={8} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>房源贷款金额:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                        <Col span={8} style={{ textAlign: 'left'}}>
                            <span style={{ marginRight: 5 }}>房源已还金额:</span>
                            <Input style={{ width: 120 }}></Input>
                        </Col>
                    </Row>
                </div>
            </Layout>
        )
    }
}
export default TradeEstate
