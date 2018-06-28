//组织参数配置对话框页面
import {connect} from 'react-redux';
import {orgParamDlgClose,orgParamSave,getDicParList} from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Modal, Row, Col, Form, Input,Select} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;

class OrgParamEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        paramInfo:{
            items:[]
        },
        isEdit:false
    }
    componentWillMount(){

    }
    componentWillReceiveProps(newProps){
        let {operType} = newProps.operInfo;
        if (operType === 'edit') {
            newProps.operInfo.operType = ''
            this.setState({visible: true, isEdit:true, dialogTitle: '修改组织参数',paramInfo:newProps.activeOrgParam});
        } else if (operType === 'add') {
            this.clear()
            newProps.operInfo.operType = ''
            let paramInfo = { ...this.state.paramInfo }
            paramInfo.branchId = newProps.activeOrgParam.branchId
            paramInfo.branchName = newProps.activeOrgParam.branchName
            paramInfo.items=[]
            this.setState({visible: true,isEdit:false, dialogTitle: '添加组织参数',paramInfo:paramInfo});
            this.props.dispatch(getDicParList(['YJ_BRANCH_PAR']));
            
        } 
        else if (newProps.basicOper.operType === 'DIC_GET_PARLIST_COMPLETE') {
            let paramInfo = { ...this.state.paramInfo }
            paramInfo.items = newProps.basicData.branchTypes
            this.setState({paramInfo})
            newProps.basicOper.operType = ''
        }
        else if(operType === 'ORG_PARAM_DIALOG_CLOSE') {
            this.setState({visible: false});
            newProps.operInfo.operType = ''
        }
    }
    clear=()=>{
        let paramInfo = {...this.state.paramInfo}
        paramInfo.parCode = ''
        paramInfo.parValue = ''
        this.setState({paramInfo})
    }
    handleOk = (e) => {
        e.preventDefault();

        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                //调用保存接口，进行数据保存,待续
                values.branchId = this.state.paramInfo.branchId
                this.props.dispatch(orgParamSave(values));
            }
        });
    };
    handleCancel = (e) => {//关闭对话框
        this.props.dispatch(orgParamDlgClose());
    };
    handleSelect=(e,type)=>{

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
                            label={(<span>参数名称</span>)}>
                            {getFieldDecorator('parCode', {
                                initialValue: this.state.paramInfo.parCode,
                                rules: [{required: true, message: '请填写参数名称!' }]
                            })(
                               !this.state.isEdit? <Select style={{ width: 200 }} disabled={this.state.isEdit} onChange={(e) => this.handleSelect(e, 'parcode')}>
                                        {
                                            this.state.paramInfo.items.map(tp => <Select.Option key={tp.key} value={tp.key}>{tp.key}</Select.Option>)
                                        }
                                    </Select>:
                                    <Input style={{float: 'left',width:300}} disabled={true}></Input>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>参数值</span>)}>
                            {getFieldDecorator('parValue', {
                                initialValue: this.state.paramInfo.parValue,
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
        activeOrgParam:state.orgparam.activeOrgParam,
        basicData: state.base,
        basicOper: state.base.operInfo,
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(OrgParamEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);