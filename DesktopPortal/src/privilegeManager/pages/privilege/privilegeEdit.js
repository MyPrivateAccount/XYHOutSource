import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Layout, Button, Modal, Row, Col, Form, Input, Menu, Select, message } from 'antd'
import { privilegeAdd, privilegeEdit, privilegeDialogClose, privilegeSave } from '../../actions/actionCreator';

const FormItem = Form.Item;
const Option = Select.Option;

class PrivilegeEditor extends Component {
    state = {
        dialogTitle: '',
        visible: false,
        selectAppId: '',
        privilegeInfo: {}
    }
    componentWillReceiveProps(newProps) {
        let { activePrivilege, selectAppId } = newProps;
        let { operType } = newProps.operInfo;
        if (operType == 'edit') {
            if (activePrivilege) {
                for (let i in this.props.appList) {
                    let appInfo = this.props.appList[i];
                    if (appInfo.id == activePrivilege.applicationId) {
                        this.setState({ selectAppId: appInfo.id });
                    }
                }
            }
            this.setState({ visible: true, dialogTitle: '权限编辑', privilegeInfo: activePrivilege });
        } else if (operType == 'add') {
            let newAppId = ''
            if (selectAppId != '') {
                newAppId = selectAppId;
            }
            this.setState({ visible: true, dialogTitle: '权限新增', privilegeInfo: { applicationId: newAppId }, selectAppId: newAppId });
        } else {
            this.props.form.resetFields();
            this.setState({ visible: false });
        }
    }
    handleAppTypeChange = (e) => {
        console.log('menu click', e);
        this.setState({ selectAppId: e });
    }
    handleOk = (e) => {
        e.preventDefault();
        let { operType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                //values.applicationId = this.state.selectAppId;
                console.log('Received values of form: ', values);
                let method = (operType == 'add' ? 'POST' : "PUT");
                this.props.dispatch(privilegeSave({ method: method, data: values, searchAppid: this.state.selectAppId }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(privilegeDialogClose());
    }
    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                    <Col span={12}> <FormItem
                        {...formItemLayout}
                        label={(
                            <span>
                                权限编号
                            </span>
                        )}
                        hasFeedback>
                        {getFieldDecorator('id', {
                            initialValue: this.state.privilegeInfo.id,
                            rules: [{ required: true, message: '请输入权限编号!' }],
                        })(
                            <Input style={{ float: 'left' }} />
                            )}
                    </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    权限名称
                            </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('name', {
                                initialValue: this.state.privilegeInfo.name,
                                rules: [{ required: true, message: '请输入权限名称!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>

                </Row>
                <Row >
                    <Col span={12}> <FormItem
                        {...formItemLayout}
                        label={(
                            <span>
                                分组名称
                            </span>
                        )}
                        hasFeedback>
                        {getFieldDecorator('groups', {
                            initialValue: this.state.privilegeInfo.groups,
                            rules: [{ required: true, message: '请输入分组名称!' }],
                        })(
                            <Input style={{ float: 'left' }} />
                            )}
                    </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    所属应用
                            </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('applicationId', {
                                initialValue: this.state.privilegeInfo.applicationId,
                            })(
                                <Select style={{ width: 120 }} onChange={this.handleAppTypeChange}>
                                    {
                                        this.props.appList.map((app, i) => <Option key={app.id} value={app.id}>{app.displayName}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>

                </Row>
            </Modal>
        )
    }
}

function mapStateToProps(state) {
    //console.log("privilegeEditMapStateToProps:" + JSON.stringify(state));
    return {
        appList: state.app.appList,
        operInfo: state.privilege.operInfo,
        activePrivilege: state.privilege.activePrivilege,
        selectAppId: state.privilege.selectAppId
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(PrivilegeEditor);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedRegistrationForm);