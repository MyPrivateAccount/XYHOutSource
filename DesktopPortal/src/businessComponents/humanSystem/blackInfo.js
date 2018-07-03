import React, {Component} from 'react'
import {Row, Col, Checkbox, Select, Form, Input, Icon} from 'antd';
const FormItem = Form.Item;
const Option = Select.Option;
const {TextArea} = Input;

class BlackInfo extends Component {
    state = {

    }

    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'black')
        }
    }

    onIdCardBlur = (e) => {
        let idCard = e.target.value;
        if (idCard.length > 0 && !this.props.form.getFieldError('idcards')) {
            let sex = (idCard.substr(17, 1) % 2 == 1 ? "1" : "2");//1:男,2:女
            this.props.form.setFieldsValue({sex: sex});
        }
    }

    render() {
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 17},
        };
        let blackInfo = this.props.entityInfo || {};
        let disabled = (this.props.readOnly || false);
        return (
            <div>
                <div className="page-title" style={{marginBottom: '10px'}}>黑名单</div>
                <Form >
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="姓名">
                                {getFieldDecorator('name', {
                                    initialValue: blackInfo.name,
                                    rules: [{required: true, message: '请输入姓名'}]
                                })(
                                    <Input disabled={disabled} placeholder="请输入姓名" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="身份证号" >
                                {getFieldDecorator('idCard', {
                                    initialValue: blackInfo.idCard,
                                    rules: [{
                                        required: true, message: '请输入身份证号'
                                    }, {
                                        pattern: /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/, message: '不是有效的身份证号'
                                    }]
                                })(
                                    <Input disabled={disabled} onBlur={this.onIdCardBlur} placeholder="请输入身份证号码" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="手机号码" >
                                {getFieldDecorator('phone', {
                                    initialValue: blackInfo.phone,
                                    rules: [{
                                        required: true, message: '请输入手机号码',
                                    }, {pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!'}]
                                })(
                                    <Input disabled={disabled} placeholder="请输入手机号码" />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="性别">
                                {getFieldDecorator('sex', {
                                    initialValue: blackInfo.sex,
                                    rules: [{required: true, message: '请选择性别'}]
                                })(
                                    <Select disabled={disabled} placeholder="选择性别">
                                        <Option key='1' value="1">男</Option>
                                        <Option key='2' value="2">女</Option>
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="Email地址" >
                                {getFieldDecorator('email', {
                                    initialValue: blackInfo.email,
                                    rules: [{type: 'email', message: '请输入正确的email地址'}]
                                })(
                                    <Input disabled={disabled} placeholder={disabled ? "" : "请输入Email地址"} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}></Col>
                    </Row>
                    <Row>
                        <Col span={14}>
                            <FormItem labelCol={{span: 3}} wrapperCol={{span: 21}} label="备注">
                                {getFieldDecorator('reason', {
                                    initialValue: blackInfo.reason,
                                    rules: []
                                })(
                                    <TextArea disabled={disabled} rows={4} placeholder={disabled ? "" : "请输入备注"} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>

                </Form>
            </div >
        )
    }
}

const WrappedRegistrationForm = Form.create()(BlackInfo);
export default WrappedRegistrationForm;