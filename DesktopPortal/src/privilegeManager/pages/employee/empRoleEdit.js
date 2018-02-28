import { connect } from 'react-redux';
import { appAdd, appEdit } from '../../../actions/actionCreators';
import React, { Component } from 'react';
import { Layout, Table, Button, Icon, Modal, Row, Col, Checkbox } from 'antd';
import { empDialogClose, empRoleSave, roleGetList } from '../../actions/actionCreator';
import './empRoleStyle.less'

const CheckboxGroup = Checkbox.Group;

class EmployeeRoleEditor extends Component {
    state = {
        dialogTitle: '员工权限',
        visible: false,
        roleOptions: [],
        checkedRoles: []//选中
    }
    componentWillMount = () => {
        this.props.dispatch(roleGetList());//加载角色列表
    }
    componentWillReceiveProps(newProps) {
        let { empRoleOperType } = newProps.operInfo;
        let roleOptions = [];
        console.log(newProps.activeEmp);
        newProps.roleSource.map((role) => {
            roleOptions.push({ label: role.name, value: role.name });
        });
        console.log("当前用户：", newProps.activeEmp.roles);
        this.setState({ roleOptions: roleOptions, checkedRoles: newProps.activeEmp.roles });
        if (empRoleOperType == 'edit') {
            this.setState({ visible: true, dialogTitle: '员工权限' });
        } else {
            this.setState({ visible: false });
        }
    }
    handleOk = (e) => {
        let addRoles = this.state.checkedRoles, removeRoles = [];
        if (this.props.activeEmp.roles) {
            this.props.activeEmp.roles.map((oldRole) => {
                if (this.state.checkedRoles.find((newRole) => newRole == oldRole) == undefined) {
                    removeRoles.push(oldRole);
                }
            });
        }
        console.log("addroles,removeroles,", addRoles, removeRoles);
        this.props.dispatch(empRoleSave({ addRoles: addRoles, removeRoles: removeRoles, userName: this.props.activeEmp.userName }));
    }
    handleCancel = (e) => {
        this.props.dispatch(empDialogClose());
    }

    handleRoelChange = (e) => {
        console.log("角色选择", e);
        this.setState({ checkedRoles: e });
    }

    render() {
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={(e) => this.handleOk(e)} onCancel={this.handleCancel} >
                <div style={{ color: 'rgba(0, 0, 0, 0.85)' }}>
                    <Row style={{ margin: '2px' }}>
                        <Col span={3}>用户名：</Col>
                        <Col span={21}><b>{this.props.activeEmp.trueName}</b></Col>
                    </Row>
                    <Row style={{ margin: '2px' }}>
                        <Col span={3} >所属角色：</Col>
                        <Col span={21}> <CheckboxGroup options={this.state.roleOptions} value={this.state.checkedRoles} onChange={this.handleRoelChange} /></Col>
                    </Row>
                </div>
            </Modal>
        )
    }
}

function empDialogMapStateToProps(state) {
    //console.log("empRoleManagerMapStateToProps:====", state);
    return {
        activeEmp: state.emp.activeEmp,
        operInfo: state.emp.operInfo,
        roleSource: state.role.roleSource
    }
}
function empDialogMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(empDialogMapStateToProps, empDialogMapDispatchToProps)(EmployeeRoleEditor);