import { connect } from 'react-redux';
import { getXYHBuildingDetail, setLoading, expandSearchbox, recommendBuilding } from '../actions/actionCreator';
import React, { Component } from 'react'
import { Button, Row, Col, Tag, Form, Modal, Input, InputNumber } from 'antd'

const FormItem = Form.Item;

class AddFLowDialog extends Component {
    state = {
        dialogTitle: "新增流程",
        visible: false
    }

    handleOk = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                // let newEmpInfo = values;
                // if (operType != 'add') {
                //     newEmpInfo = Object.assign({}, this.props.activeEmp, values);
                //     newEmpInfo.roles = values.roles;
                // }
                // newEmpInfo.filialeId = values.organizationId;
                // let method = (operType == 'add' ? 'POST' : "PUT");
                // console.log(`method:${method},empInfo:${JSON.stringify(newEmpInfo)}`);
                // this.props.dispatch(empSave({ method: method, empInfo: newEmpInfo, activeEmp: this.props.activeEmp }));
            }
        });
    }
    handleCancel = (e) => {

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
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="流程ID" >
                            {getFieldDecorator('flowID', {
                                //initialValue: this.state.empInfo.userName,
                                rules: [{ required: true, message: '请输入流程ID!' }],
                            })(
                                <Input />
                                )}
                        </FormItem></Col>
                    <Col span={12}>
                        <FormItem  {...formItemLayout} label="流程名称">
                            {getFieldDecorator('flowName', {
                                //initialValue: this.state.empInfo.userName,
                                rules: [{ required: true, message: '请输入流程名称!' }],
                            })(
                                <Input />
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem  {...formItemLayout} label="重试次数">
                            {getFieldDecorator('flowName', {
                                //initialValue: this.state.empInfo.userName,
                                rules: [{ required: true, message: '请输入流程名称!' }],
                            })(
                                <InputNumber min={1} max={10} defaultValue={3} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                    </Col>
                </Row>
            </Modal>
        )
    }

}

function mapStateToProps(state) {
    return {
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(mapStateToProps, mapDispatchToProps)(AddFLowDialog);