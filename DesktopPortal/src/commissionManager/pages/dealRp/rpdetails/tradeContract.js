//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import './trade.less'

const { Header, Content } = Layout;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const options = [
    { label: '一手商铺', value: '1' },
    { label: '二手商铺', value: '2' },
    { label: '一手写字楼', value: '3' },
];
const FormItem = Form.Item;
const rowstyle = {
    
}
class TradeContract extends Component {
    state = {

    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <Layout>
                <div style={{marginLeft:12}}>
                    <Row style={{margin:10}}>
                        <Col span={12}>
                           <span>成交报备:</span>
                           <Input style={{width:200,marginLeft:5}}></Input>
                        </Col>
                        <Col span={4} pull={6}>
                            <Button type='primary'>选择</Button>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={12}>
                            <label>报数物业分类:</label>
                        </Col>
                        <Col span={12} pull={10}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={12}>
                            <label>成交报告类型：</label>
                        </Col>
                        <Col span={12} pull={10}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:32}}>
                        <Col span={24}>
                            <label>公司名称：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8}>
                            <label>分行(组)名称：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>成交人：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>成交日期：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:54}}>
                        <Col span={24}>
                            <label>备注：</label>
                            <Input.TextArea rows={4} style={{width:510}}></Input.TextArea>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={2} style={{textAlign:'right'}}>
                            <label style={{marginRight:12}}>交易类型:</label>
                        </Col>
                        <Col span={8} style={{textAlign:'left'}}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={2} style={{textAlign:'right'}}>
                            <label style={{marginRight:12}}>项目类型:</label>
                        </Col>
                        <Col span={8} style={{textAlign:'left'}}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={12}>
                            <label>详细交易类型:</label>
                        </Col>
                        <Col span={12} pull={10}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={2} style={{textAlign:'right'}}>
                            <label style={{marginRight:12}}>产权类型:</label>
                        </Col>
                        <Col span={8} style={{textAlign:'left'}}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:32}}>
                        <Col span={8}>
                            <label>成交总价：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>佣金：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={2} style={{textAlign:'right'}}>
                            <label style={{marginRight:12}}>付款方式:</label>
                        </Col>
                        <Col span={8} style={{textAlign:'left'}}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:32}}>
                        <Col span={24}>
                            <label>网签日期：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                       <Col span={12}>
                            <label>是否资金监管:</label>
                        </Col>
                        <Col span={12} pull={10}>
                             <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10}}>
                        <Col span={8}>
                            <label>客户来访日期：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>合同签约日期：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={2}>
                            <label>合同类型：</label>
                        </Col>
                        <Col span={6}>
                            <RadioGroup options={options} defaultValue={['Pear']}/>
                        </Col>
                    </Row>
                    <Row style={{margin:10,marginLeft:-12}}>
                        <Col span={8}>
                            <label>资金监管协议编号：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>买卖居间合同编号：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                        <Col span={8}>
                            <label>自制合同：</label>
                            <Input style={{width:200}}></Input>
                        </Col>
                    </Row>
                </div>
            </Layout>
        )
    }
}
const WrappedRegistrationForm = Form.create()(TradeContract);
export default WrappedRegistrationForm