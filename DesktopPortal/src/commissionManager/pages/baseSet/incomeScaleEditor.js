//提成比例配置对话框页面
import {connect} from 'react-redux';
import { incomeScaleSave, incomeScaleDlgClose } from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Modal, Row, Col, Form, Input,Select,notification} from 'antd'
import NumericInput from './numberInput'

const FormItem = Form.Item;
const Option = Select.Option;

class InComeScaleEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        paramInfo:{},
        isEdit:false,
    }
    componentWillMount(){

    }
    componentWillReceiveProps(newProps){
        let {operType} = newProps.operInfo;
        if (operType === 'edit') {
            this.setState({visible: true, isEdit:true, dialogTitle: '修改',paramInfo:newProps.activeScale});
            newProps.operInfo.operType = ''
        } else if (operType === 'add') {
            this.clear()
            this.setState({visible: true,isEdit:false, dialogTitle: '添加'});
            this.setState({paramInfo:{branchName:newProps.param.branchName,codeName:newProps.param.codeName,branchId:newProps.param.branchId,code:newProps.param.code}})
            newProps.operInfo.operType = ''
        } 
        else if(operType === 'INCOME_SCALE_DLGCLOSE') {
            this.setState({visible: false});
            newProps.operInfo.operType = ''
        }
    }
    clear=()=>{
        let paramInfo = {...this.state.paramInfo}
        paramInfo.startYj = ''
        paramInfo.endYj = ''
        paramInfo.percent = ''
        this.setState({paramInfo})
    }
    handleOk = (e) => {
        e.preventDefault();

        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                if(parseFloat(values.startYj)>parseFloat(values.endYj)){
                    notification.error({
                        message: '错误',
                        description: '开始业绩必须小于结束业绩',
                        duration: 3
                    });
                    return
                }
                //调用保存接口，进行数据保存,待续
                if(this.state.isEdit){
                    values.id = this.state.paramInfo.id
                    values.mod = true
                }
                values.branchId = this.state.paramInfo.branchId;
                values.code = this.state.paramInfo.code;
                this.props.dispatch(incomeScaleSave(values));
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(incomeScaleDlgClose());
    };
    getPercent = (e) => {
        let pp = e
        pp = pp.substr(0, pp.length - 1)
        pp = parseFloat(pp) / 100
        return pp
    }
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
                            {getFieldDecorator('branchName', {

                                initialValue: this.state.paramInfo.branchName,
                            })(
                                <Input style={{float: 'left',width:300}} disabled={true}></Input>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>职位类别</span>)}>
                            {getFieldDecorator('codeName', {
                                initialValue: this.state.paramInfo.codeName
                            })(
                                <Input style={{float: 'left',width:300}} disabled={true}></Input>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>起始业绩</span>)}>
                            {getFieldDecorator('startYj', {
                                initialValue: this.state.paramInfo.startYj,
                                rules: [{required: true, message: '请填写起始业绩!' }]
                            })(
                                <Input type="number" step="0.01" style={{float: 'left',width:300}}/>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>结束业绩</span>)}>
                            {getFieldDecorator('endYj', {
                                initialValue: this.state.paramInfo.endYj,
                                rules: [{required: true, message: '请填写结束业绩!' }]
                            })(
                                <Input type="number" step="0.01" style={{float: 'left',width:300}}/>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>提成比例</span>)}>
                            {getFieldDecorator('percent', {
                                initialValue: this.state.isEdit?this.getPercent(this.state.paramInfo.percent):'',
                                rules: [{required: true, message: '请填写提成比例!' }]
                            })(
                                <NumericInput style={{float: 'left',width:300}}/>
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
        activeScale:state.scale.activeScale,
        param:state.scale.param
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(InComeScaleEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);