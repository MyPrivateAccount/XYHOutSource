//付款组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Select, Row, Col, Form, Input, Tooltip, Button, Modal, Layout, Tabs, DatePicker } from 'antd'
import moment from 'moment'
const FormItem = Form.Item;

class FKCp extends Component {

    componentDidMount(){
        this.props.onFKCp(this)
        this.loadData()
    }
    //加载数据
    loadData=()=>{
        let dt = this.props.sfkinfo;
        const dateFormat = 'YYYY-MM-DD'
        let now = moment(moment().format('YYYY-MM-DD'),dateFormat)
        console.log(now)
        if(dt!==null){
            this.props.form.setFieldsValue({'sectionId':dt.sectionId})
            this.props.form.setFieldsValue({'skr':dt.skr})
            this.props.form.setFieldsValue({'sjrq':dt.sjrq!==undefined?dt.sjrq:now})
            this.props.form.setFieldsValue({'jzrq':dt.jzrq!==undefined?dt.sjrq:now})
            this.props.form.setFieldsValue({'je':dt.je})
            this.props.form.setFieldsValue({'yt':dt.yt})
            this.props.form.setFieldsValue({'gszh':dt.gszh})
            this.props.form.setFieldsValue({'skfs':dt.skfs})
            this.props.form.setFieldsValue({'jzdrzh':dt.jzdrzh})
            this.props.form.setFieldsValue({'bz':dt.bz})
            this.props.form.setFieldsValue({'buy':dt.buy})
            this.props.form.setFieldsValue({'buyCode':dt.buyCode})
            this.props.form.setFieldsValue({'sell':dt.sell})
            this.props.form.setFieldsValue({'sellCode':dt.sellCode})
        }
    }
    //获取页面数据
    getData=()=>{
        let rs = null
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.dsdfType = 2
                values.sjrq = values.sjrq.format('YYYY-MM-DD')
                values.jzrq = values.jzrq.format('YYYY-MM-DD')
                rs =  values
            }
          });
        return rs
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <Layout>
                <Layout.Content>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>部门</span>)}>
                                {
                                    getFieldDecorator('sectionId', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }} disabled={true}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款人</span>)}>
                                {
                                    getFieldDecorator('skr', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }} disabled={true}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>收据日期</span>)}>
                                {
                                    getFieldDecorator('sjrq', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <DatePicker style={{ width: 120 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款日期</span>)}>
                                {
                                    getFieldDecorator('jzrq', {
                                        rules: [{ required: true, message: '请输入付款日期' }],
                                    })(
                                        <DatePicker style={{ width: 120 }}></DatePicker>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>金额</span>)}>
                                {
                                    getFieldDecorator('je', {
                                        rules: [{ required: true, message: '请输入金额' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>用途</span>)}>
                                {
                                    getFieldDecorator('yt', {
                                        rules: [{ required: false, message: '请输入交款方' }],
                                    })(
                                        <Select style={{ width: 80 }}>
                                         <Select.Option key={'1'} value={'业主佣金'}>业主佣金</Select.Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款账户</span>)}>
                                {
                                    getFieldDecorator('gszh', {
                                        rules: [{ required: false, message: '请输入交款方' }],
                                    })(
                                        <Select style={{ width: 80 }}>
                                          <Select.Option key={'1'} value={'12345678'}>新耀行</Select.Option>
                                          <Select.Option key={'2'} value={'45678909'}>龙山行</Select.Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>付款方式</span>)}>
                                {
                                    getFieldDecorator('skfs', {
                                        rules: [{ required: false, message: '请输入交款方' }],
                                    })(
                                        <Select style={{ width: 80 }}>
                                           <Select.Option key={'1'} value={'转账'}>转账</Select.Option>
                                          <Select.Option key={'2'} value={'现金'}>现金</Select.Option>
                                          <Select.Option key={'3'} value={'刷卡'}>刷卡</Select.Option>
                                          <Select.Option key={'4'} value={'支票'}>支票</Select.Option>
                                        </Select>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>付款人</span>)}>
                                {
                                    getFieldDecorator('jzdrzh', {
                                        rules: [{ required: true, message: '请输入付款人' }],
                                    })(
                                        <Input style={{ width: 80 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('bz', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input.TextArea rows={4} style={{ width: 510 }}></Input.TextArea>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>买方/承租方</span>)}>
                                {
                                    getFieldDecorator('buy', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>[买]/证件号码</span>)}>
                                {
                                    getFieldDecorator('buyCode', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>卖方/承租方</span>)}>
                                {
                                    getFieldDecorator('sell', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            <FormItem {...formItemLayout} label={(<span>[卖]/证件号码</span>)}>
                                {
                                    getFieldDecorator('sellCode', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        ext: state.rp.ext,
        operInfo: state.rp.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(FKCp);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);