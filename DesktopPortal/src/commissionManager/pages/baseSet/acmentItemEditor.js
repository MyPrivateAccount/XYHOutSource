//业绩分摊新增分摊项
import {connect} from 'react-redux';
import React, {Component} from 'react'
import {Row, Col, Form, Input,Tooltip,Button,Select,Checkbox} from 'antd'
import NumericInput from './numberInput'

const FormItem = Form.Item;
const Option = Select.Option;

class AcmentItemEditor extends Component{
    state = {
        paramInfo:{isCheck:true}
    }
    componentDidMount(){
        this.props.onSelf(this)
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
    getData=()=>{
        let dt = {}
        this.props.form.validateFields((err, values) => {
            console.log('getData:'+err)
            if (!err) {
                console.log('Received values of form: ', values);
                dt = values
                this.props.saveItemValue(dt)
            }
        });
    }
    getPercent=(e)=>{
        let pp = e
        pp = pp.substr(0,pp.length-1)
        pp = parseFloat(pp)/100
        return pp
    }
    isFixedChange=(e)=>{
        let paramInfo = {...this.state.paramInfo}
        paramInfo.isfixed = e.target.checked
        this.setState({paramInfo},()=>{
            console.log(this.state.paramInfo)
        })
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
                            {getFieldDecorator('code', {

                                initialValue: '',
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
                            {getFieldDecorator('name', {
                                initialValue: '',
                                rules: [{required: true, message: '请填写名称!' }]
                            })(
                                <Input style={{float: 'left',width:200}}></Input>
                                )}
                        </FormItem></Col>
                </Row>
                <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(
                                    <span>
                                        分摊类型
                                </span>
                                )}
                                hasFeedback>
                                {getFieldDecorator('type', {
                                    initialValue: this.state.paramInfo.type==='外部佣金'?"1":"2",
                                })(
                                    <Select initialValue="1" style={{ width: 200 }}>
                                        <Option value="1">外部佣金</Option>
                                        <Option value="2">内部分摊项</Option>
                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12}>
                            <FormItem
                                {...formItemLayout}
                                label={(<span>默认分摊比例</span>)}>
                                {getFieldDecorator('percent', {
                                    initialValue: '',
                                    rules: [{ required: true, message: '请填写默认分摊比例!' }]
                                })(
                                    <NumericInput  style={{ float: 'left', width: 200 }}></NumericInput>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={12} push={5}>
                            <FormItem>
                                {getFieldDecorator('isfixed', {
                                    initialValue:true
                                })(
                                    <Checkbox defaultChecked={true}>固定比例</Checkbox>
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