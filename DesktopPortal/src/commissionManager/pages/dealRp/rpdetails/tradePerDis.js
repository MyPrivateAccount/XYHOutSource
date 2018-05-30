//业绩分配组件
import { connect } from 'react-redux';
import React, { Component } from 'react';
import moment from 'moment'
import {DatePicker,notification, Form, Span, Layout, Table, Button, Radio, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import TradeWyTable from './tradeWyTable'
import TradeNTable from './tradeNTable'

const FormItem = Form.Item;
class TradePerDis extends Component {

    state={
        isDataLoading:false,
        rpData:{}
    }
    componentWillMount = () => {

    }
    componentWillReceiveProps(newProps) {
        this.setState({ isDataLoading: false });
        if(newProps.operInfo.operType === 'FPSAVE_UPDATE'){
            notification.success({
                message: '提示',
                description: '保存成交报告业绩分配信息成功!',
                duration: 3
            });
            newProps.operInfo.operType = ''
        }
        else if(newProps.operInfo.operType === 'FPGET_UPDATE'){//信息获取成功
            this.setState({ rpData: newProps.ext});
            newProps.operInfo.operType = ''
        }
    }
    handleSave = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = this.props.rpId;
                console.log(values);
            }
        });
    }
    handleAddWy = (e) => {
        e.preventDefault();
        this.wytb.handleAdd();
    }
    onWyTableRef = (ref) => {
        this.wytb = ref
    }
    getInvalidDate=(dt)=>{
        var newdt = ''+dt;
        if(newdt.indexOf('T')!==-1){
            newdt = newdt.substr(0,newdt.length-9);
            console.log("newdt:"+newdt)
            return newdt;
        }
        return dt
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const formItemLayout2 = {
            labelCol: { span: 10 },
            wrapperCol: { span: 10 },
        };
        return (
            <Layout>
                <Spin spinning={this.state.isDataLoading} tip={this.state.tip}>
                <Row>
                    <Col span={8}>
                        <FormItem {...formItemLayout} label={(<span>业主应收</span>)}>
                            {
                                getFieldDecorator('yjYzys', {
                                    initialValue: this.state.rpData.yjYzys,
                                })(
                                    <Input style={{ width: 200 }}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                    <Col span={8}>
                        <FormItem {...formItemLayout2} label={(<span>业主佣金到期日</span>)}>
                            {
                                getFieldDecorator('yjYzyjdqr', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: moment(this.getInvalidDate(this.state.rpData.yjYzyjdqr)),
                                })(
                                    <DatePicker style={{ width: 200 }}></DatePicker>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={8}>
                        <FormItem {...formItemLayout} label={(<span>客户应收</span>)}>
                            {
                                getFieldDecorator('yjKhys', {
                                    initialValue: this.state.rpData.yjKhys,
                                })(
                                    <Input style={{ width: 200 }}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                    <Col span={8}>
                        <FormItem {...formItemLayout2} label={(<span>客户佣金到期日</span>)}>
                            {
                                getFieldDecorator('yjKhyjdqr', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: moment(this.getInvalidDate(this.state.rpData.yjKhyjdqr)),
                                })(
                                    <DatePicker style={{ width: 200 }}></DatePicker>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={4}>
                        <FormItem {...formItemLayout} label={(<span>总成交佣金</span>)}>
                            {
                                getFieldDecorator('yjZcjyj', {
                                    rules: [{ required: false, message: '请选择成交日期!' }],
                                    initialValue: this.state.rpData.yjZcjyj,
                                })(
                                    <Input style={{ width: 200 }}></Input>
                                )
                            }
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={3}><Button type='primary' onClick={this.handleAddWy}>新增外佣</Button></Col>
                </Row>
                <Row>
                    <TradeWyTable onWyTableRef={this.onWyTableRef}/>
                </Row>
                <Row>
                    <Col span={3}><Button type='primary'>新增内部分配</Button></Col>
                </Row>
                <Row>
                    <TradeNTable/>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }}>
                        <Button type='primary' onClick={this.handleSave}>保存</Button>
                    </Col>
                </Row>
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        operInfo: state.rp.operInfo,
        ext:state.rp.ext
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(TradePerDis);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);
