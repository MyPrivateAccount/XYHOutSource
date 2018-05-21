//提成比例配置对话框页面
import {connect} from 'react-redux';
import { incomeScaleSave, incomeScaleDlgClose } from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Modal, Row, Col, Form, Input,Select} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class InComeScaleEditor extends Component{
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
            this.setState({visible: true, dialogTitle: '修改',paramInfo:newProps.activeScale});
        } else if (operType === 'add') {
            this.setState({visible: true, dialogTitle: '添加'});
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
                this.props.dispatch(incomeScaleSave());
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(incomeScaleDlgClose());
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
                            label={(<span>职位类别</span>)}>
                            {getFieldDecorator('paramRankPos', {
                                initialValue: this.state.paramInfo.rankPos
                            })(
                                <Input style={{float: 'left',width:300}}></Input>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>起始业绩</span>)}>
                            {getFieldDecorator('paramStyj', {
                                initialValue: this.state.paramInfo.paramStyj,
                                rules: [{required: true, message: '请填写起始业绩!' }]
                            })(
                                <Input style={{float: 'left',width:300}}/>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>结束业绩</span>)}>
                            {getFieldDecorator('paramEdyj', {
                                initialValue: this.state.paramInfo.paramEdyj,
                                rules: [{required: true, message: '请填写结束业绩!' }]
                            })(
                                <Input style={{float: 'left',width:300}}/>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>提成比例</span>)}>
                            {getFieldDecorator('paramScale', {
                                initialValue: this.state.paramInfo.paramEdyj,
                                rules: [{required: true, message: '请填写提成比例!' }]
                            })(
                                <Input style={{float: 'left',width:300}}/>
                                )}
                        </FormItem>
                    </Col>
                </Row>
            </Modal>
        )
    }
}

function MapStateToProps(state) {

    return {
        operInfo:state.scale.operInfo,
        activeScale:state.scale.activeScale
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(InComeScaleEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);