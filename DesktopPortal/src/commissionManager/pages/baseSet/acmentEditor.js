//业绩分摊比例弹出对话框
import {connect} from 'react-redux';
import { acmentParamSave, acmentParamDlgClose ,acmentParamItemAdd} from '../../actions/actionCreator'
import React, {Component} from 'react'
import {Button,Modal, Row, Col, Form, Input,Select,Checkbox,Tooltip} from 'antd'
import AcmentItemEditor from './acmentItemEditor'

const FormItem = Form.Item;
const Option = Select.Option;

class AcmentEditor extends Component{
    state = {
        dialogTitle:'',
        visible:false,
        iedVisible:false,
        paramInfo:{isCheck:true}
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
            this.setState({visible: false});
        }
    }
    handleOk = (e) => {
        
    };
    handleCancel = (e) => {//关闭对话框
        this.setState({iedVisible:false})
        this.props.dispatch(acmentParamDlgClose());
    };
    handleNewItem = (e)=>{
        this.setState({iedVisible:true});
    }
    handleItemValue = (e)=>{
        this.setState({iedVisible: false});
        this.setState({paramInfo:{ftItem:e.itemName}});
    }
    handleback = (e)=>{
        this.setState({iedVisible: false});
    }
    render(){
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 9},
            wrapperCol: {span: 11},
        };
        return (
            <Modal width={600} title={this.state.dialogTitle} maskClosable={false} visible={this.state.visible}
                onOk={this.handleOk} onCancel={this.handleCancel} >
                <div style={{display:!this.state.iedVisible?'block':'none'}}>
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

                                initialValue: this.state.paramInfo.ftItem,
                            })(
                                <Input style={{float: 'left',width:200}}></Input>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12} push={2}>
                        <Tooltip title="新增">
                            <Button type='primary' shape='circle' icon='plus' onClick={this.handleNewItem}/>
                        </Tooltip>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>默认分摊比例</span>)}>
                            {getFieldDecorator('ftScale', {
                                initialValue: this.state.paramInfo.ftScale,
                                rules: [{required: true, message: '请填写默认分摊比例!' }]
                            })(
                                <Input style={{float: 'left',width:200}}></Input>
                                )}
                        </FormItem>
                        </Col>
                </Row>
                <Row>
                    <Col span={12} push={5}>
                        <FormItem>
                        {getFieldDecorator('ftCheck', {
                                initialValue: this.state.paramInfo.isCheck,
                            })(
                                <Checkbox defaultChecked={this.state.paramInfo.isCheck}>固定比例</Checkbox>
                                )}
                        </FormItem></Col>
                </Row>
                </div>
                <AcmentItemEditor vs={this.state.iedVisible} updateItemValue={this.handleItemValue} back={this.handleback}/>
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