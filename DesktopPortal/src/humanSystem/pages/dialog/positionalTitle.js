import React, {Component} from 'react'
import {Row, Col, Modal, DatePicker, Form, Input} from 'antd';
import moment from 'moment'
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
class PositionalTitle extends Component {
    state = {

    }

    handleOk = (e) => {
        let editInfo = this.props.entityInfo || {};
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                if (editInfo.id) {
                    values.id = editInfo.id;
                } else {
                    values.id = NewGuid();
                }
                values.getTitleTime = values.getTitleTime ? values.getTitleTime.format('YYYY-MM-DD') : '';
                console.log('职称信息: ', values);
                if (this.props.confirmCallback) {
                    this.props.confirmCallback(values);
                }
                this.handleCancel();
            }
        });
    }
    handleCancel = () => {
        this.props.form.resetFields();
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
        let editInfo = this.props.entityInfo || {};
        console.log("对象:", editInfo);
        return (
            <Modal title="上单位职位信息" maskClosable={false} width='620px' visible={this.props.showDialog} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="职称" >
                            {getFieldDecorator('titleName', {
                                initialValue: editInfo.titleName,
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
                                initialValue: editInfo.getTitleTime ? moment(editInfo.getTitleTime) : null,
                                rules: [{
                                    required: true,
                                    message: '请选择职称取得时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' disabledDate={current=>current && current > moment().endOf('day')} style={{width: '100%'}} />
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