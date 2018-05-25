//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { getDicParList } from '../../../actions/actionCreator'
import { DatePicker, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import './trade.less'

const { Header, Content } = Layout;
const Option = Select.Option;
const RadioGroup = Radio.Group;
const options = [
    { label: '自行划转', value: '1' },
    { label: '通过资金监管', value: '2' },
];
const FormItem = Form.Item;
const rowstyle = {

}
class TradeContract extends Component {
    state = {

    }
    componentWillMount = () => {
        this.props.dispatch(getDicParList(['COMMISSION_BSWY_CATEGORIES', 'COMMISSION_CJBG_TYPE', 'COMMISSION_JY_TYPE', 'COMMISSION_PAY_TYPE', 'COMMISSION_PROJECT_TYPE', 'COMMISSION_CONTRACT_TYPE', 'COMMISSION_OWN_TYPE', 'COMMISSION_TRADEDETAIL_TYPE', 'COMMISSION_SFZJJG_TYPE']));
    }
    componentWillReceiveProps(props) {

    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log(values)
            }
        });
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        let bswyTypes = this.props.basicData.bswyTypes;
        let cjbgTypes = this.props.basicData.cjbgTypes;
        let tradeTypes = this.props.basicData.tradeTypes;
        let projectTypes = this.props.basicData.projectTypes;
        let tradeDetailTypes = this.props.basicData.tradeDetailTypes;
        let ownTypes = this.props.basicData.ownTypes;
        let payTypes = this.props.basicData.payTypes;
        let contractTypes = this.props.basicData.contractTypes;
        let sfzjjgTypes = this.props.basicData.sfzjjgTypes;

        return (
            <Layout>
                <div style={{ marginLeft: 12 }}>
                    <Row>
                        <Col span={12} pull={1}>
                            <FormItem {...formItemLayout} label={(<span>成交报备</span>)}>
                                {
                                    getFieldDecorator('cjbb', {
                                        rules: [{ required: false }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12} pull={5}>
                            <Button>选择</Button>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>报数物业分类</span>)}>
                                {
                                    getFieldDecorator('bswylx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                bswyTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>成交报告类型</span>)}>
                                {
                                    getFieldDecorator('cjbglx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                cjbgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>公司名称</span>)}>
                                {
                                    getFieldDecorator('gsmc', {
                                        rules: [{ required: true, message: '请填写公司名称!' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>分行名称</span>)}>
                                {
                                    getFieldDecorator('fyzId', {
                                        rules: [{ required: true, message: '请填写分行名称!' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交人</span>)}>
                                {
                                    getFieldDecorator('cjrId', {
                                        rules: [{ required: true, message: '请填写成交人!' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交日期</span>)}>
                                {
                                    getFieldDecorator('cjrq', {
                                        rules: [{ required: true, message: '请选择成交日期!' }],
                                    })(
                                        <DatePicker style={{ width: 200 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('bz', {
                                        rules: [{ required: false }],
                                    })(
                                        <Input.TextArea rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>交易类型</span>)}>
                                {
                                    getFieldDecorator('jylx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>项目类型</span>)}>
                                {
                                    getFieldDecorator('xmlx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                projectTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>详细交易类型</span>)}>
                                {
                                    getFieldDecorator('xxjylx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeDetailTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>产权类型</span>)}>
                                {
                                    getFieldDecorator('cqlx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                ownTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交总价</span>)}>
                                {
                                    getFieldDecorator('cjzj', {
                                        rules: [{ required: true, message: '请选择成交日期!' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>佣金</span>)}>
                                {
                                    getFieldDecorator('yj', {
                                        rules: [{ required: true, message: '请选择成交日期!' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('fkfs', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                payTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>网签日期</span>)}>
                                {
                                    getFieldDecorator('wqrq', {
                                        rules: [{ required: false }],
                                    })(
                                        <DatePicker style={{ width: 200 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>是否资金监管</span>)}>
                                {
                                    getFieldDecorator('sfzjjg', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                sfzjjgTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>客户来访日期</span>)}>
                                {
                                    getFieldDecorator('kflfrq', {
                                        rules: [{ required: false }],
                                    })(
                                        <DatePicker style={{ width: 200 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同签约日期</span>)}>
                                {
                                    getFieldDecorator('htqyrq', {
                                        rules: [{ required: false }],
                                    })(
                                        <DatePicker style={{ width: 200 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同类型</span>)}>
                                {
                                    getFieldDecorator('htlx', {
                                        rules: [{ required: false }],
                                    })(
                                        <RadioGroup>
                                            {
                                                contractTypes.map(tp => <Radio key={tp.key} value={tp.value}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>资金监管协议编号</span>)}>
                                {
                                    getFieldDecorator('jjjgxybh', {
                                        rules: [{ required: false }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>买卖居间合同编号</span>)}>
                                {
                                    getFieldDecorator('mmjjhtbh', {
                                        rules: [{ required: false }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>自制合同</span>)}>
                                {
                                    getFieldDecorator('zzht', {
                                        rules: [{ required: false }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }}>
                            <Button type='primary' onClick={this.handleSave}>保存</Button>
                        </Col>
                    </Row>
                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeContract);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);