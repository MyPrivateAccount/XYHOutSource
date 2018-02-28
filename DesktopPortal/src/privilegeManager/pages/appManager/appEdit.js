import { connect } from 'react-redux';
import { appAdd, appEdit, appDataSave, appDialogClose } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { notification, Modal, Row, Col, Form, Input, Select } from 'antd'
import { ApplicationTypes } from '../../../constants/baseConfig'

const FormItem = Form.Item;
const Option = Select.Option;

class AppEditor extends Component {
    state = {
        visible: false,
        appInfo: {},
        dialogTitle: '',
        //isShowClietnSecret: false,
    }
    componentWillReceiveProps(newProps) {
        let { operType, result } = newProps.operInfo;
        let { activeApp } = newProps;
        console.log("operType:", operType);
        if (this.state.appInfo.clientId !== activeApp.clientId || activeApp.clientId === undefined) {
            if (operType === 'edit') {
                this.setState({ appInfo: activeApp, visible: true, dialogTitle: '应用编辑' });
            } else if (operType === 'add') {
                let appInfo = {};
                if (this.state.appInfo.applicationType) {
                    appInfo.applicationType = this.state.appInfo.applicationType;
                }
                this.setState({ appInfo: appInfo, visible: true, dialogTitle: '应用新增' });
            }
        }
        if (!['edit', 'add'].includes(operType)) {
            this.props.form.resetFields();
            this.setState({ visible: false, appInfo: {} });
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let { operType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                if (this.state.appInfo.applicationType === "privApp") {
                    values.type = 'confidential';
                } else {
                    values.clientSecret = '';
                    values.type = 'public';
                }
                let method = 'POST';
                if (operType != 'add') {
                    method = 'PUT';
                    values = Object.assign({}, this.props.activeApp, values);
                } else {
                    //重复验证(无分页)
                    if (this.props.appList && this.props.appList.find(app => app.clientId === values.clientId)) {
                        notification.error({
                            description: '应用编号已经存在!'
                        });
                        return;
                    }
                }
                this.props.dispatch(appDataSave({ method: method, data: values }));
            }
        });

    }
    handlePrivilegeTypeChange = (appTypeValue) => {
        console.log("selected", appTypeValue);
        let appInfo = this.state.appInfo || {};
        appInfo.applicationType = appTypeValue;
        this.setState({ appInfo: appInfo });
    }
    handleCancel = (e) => {
        this.props.dispatch(appDialogClose());
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <div>
                <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                    onOk={this.handleOk} onCancel={this.handleCancel} >
                    <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        应用编号
                                    </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('clientId', {
                                    initialValue: this.state.appInfo.clientId,
                                    rules: [{ required: true, message: '请输入应用编号!' }],
                                })(
                                    <Input style={{ float: 'left' }} />
                                    )}
                            </FormItem>
                        </Col>
                        <Col span={12}> <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    应用名称
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('displayName', {
                                initialValue: this.state.appInfo.displayName,
                                rules: [{ required: true, message: '请输入应用名称!' }],
                            })(
                                <Input style={{ float: 'left' }} />
                                )}
                        </FormItem></Col>
                    </Row>
                    <Row>
                        <Col span={12} >
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        应用类型
                                    </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('applicationType', {
                                    initialValue: this.state.appInfo.applicationType,
                                })(
                                    <Select style={{ width: 120 }} onChange={this.handlePrivilegeTypeChange}>
                                        {
                                            ApplicationTypes.map((app) => <Option key={app.key} value={app.key}>{app.value}</Option>)
                                        }
                                    </Select>
                                    )}
                            </FormItem>
                        </Col>
                        <Col span={12}>
                            {this.state.appInfo.applicationType === 'privApp' ? <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        应用密钥
                                    </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('clientSecret', {
                                    initialValue: this.state.appInfo.clientSecret,
                                    rules: [{ required: true, message: '请输入应用密钥!' }],
                                })(
                                    <Input />
                                    )}
                            </FormItem> : null}
                        </Col>
                    </Row>
                    <Row >
                        <Col span={20} pull={2}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        跳转地址
                                    </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('redirectUris', {
                                    initialValue: this.state.appInfo.redirectUris,
                                    rules: [{ pattern: '(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]', message: '不是有效的Url地址!' }],
                                })(
                                    <Input style={{ float: 'left' }} />
                                    )}
                            </FormItem></Col>

                    </Row>
                    <Row>
                        <Col span={20} pull={2}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        退出地址
                                    </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('postLogoutRedirectUris', {
                                    initialValue: this.state.appInfo.postLogoutRedirectUris,
                                    rules: [{ pattern: '(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]', message: '不是有效的Url地址!' }],
                                })(
                                    <Input style={{ float: 'left' }} />
                                    )}
                            </FormItem></Col>
                    </Row>
                </Modal>
            </div>
        );
    }
}
function mapStateToProps(state) {
    return {
        activeApp: state.app.activeApp,
        operInfo: state.app.operInfo,
        appList: state.app.appList
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(AppEditor);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedRegistrationForm);