import React, { Component } from 'react'
import { Modal, Button, Form, Icon, Input } from 'antd';
import { connect } from 'react-redux'
import { resetPassword } from '../actions/actionCreators'

const FormItem = Form.Item;


class ModifyPassword extends Component {
    state = {
        visible: false,
        confirmDirty: false
    }
    handleOk = (e) => {
        this.props.form.validateFieldsAndScroll((err, values) => {
            if (!err) {
                this.setState({
                    visible: false,
                });
                console.log('Received values of form: ', values);
                this.props.resetPassword({code: this.props.code, entity: values})
            }
        });
    }
    handleCancel = (e) => {
        console.log(e);
        this.setState({
            visible: false,
        });
        if (this.props.onVisibleChange) {
            this.props.onVisibleChange(false);
        }
    }
    componentWillReceiveProps = (nextProps) => {
        if (nextProps.visible !== this.state.visible) {
            this.setState({
                visible: nextProps.visible,
            });
            if(nextProps.visible){
                this.props.form.resetFields();
            }
        }
    }

    handleConfirmBlur = (e) => {
        const value = e.target.value;
        this.setState({ confirmDirty: this.state.confirmDirty || !!value });
    }
    checkPassword = (rule, value, callback) => {
        const form = this.props.form;
        if (value && value !== form.getFieldValue('newPassword')) {
            callback('两次密码输入必须一致!');
        } else {
            callback();
        }
    }
    checkConfirm = (rule, value, callback) => {
        const form = this.props.form;
        if (value && this.state.confirmDirty) {
            form.validateFields(['confirmPassword'], { force: true });
        }
        callback();
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const lenValidator = [{ min: 6, message: '密码长度不得小于6位' }, { max: 24, message: '密码长度不得大于24位' }]
        return (
            <Modal
                title="修改密码"
                visible={this.state.visible}
                onOk={this.handleOk}
                onCancel={this.handleCancel}
            >
                <Form ref={(e) => this.form = e}>
                    <FormItem hasFeedback>
                        {getFieldDecorator('oldPassword', {
                            rules: [{ required: true, message: '请输入原密码' },
                            ...lenValidator],
                        })(
                            <Input type="password" prefix={<Icon type="key" style={{ fontSize: 13 }} />} placeholder="原密码" />
                            )}
                    </FormItem>
                    <FormItem hasFeedback>
                        {getFieldDecorator('newPassword', {
                            rules: [{ required: true, message: '请设置新的密码' },
                            ...lenValidator,
                            { validator: this.checkConfirm }],
                        })(
                            <Input prefix={<Icon type="lock" style={{ fontSize: 13 }} />} type="password" placeholder="新密码" />
                            )}
                    </FormItem>
                    <FormItem hasFeedback>
                        {getFieldDecorator('confirmPassword', {
                            rules: [{ required: true, message: '请再次输入您的新密码' },
                            ...lenValidator,
                            { validator: this.checkPassword }],
                        })(
                            <Input prefix={<Icon type="lock" style={{ fontSize: 13 }} />} type="password" placeholder="密码确认" onBlur={this.handleConfirmBlur} />
                            )}
                    </FormItem>
                </Form>
            </Modal>
        )
    }
}


const mapStateToProps = (state, props) => {
    return {
      code: state.oidc.user.profile.name
    }
}
const mapDispatchToProps = (dispatch) => {
    return {
        resetPassword: (...args) => dispatch(resetPassword(...args))
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(Form.create()(ModifyPassword));
