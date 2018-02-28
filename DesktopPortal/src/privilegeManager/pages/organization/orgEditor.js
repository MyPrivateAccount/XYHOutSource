import {connect} from 'react-redux';
import {appAdd, appEdit, orgNodeSave, orgDialogClose} from '../../actions/actionCreator';
import {getDicParList} from '../../../actions/actionCreators'
import React, {Component} from 'react'
import {globalAction} from 'redux-subspace';
import {Select, Button, Checkbox, Modal, Row, Col, Form, Input, message} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class OrgEditor extends React.Component {
    state = {
        snackbarOpend: false,
        orgInfo: {},
        orgType: 'Normal'
    }
    componentWillMount() {
        this.props.dispatch(globalAction(getDicParList(["ORGNAZATION_TYPE"])));
    }
    componentWillReceiveProps(newProps) {
        let {operType, result} = newProps.operInfo;
        let {activeTreeNode, treeSource} = newProps;
        //this.setState({ orgInfo: activeTreeNode });

        if (operType == 'edit') {
            this.setState({visible: true, dialogTitle: '部门编辑', orgInfo: activeTreeNode});
        } else if (operType == 'add') {
            this.setState({visible: true, dialogTitle: '部门新增', orgInfo: {}});
        } else {
            this.props.form.resetFields();
            this.setState({visible: false});
        }
    }
    handleOk = (e) => {
        e.preventDefault();
        let {operType} = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                if (!values.type) {
                    values.type = 'public';
                }
                let method = (operType == 'add' ? 'POST' : "PUT");
                let newOrg = {};
                if (operType == 'add') {
                    newOrg = values;
                    newOrg.parentId = this.props.activeTreeNode.id || "0";
                } else {
                    newOrg = Object.assign({}, this.props.activeTreeNode, values);
                }
                this.props.dispatch(orgNodeSave({orgInfo: newOrg, method: method}));
            }
        });

    };

    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(orgDialogClose());
    };

    handleOrgTypeChange = (orgType) => {
        this.setState({orgType: orgType});
    }

    render() {
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 14},
        };
        let orgnazitionType = (this.props.basicDic || {}).orgnazitionType || [];

        return (
            // <Modal title="部门详细" >
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    部门名称
                                    </span>
                            )}>
                            {getFieldDecorator('organizationName', {
                                initialValue: this.state.orgInfo.organizationName,
                                rules: [{required: true, message: '请输入部门名称!'}],
                            })(
                                <Input style={{float: 'left'}} />
                                )}
                        </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    部门主管
                                    </span>
                            )}>
                            {getFieldDecorator('leaderManager', {
                                initialValue: this.state.orgInfo.leaderManager,
                            })(
                                <Input style={{float: 'left'}} />
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    部门电话
                                    </span>
                            )}
                        >
                            {getFieldDecorator('phone', {
                                initialValue: this.state.orgInfo.phone,
                            })(
                                <Input style={{float: 'left'}} />
                                )}
                        </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    传真
                                    </span>
                            )}>
                            {getFieldDecorator('fax', {
                                initialValue: this.state.orgInfo.fax
                            })(
                                <Input style={{float: 'left'}} />
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>部门类型</span>)}>
                            {getFieldDecorator('type', {
                                initialValue: this.state.orgInfo.type || "Normal"
                            })(
                                <Select style={{width: '100%'}} onChange={this.handleOrgTypeChange}>
                                    {orgnazitionType.map(org => <Option key={org.key} value={org.value}>{org.key}</Option>)}
                                </Select>
                                )}
                        </FormItem></Col>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>所在城市</span>)}>
                            {getFieldDecorator('city', {
                                initialValue: this.state.orgInfo.city,
                                rules: [{required: this.state.orgType === "Filiale", message: '请选择所在城市!'}],
                            })(
                                <Select style={{width: '100%'}}>
                                    {
                                        this.props.areaList.map((city, i) =>
                                            <Option key={i} value={city.value}>{city.label}</Option>
                                        )
                                    }
                                </Select>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={3}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>部门地址</span>)}>
                            {getFieldDecorator('address', {
                                initialValue: this.state.orgInfo.address,
                            })(
                                <Input style={{float: 'left'}} />
                                )}
                        </FormItem></Col>
                </Row>
            </Modal>
        )
    }
}
function orgEditMapStateToProps(state) {
    //console.log('apptableMapStateToProps:' + JSON.stringify(state));
    return {
        activeTreeNode: state.org.activeTreeNode,
        treeSource: state.org.treeSource,
        operInfo: state.org.operInfo,
        areaList: state.org.areaList,
        basicDic: state.rootBasicData
    };
}

function dialogMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(OrgEditor);
export default connect(orgEditMapStateToProps, dialogMapDispatchToProps)(WrappedRegistrationForm);