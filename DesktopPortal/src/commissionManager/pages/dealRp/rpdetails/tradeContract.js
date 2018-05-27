//交易合同组件
//合同列表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { getDicParList,dealRpSave } from '../../../actions/actionCreator'
import {notification, DatePicker, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import './trade.less'

const RadioGroup = Radio.Group;
const FormItem = Form.Item;
class TradeContract extends Component {
    state = {
        isDataLoading:false
    }
    componentWillMount = () => {
        this.setState({isDataLoading:true,tip:'信息初始化中...'})
        this.props.dispatch(getDicParList(['COMMISSION_BSWY_CATEGORIES', 'COMMISSION_CJBG_TYPE', 'COMMISSION_JY_TYPE', 'COMMISSION_PAY_TYPE', 'COMMISSION_PROJECT_TYPE', 'COMMISSION_CONTRACT_TYPE', 'COMMISSION_OWN_TYPE', 'COMMISSION_TRADEDETAIL_TYPE', 'COMMISSION_SFZJJG_TYPE']));
    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'HTSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告交易合同信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                values.cjrq = this.state.cjrq;
                values.yxsqyrq = this.state.yxsqyrq;
                values.yjfksj = this.state.yjfksj;
                values.kflfrq = this.state.kflfrq;
                values.htqyrq = this.state.htqyrq;

                console.log(values);
                this.setState({isDataLoading:true,tip:'保存信息中...'})
                this.props.dispatch(dealRpSave(values));
            }
        });
    }
    cjrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({cjrq:dateString})
    }
    wqrq_dateChange=(value,dateString)=>{
        console.log(dateString)
        this.setState({yxsqyrq:dateString})
    }
    yjfksj_dateChange=(value,dateString)=>{
        this.setState({yjfksj:dateString})
    }
    kflfrq_dateChange=(value,dateString)=>{
        this.setState({kflfrq:dateString})
    }
    htqyrq_dateChange=(value,dateString)=>{
        this.setState({htqyrq:dateString})
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
                                        <Input style={{ width: 200 }}></Input>
                                }
                            </FormItem>
                        </Col>
                        <Col span={12} pull={5}>
                            <Button>选择</Button>
                        </Col>
                    </Row>
                    <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                    <Row>
                        <Col span={24} pull={4}>
                            <FormItem {...formItemLayout} label={(<span>报数物业分类</span>)}>
                                {
                                    getFieldDecorator('bswylx', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                bswyTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                cjbgTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        rules: [{ required: false, message: '请填写公司名称!' }],
                                        initialValue: '',
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
                                        rules: [{ required: false, message: '请填写分行名称!' }],
                                        initialValue: '',
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
                                        rules: [{ required: false, message: '请填写成交人!' }],
                                        initialValue: '',
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
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: '',
                                    })(
                                        <DatePicker style={{ width: 200 }}  onChange={this.cjrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>成交报告编号</span>)}>
                                {
                                    getFieldDecorator('cjbgbh', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>附加说明</span>)}>
                                {
                                    getFieldDecorator('fjsm', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <Input style={{ width: 200 }}></Input>
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
                                        initialValue: '',
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                projectTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                tradeDetailTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                ownTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>佣金</span>)}>
                                {
                                    getFieldDecorator('ycjyj', {
                                        rules: [{ required: false, message: '请选择成交日期!' }],
                                        initialValue: 0,
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                payTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
                                            }
                                        </RadioGroup>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>网签日期</span>)}>
                                {
                                    getFieldDecorator('yxsqyrq', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.wqrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>预计放款日期</span>)}>
                                {
                                    getFieldDecorator('yjfksj', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.yjfksj_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>预计放款金额</span>)}>
                                {
                                    getFieldDecorator('yjfkje', {
                                        rules: [{ required: false }],
                                        initialValue: 0,
                                    })(
                                        <Input style={{ width: 200 }}></Input>
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
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                sfzjjgTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.kflfrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同签约日期</span>)}>
                                {
                                    getFieldDecorator('htqyrq', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <DatePicker style={{ width: 200 }} onChange={this.htqyrq_dateChange}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={8}>
                            <FormItem {...formItemLayout} label={(<span>合同类型</span>)}>
                                {
                                    getFieldDecorator('htlx', {
                                        rules: [{ required: false }],
                                        initialValue: '',
                                    })(
                                        <RadioGroup>
                                            {
                                                contractTypes.map(tp => <Radio key={tp.key} value={tp.key}>{tp.key}</Radio>)
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
                                        initialValue: '',
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
                                        initialValue: '',
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
                                        initialValue: '',
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
                    </Spin>
                </div>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo:state.rp.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradeContract);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);