//业绩分摊比例弹出对话框
import {connect} from 'react-redux';
import { acmentParamSave, acmentParamDlgClose } from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Modal, Row, Col, Form, Input,Select} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class AcmentEditor extends Component{
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
                this.props.dispatch(acmentParamSave());
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(acmentParamDlgClose());
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
                                    分摊项
                                </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('ftItem', {

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
                            label={(<span>默认分摊比例</span>)}>
                            {getFieldDecorator('ftScale', {
                                initialValue: this.state.paramInfo.rankPos
                            })(
                                <Input style={{float: 'left',width:300}}></Input>
                                )}
                        </FormItem></Col>
                </Row>
            </Modal>
        )
    }
}

function MapStateToProps(state) {

    return {
        operInfo:state.acm.operInfo,
        activeScale:state.acm.activeScale
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(AcmentEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);