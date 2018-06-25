import React, {Component} from 'react'
import {Row, Col, Modal, DatePicker, Form, Input} from 'antd';
import moment from 'moment'
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
class PositionalTitle extends Component {
    state = {

    }

    handleOk = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id=NewGuid();
                console.log('职称信息: ', values);
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
            <Modal title="上单位职位信息" maskClosable={false} style={{width: '600px !important'}} visible={this.props.showDialog} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="职称" >
                            {getFieldDecorator('titleName', {
                                rules: [{
                                    required: true, message: '请输入职称',
                                }]
                            })(
                                <Input placeholder="请输入职称" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="取得时间">
                            {getFieldDecorator('getTitleTime', {
                                rules: [{
                                    required: true,
                                    message: '请选择职称取得时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}></Col>
                </Row>
            </Modal>
        )
    }
}

const WrappedRegistrationForm = Form.create()(PositionalTitle);
export default WrappedRegistrationForm;