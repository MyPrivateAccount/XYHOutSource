import { connect } from 'react-redux';
import { appAdd, appEdit } from '../../../actions/actionCreators';
import React, { Component } from 'react';
import { Layout, Table, Button, Icon, Modal, Row, Col, Form, Input, message, Select, TreeSelect } from 'antd';
import { companyACloseDialog, companyASave } from '../../actions/actionCreator';

const FormItem = Form.Item;
const Option = Select.Option;



class CompanyAEdit extends Component {
    state = {
        companyAInfo: {},
        dialogTitle: '甲方信息',
        visible: false,
        //empOrgId: ''
    }
    componentWillMount = () => {
        //this.props.dispatch(roleGetList());//加载角色列表
        //this.props.dispatch(empGetPrivList());
    }
    componentWillReceiveProps(newProps) {
        //console.log("newProps:", newProps);
        let { operType, result } = newProps.operInfo;
        if (operType == 'edit') {
          
            this.setState({ companyAInfo: newProps.activeCompanyA, visible: true, dialogTitle: '甲方编辑' });
        } else if (operType == 'add') {
            this.setState({ companyAInfo: {  }, visible: true, dialogTitle: '甲方新增' });
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
                let newInfo = values;
                if (operType != 'add') {
                    newInfo = Object.assign({}, this.props.activeCompanyA, values);
                    //newInfo.roles = values.roles;
                }
                //newInfo.filialeId = values.organizationId;
                let method = (operType == 'add' ? 'POST' : "PUT");
                console.log(`method:${method},empInfo:${JSON.stringify(newInfo)}`);
                this.props.dispatch(companyASave({ method: method, companyAInfo: newInfo, activeCompanyA: this.props.activeCompanyA }));
            }
        });
    }
    handleCancel = (e) => {
        this.props.dispatch(companyACloseDialog());
    }



    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        //console.log('this.state.companyAInfo:', this.state);
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                    {/* <Col span={12}>
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
                        </FormItem></Col> */}
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    甲方名称
                                    </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('name', {
                                initialValue: this.state.companyAInfo.name,
                                rules: [{ required: true, message: '请输入甲方名称!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                    <FormItem {...formItemLayout} label={<span>甲方类型</span>}>
                        {getFieldDecorator('type', {
                                        initialValue: this.state.companyAInfo.type,
                                        rules: [{ required: true, message: '请选择甲方类型!' }],
                                    })(
                                        <Select style={{ width: "100%" }}>
                                            {
                                                this.props.basicData.firstPartyCatogories.map((item) =>
                                                    <Option key={item.value}>{item.key}</Option>
                                                )
                                            }
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
                                        所在地址
                                        </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('address', {
                                    initialValue: this.state.companyAInfo.address,
                                    //rules: [{ required: true, message: '请输入甲方地址!' }],
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
                                        联系电话
                                        </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('phoneNum', {
                                    initialValue: this.state.companyAInfo.phoneNum,
                                    rules: [{ pattern: '^((1[0-9][0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$', message: '不是有效的手机号码!' },/* { required: true, message: '请输入联系电话!' }*/],
                                })(
                                    <Input />
                                    )}
                            </FormItem>
                        </Col>
    
                </Row>
               

            </Modal>
        )
    }
}

function dialogMapStateToProps(state) {
    return {
        basicData: state.basicData,
        activeCompanyA: state.companyAData.activeCompanyA,
        operInfo: state.companyAData.operInfo,
        
    }
}
function dialogMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(CompanyAEdit);
export default connect(dialogMapStateToProps, dialogMapDispatchToProps)(WrappedRegistrationForm);