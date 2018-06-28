import React, {Component} from 'react'
import {Row, Col, Modal, DatePicker, Form, Input} from 'antd';
import moment from 'moment'
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
class FormerCompany extends Component {
    state = {

    }
    handleOk = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                console.log('前公司信息: ', values);
                if (this.props.confirmCallback) {
                    this.props.confirmCallback(values);
                }
                this.handleCancel();
            }
        });
    }
    handleCancel = () => {
        if (this.props.closeDialog) {
            this.props.closeDialog();
        }
    }

    render() {

        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 17},
        };
        return (
            <Modal title="上单位职位信息" maskClosable={false} bodyStyle={{width: '600px !important'}} style={{width: '600px !important'}} visible={this.props.showDialog} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="所在单位" >
                            {getFieldDecorator('company', {
                                rules: [{
                                    required: true, message: '请输入所在单位',
                                }]
                            })(
                                <Input placeholder="请输入所在单位" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="岗位" >
                            {getFieldDecorator('position', {
                                rules: [{
                                    required: true, message: '请输入岗位',
                                }]
                            })(
                                <Input placeholder="请输入岗位" />
                            )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="证明人" >
                            {getFieldDecorator('Witness', {
                                rules: [{
                                    required: true, message: '请输入证明人',
                                }]
                            })(
                                <Input placeholder="请输入证明人" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="证明人联系电话" >
                            {getFieldDecorator('WitnessPhone', {
                                rules: [{
                                    required: true, message: '证明人联系电话',
                                }, {pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!'}]
                            })(
                                <Input placeholder="证明人联系电话" />
                            )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="起始时间">
                            {getFieldDecorator('startTime', {
                                initialValue: moment(),
                                rules: [{
                                    required: true,
                                    message: '请选择起始时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="终止时间">
                            {getFieldDecorator('endTime', {
                                initialValue: moment(),
                                rules: [{
                                    required: true,
                                    message: '请选择终止时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
            </Modal>
        )
    }
}

const WrappedRegistrationForm = Form.create()(FormerCompany);
export default WrappedRegistrationForm;