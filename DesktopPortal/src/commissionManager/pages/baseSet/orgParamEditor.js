//组织参数配置对话框页面
import {connect} from 'react-redux';
import {orgParamDlgClose,orgParamSave} from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Modal, Row, Col, Form, Input,Select} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class OrgParamEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        paramInfo:{}
    }
    componentWillMount(){

    }
    componentWillReceiveProps(newProps){
        let {operType} = newProps.operInfo;
        if (operType === 'edit') {
            this.setState({visible: true, dialogTitle: '修改组织参数',paramInfo:newProps.activeOrgParam});
        } else if (operType === 'add') {
            this.setState({visible: true, dialogTitle: '添加组织参数'});
        } else {
            this.props.form.resetFields();
            this.setState({visible: false});
        }
    }
    handleOk = (e) => {
        e.preventDefault();

        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                //调用保存接口，进行数据保存,待续
                this.props.dispatch(orgParamSave());
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(orgParamDlgClose());
    };
    render(){
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 14},
        };
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <Row>
                <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    组织
                                </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('orgName', {

                                initialValue: this.state.paramInfo.orgName,
                            })(
                                <Input style={{float: 'left',width:300}}></Input>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>参数名称</span>)}>
                            {getFieldDecorator('paramName', {
                                initialValue: this.state.paramInfo.paramName
                            })(
                                <Select defaultValue="lucy" style={{ width: 120 }}>
                                    <Option value="jack">Jack</Option>
                                    <Option value="lucy">Lucy</Option>
                                    <Option value="disabled" disabled>Disabled</Option>
                                    <Option value="Yiminghe">yiminghe</Option>
                                </Select>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>参数值</span>)}>
                            {getFieldDecorator('paramVal', {
                                initialValue: this.state.paramInfo.paramVal,
                                rules: [{required: true, message: '请填写参数值!' }]
                            })(
                                <Input style={{float: 'left',width:300}}/>
                                )}
                        </FormItem></Col>
                </Row>
            </Modal>
        )
    }
}

function MapStateToProps(state) {

    return {
        operInfo:state.orgparam.operInfo,
        activeOrgParam:state.orgparam.activeOrgParam
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(OrgParamEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);