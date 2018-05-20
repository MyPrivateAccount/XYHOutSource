//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Span, Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'

const { Header, Content } = Layout;
const Option = Select.Option;

class TradeContract extends Component{
    state = {

    }
    render(){
        return (
            <Layout>
                <Content>
                    <div>
                        <Row>
                            <Col span={12}>
                              <label>成交报备:</label>
                              <Input/>
                            </Col>
                            <Col span={4}>
                              <Button/>
                            </Col>
                        </Row> 
                        <Row>
                            <Col span={24}>
                              <label>报数物业分类：</label>
                              <Checkbox.Group>
                                  <Checkbox>一手商铺</Checkbox>
                                  <Checkbox>二手商铺</Checkbox>
                             </Checkbox.Group>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={24}>
                              <label>成交报告类型：</label>
                              <Checkbox.Group>
                                  <Checkbox>一手商铺</Checkbox>
                                  <Checkbox>二手商铺</Checkbox>
                             </Checkbox.Group>
                            </Col>
                        </Row> 
                        <Row>
                            <Col span={24}>
                              <label>公司名称：</label>
                              <Input>新耀行</Input>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                              <label>分行名称：</label>
                              <Input>新耀行</Input>
                            </Col>
                            <Col span={8}>
                              <label>成交人：</label>
                              <Input>1</Input>
                            </Col>
                            <Col span={8}>
                              <label>成交日期：</label>
                              <Input>2</Input>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={24}>
                              <label>备注：</label>
                              <Input.TextArea rows={4}></Input.TextArea>
                            </Col>
                        </Row>  
                        <Row>
                            <Col span={12}>
                              <label>交易类型：</label>
                              <Checkbox.Group>
                                  <Checkbox>售</Checkbox>
                                  <Checkbox>租</Checkbox>
                             </Checkbox.Group>
                            </Col>
                            <Col span={12}>
                              <label>项目类型：</label>
                              <Select>
                                  <Select.Option>1</Select.Option>
                                  <Select.Option>2</Select.Option>
                              </Select>
                            </Col>
                        </Row>   
                        <Row>
                            <Col span={12}>
                              <label>详细交易类型：</label>
                              <Checkbox.Group>
                                  <Checkbox>售</Checkbox>
                                  <Checkbox>租</Checkbox>
                             </Checkbox.Group>
                            </Col>
                            <Col span={12}>
                              <label>产权类型：</label>
                              <Select>
                                  <Select.Option>1</Select.Option>
                                  <Select.Option>2</Select.Option>
                              </Select>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                              <label>成交总价：</label>
                              <Input></Input>
                            </Col>
                            <Col span={12}>
                              <label>佣金：</label>
                              <Input></Input>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={24}>
                              <label>付款方式：</label>
                              <Select>
                                  <Select.Option>1</Select.Option>
                                  <Select.Option>2</Select.Option>
                              </Select>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                              <label>网签日期：</label>
                              <Input></Input>
                            </Col>
                            <Col span={12}>
                              <label>是否资金监管：</label>
                              <Checkbox.Group>
                                  <Checkbox>是</Checkbox>
                                  <Checkbox>否</Checkbox>
                             </Checkbox.Group>
                            </Col>
                        </Row> 
                        <Row>
                            <Col span={8}>
                              <label>客户来访日期：</label>
                              <Input>新耀行</Input>
                            </Col>
                            <Col span={8}>
                              <label>合同签约日期：</label>
                              <Input>1</Input>
                            </Col>
                            <Col span={8}>
                              <label>合同类型：</label>
                              <Checkbox.Group>
                                  <Checkbox>售</Checkbox>
                                  <Checkbox>租</Checkbox>
                             </Checkbox.Group>
                            </Col>
                        </Row>   
                        <Row>
                            <Col span={8}>
                              <label>资金监管协议编号：</label>
                              <Input>新耀行</Input>
                            </Col>
                            <Col span={8}>
                              <label>买卖居间合同编号：</label>
                              <Input>1</Input>
                            </Col>
                            <Col span={8}>
                              <label>自制合同：</label>
                              <Input></Input>
                            </Col>
                        </Row>                  
                    </div>
                </Content>
            </Layout>
        )
    }
}