import { connect } from 'react-redux';
import React, { Component } from 'react'
import { Select, Button, Modal, Row, Col, Form, Input, Menu, Dropdown, message, TreeSelect } from 'antd'
import { roleEditComplete, roleSave, roleDialogClose, getOrgDataByID } from '../../actions/actionCreator';

const FormItem = Form.Item;
const TreeNode = TreeSelect.TreeNode;
const Option = Select.Option;

class RoleEditor extends Component {
    state = {
        dialogTitle: '',
        visible: false,
        roleInfo: {},
        roleType: 'Normal'
    }
    componentWillReceiveProps(newProps) {
        let { operType, result } = newProps.operInfo;
        let { activeRole } = newProps;
        if (operType == 'edit') {
            this.setState({ visible: true, dialogTitle: '角色编辑', roleInfo: activeRole });
        } else if (operType == 'add') {
            this.setState({ visible: true, dialogTitle: '角色新增', roleInfo: {} });
        } else {
            this.props.form.resetFields();
            this.setState({ visible: false });
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let { operType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                let method = 'POST';
                if (operType != 'add') {
                    method = "PUT";
                    values = Object.assign({}, this.props.activeRole, values);
                }
                this.props.dispatch(roleSave({ method: method, data: values }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(roleDialogClose());
    }
    //角色部门选择
    handleOrgSelected = (e) => {
        console.log("", );
    }

    handleRoleTypeChange = (roleType) => {
        this.setState({ roleType: 'Normal' })
    }

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
        const orgTreeSource = (this.state.roleType === "Normal" ? this.props.permissionOrgTree.AddNormalRoleTree : this.props.permissionOrgTree.AddPublicRoleTree);

        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    角色名称
                            </span>
                            )}>
                            {getFieldDecorator('name', {
                                initialValue: this.state.roleInfo.name,
                                rules: [{ required: true, message: '请输入角色名称!' }],
                            })(
                                <Input style={{ float: 'left' }} />
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
                                    角色类型
                            </span>
                            )}>
                            {getFieldDecorator('type', {
                                initialValue: this.state.roleInfo.type || "Normal",
                                rules: [{ required: true, message: '请选择角色类型!' }],
                            })(
                                <Select style={{ width: "100%" }} onChange={this.handleRoleTypeChange}>
                                    <Option value="Normal">普通</Option>
                                    <Option value="Public">公共</Option>
                                </Select>
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
                                    所属部门
                            </span>
                            )}>
                            {getFieldDecorator('organizationId', {

                                initialValue: this.state.roleInfo.organizationId,
                                rules: [{ required: true, message: '请选择所属部门!' }]
                            })(
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.orgTreeSource}
                                    placeholder="请选择所属部门">
                                    {treeNodes}
                                </TreeSelect>
                                )}
                        </FormItem>
                    </Col>
                </Row>
            </Modal>
        )
    }
}
function roleEditMapStateToProps(state) {
    //console.log("roleEditMapStateToProps:", state.role);
    return {
        activeRole: state.role.activeRole,
        operInfo: state.role.operInfo,
        roleSource: state.role.roleSource,
        treeSource: state.org.treeSource,
        permissionOrgTree: state.org.permissionOrgTree
    }
}

function roleEditMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(RoleEditor);
export default connect(roleEditMapStateToProps, roleEditMapDispatchToProps)(WrappedRegistrationForm);