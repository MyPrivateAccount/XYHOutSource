//收款组件
import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Select, Row, Col, Form, Input, Tooltip, Button, Modal, Layout, Tabs, DatePicker } from 'antd'

const FormItem = Form.Item;

class SJCp extends Component {

    constructor(props) {
        super(props)
        this.props.onSJCp(this)
    }
    componentDidMount() {
        this.loadData()
    }
    //加载数据
    loadData = () => {
        let dt = this.props.sfkinfo;
        if (dt !== null) {
            this.props.form.setFieldsValue({ 'sjhm': dt.sjhm })
            this.props.form.setFieldsValue({ 'qtsj': dt.qtsj })
            this.props.form.setFieldsValue({ 'sjbz': dt.sjbz })
        }
    }
    //获取页面数据
    getData = () => {
        let rs = null
        this.props.form.validateFields((err, values) => {
            if (!err) {
                rs = values
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
                            <FormItem {...formItemLayout} label={(<span>收据号码</span>)}>
                                {
                                    getFieldDecorator('sjhm', {
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
                            <FormItem {...formItemLayout} label={(<span>其它收据</span>)}>
                                {
                                    getFieldDecorator('qtsj', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input style={{ width: 200 }}></Input>
                                    )
                                }
                            </FormItem>
                        </Col>
                    </Row>
                    <Row style={{ margin: 5 }}>
                        <Col span={12} >
                            <FormItem {...formItemLayout} label={(<span>备注</span>)}>
                                {
                                    getFieldDecorator('sjbz', {
                                        rules: [{ required: false, message: '' }],
                                    })(
                                        <Input.TextArea rows={4} style={{ width: 510 }}></Input.TextArea>
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
const WrappedRegistrationForm = Form.create()(SJCp);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);