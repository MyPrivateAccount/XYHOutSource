//业绩分摊新增分摊项
import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Row, Col, Form, Input,Tooltip,Button} from 'antd'

const FormItem = Form.Item;

class AcmentItemEditor extends Component{
    state = {
        paramInfo:{isCheck:true}
    }
    componentWillMount(){

    }
    componentWillReceiveProps(newProps){
        
    }
    handleChoose = (e)=>{
        //选择
        e.preventDefault();

        this.props.form.validateFields((err, values) => {
            if (!err) {
                console.log('Received values of form: ', values);
                //调用保存接口，进行数据保存,待续
                this.props.updateItemValue(values);
            }
        });
    }
    render(){
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 9},
            wrapperCol: {span: 11},
        };
        return (
            <div style={{display:this.props.vs?'block':'none'}}>
                <Row>
                <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(
                                <span>
                                    编码
                                </span>
                            )}
                            hasFeedback>
                            {getFieldDecorator('itemCode', {

                                initialValue: this.state.paramInfo.ftItem,
                                rules: [{required: true, message: '请填写编码!' }]
                            })(
                                <Input style={{float: 'left',width:200}}></Input>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12} push={2}>
                        <FormItem>
                        <Tooltip title="选择">
                            <Button  type='primary' shape='circle' icon={'check'} onClick={this.handleChoose}/>
                        </Tooltip>
                        <Tooltip title="返回">
                            <Button style={{'margin-left':5}} type='primary' shape='circle' icon={'rollback'} onClick={this.props.back}/>
                        </Tooltip>
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem
                            {...formItemLayout}
                            label={(<span>名称</span>)}>
                            {getFieldDecorator('itemName', {
                                initialValue: this.state.paramInfo.ftScale,
                                rules: [{required: true, message: '请填写名称!' }]
                            })(
                                <Input style={{float: 'left',width:200}}></Input>
                                )}
                        </FormItem></Col>
                </Row>
            </div>
        )
    }
}

function MapStateToProps(state) {

    return {
        operInfo:state.acm.operInfo,
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedRegistrationForm = Form.create()(AcmentItemEditor);
export default connect(MapStateToProps, MapDispatchToProps)(WrappedRegistrationForm);