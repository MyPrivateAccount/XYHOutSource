import React, { Component } from 'react';
import { Row, Col, Button, Icon, DatePicker, Input, Select } from 'antd';
import './search.less'

class DRpSearchCondition extends Component {

    state = {
        expandSearchCondition: false
    }

    handleSearchBoxToggle = (e) => {//筛选条件展开、收缩
        let visible = !this.state.expandSearchCondition;
        this.setState({ expandSearchCondition: visible });
    }
    handleCreateTime = (e, field) => {

    }
    render() {
        let expandSearchCondition = this.state.expandSearchCondition;
        return (
            <div className='searchCondition'>
                <Row>
                    <Col span={12}>
                        <span>所有报告></span>
                    </Col>
                    <Col span={4}>
                        <Button onClick={this.handleSearchBoxToggle}>{expandSearchCondition ? "收起筛选" : "展开筛选"}<Icon type={expandSearchCondition ? "up-square-o" : "down-square-o"} /></Button>
                    </Col>
                </Row>
                <div style={{ display: expandSearchCondition ? "block" : "none" }}>
                    <Row className="normalInfo">
                        <Col span={12}>
                            <label><span style={{ marginRight: '10px' }}>审批通过日期：</span>
                                <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交编号</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>定金编号</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>片区</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>所属部门</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={4}>
                            <label>
                                <span style={{ marginRight: '10px' }}>客户名称</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={4}>
                        </Col>
                        <Col span={12}>
                            <label><span style={{ marginRight: '10px' }}>上业绩日期：</span>
                                <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateStart')} />- <DatePicker disabledDate={this.disabledDate} onChange={(e, dateString) => this.handleCreateTime(dateString, 'createDateEnd')} />
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>报告类型</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>交易类型</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交状态</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>报数物业分类</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>付款方式</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>合同编号</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>过户状态</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>客户信息来源</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘名称</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘栋数</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>楼盘房号</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>物业类型</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                    </Row>
                    <Row className="normalInfo">
                        <Col span={6} style={{marginLeft:10}}>
                            <label>
                                <span style={{ marginRight: '10px' }}>录入人</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6}>
                            <label>
                                <span style={{ marginRight: '10px' }}>成交人</span>
                                <Input style={{ width: 80 }}></Input>
                            </label>
                        </Col>
                        <Col span={6} style={{marginLeft:-10}}>
                            <label>
                                <span style={{ marginRight: '10px' }}>审批状态</span>
                                <Select style={{ width: 80 }}></Select>
                            </label>
                        </Col>
                        <Col span={6}>
                        </Col>
                    </Row>
                </div>
            </div>
        )
    }
}
export default DRpSearchCondition