import { connect } from 'react-redux';
import { appAdd, appEdit } from '../../../actions/actionCreators';
import React, { Component } from 'react';
import { Layout, Table, Button, Icon, Modal, Row, Col, Form, Input, message, Select, TreeSelect } from 'antd';
import { empDialogClose, empSave, roleGetList, empGetPrivList, getOrgDataByID } from '../../actions/actionCreator';

const FormItem = Form.Item;
const Option = Select.Option;
const TreeNode = TreeSelect.TreeNode;


class EmployeeEditor extends Component {
    state = {
        empInfo: {},
        dialogTitle: '员工信息',
        visible: false,
        empOrgId: ''
    }
    componentWillMount = () => {
        this.props.dispatch(roleGetList());//加载角色列表
        this.props.dispatch(empGetPrivList());
    }
    componentWillReceiveProps(newProps) {
        let { operType, result } = newProps.operInfo;
        if (operType == 'edit') {
            this.setState({ empInfo: newProps.activeEmp, visible: true, dialogTitle: '员工编辑' });
        } else if (operType == 'add') {
            this.setState({ empInfo: { organizationId: newProps.activeTreeNode.id, password: '123456' }, visible: true, dialogTitle: '员工新增' });
        } else {
            this.setState({ visible: false });
            this.props.form.resetFields();
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let { operType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                let newEmpInfo = values;
                if (operType != 'add') {
                    newEmpInfo = Object.assign({}, this.props.activeEmp, values);
                    newEmpInfo.roles = values.roles;
                }
                newEmpInfo.filialeId = values.organizationId;
                let method = (operType == 'add' ? 'POST' : "PUT");
                console.log(`method:${method},empInfo:${JSON.stringify(newEmpInfo)}`);
                this.props.dispatch(empSave({ method: method, empInfo: newEmpInfo, activeEmp: this.props.activeEmp }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(empDialogClose());
    }

    // loadTreeSelectData = (e) => {//加载
    //     let { eventKey } = e.props;
    //     console.log("loadTreeselectData", );
    //     this.props.dispatch(getOrgDataByID({ id: eventKey }));
    //     return new Promise((resolve) => {
    //         setTimeout(() => {
    //             resolve();
    //         }, 500);
    //     });
    // }

    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const loop = data => data.map((item) => {
            if (item.children) {
                return <TreeNode title={item.name} key={item.key} value={item.key}>{loop(item.children)}</TreeNode>;
            }
            return <TreeNode title={item.name} key={item.key} value={item.key} isLeaf={item.isLeaf} />;
        });
        const treeNodes = loop(this.props.treeSource);
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    员工编号
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('userName', {
                                initialValue: this.state.empInfo.userName,
                                rules: [{ required: true, message: '请输入员工编号!' }],
                            })(
                                <Input />
                                )}
                        </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    员工姓名
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('trueName', {
                                initialValue: this.state.empInfo.trueName,
                                rules: [{ required: true, message: '请输入员工姓名!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    联系电话
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('phoneNumber', {
                                initialValue: this.state.empInfo.phoneNumber,
                                rules: [{ pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!' }, { required: true, message: '请输入联系电话!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    Email
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('email', {
                                initialValue: this.state.empInfo.email,
                                rules: [{ type: 'email', message: '输入邮件地址不正确!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>职级</span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('position', {
                                initialValue: this.state.empInfo.position,
                            })(
                                <Select style={{ width: 200 }} placeholder='请选择职级'>
                                    {
                                        this.props.privList.map((priv, i) => <Option key={priv.value}>{priv.key}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        {
                            this.props.operInfo.operType == 'add' ? <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>初始密码</span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('password', {
                                    initialValue: this.state.empInfo.password,
                                    rules: [{ min: 6, message: '请输入至少6位密码!' }, { required: true, message: '请输入初始密码!' }],
                                })(
                                    <Input type='password' />
                                    )}
                            </FormItem> : null
                        }

                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    所属部门
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('organizationId', {

                                initialValue: this.state.empInfo.organizationId,
                                rules: [{ required: true, message: '请选择所属部门!' }]
                            })(
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="请选择所属部门">
                                    {treeNodes}
                                </TreeSelect>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                {/* <Row>
                    <Col span={24} pull={3}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>所属角色</span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('roles', {
                                initialValue: this.state.empInfo.roles,
                                //rules: [{ required: true, message: '请选择用户所属角色!' }],
                            })(
                                <Select mode="multiple" placeholder="请选择所属角色" style={{ width: '99%' }}>
                                    {
                                        this.props.roleSource.map((role, i) => <Option key={role.id} value={role.name}>{role.name}</Option>)
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row> */}
            </Modal>
        )
    }
}

function empDialogMapStateToProps(state) {
    console.log("empDialogMapStateToProps:", state);
    return {
        activeEmp: state.emp.activeEmp,
        operInfo: state.emp.operInfo,
        roleSource: state.role.roleSource,
        privList: state.emp.privList,
        treeSource: state.org.treeSource,
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree
    }
}
function empDialogMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(EmployeeEditor);
export default connect(empDialogMapStateToProps, empDialogMapDispatchToProps)(WrappedRegistrationForm);