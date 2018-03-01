import { connect } from 'react-redux';
import { saveRulesInfo, rulesView,ruleLoadingStart } from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Form, Button, Checkbox, Icon, Row, Col, Input, InputNumber, TimePicker,DatePicker  } from 'antd'
import moment from 'moment'

const FormItem = Form.Item;
const Option = Select.Option;
const CheckboxGroup = Checkbox.Group;

class RulesEdit extends Component {
    state = {
        loadingState: false,
        checked: false,
    }
    componentWillMount() {
        if (this.props.setChildForm) {
            this.props.setChildForm(this.props.form)
        }
        let ruleInfo = this.props.buildInfo.ruleInfo || {};
        this.setState({
            checked: ruleInfo.isCompletenessPhone
        })
    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
    }
    handleSave = (e) => {
        let { rulesOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.props.dispatch(ruleLoadingStart())
                let nowInfo = values
                nowInfo.liberatingStart = new Date(values.liberatingStart.format('YYYY-MM-DD HH:mm'))
                nowInfo.liberatingEnd = new Date(values.liberatingEnd.format('YYYY-MM-DD HH:mm'))
                nowInfo.id = this.props.buildInfo.id
                let ruleInfo = this.props.buildInfo.ruleInfo || {}
                nowInfo.reportedTemplate = ruleInfo.reportedTemplate
                console.log(nowInfo, 'formData')
                this.props.dispatch(saveRulesInfo({
                    rulesOperType: rulesOperType, 
                    entity: nowInfo, 
                    id: this.props.buildInfo.id,
                    template: false,
                    ownCity: this.props.user.City 
                }));
            }
        });

    }

    handleCancel = (e) => {
        let body = this.props.buildInfo.ruleInfo
        console.log(body, '???cacaca')
        this.props.dispatch(rulesView({body: body }))
        // body.liberatingStart = moment(body.liberatingStart).format('YYYY-MM-DD HH:mm')
        // body.liberatingEnd = moment(body.liberatingEnd).format('YYYY-MM-DD HH:mm')
        // console.log( body, '123')
        
    }
    checkboxChange = (e) => {
        this.setState({
            checked: !this.state.checked
        })
      console.log(e.target.checked, '勾选')
    }

    render() {
        const format = 'HH:mm';
        const { getFieldDecorator, getFieldsValue } = this.props.form;
        
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 14 },
        };
        const tips = '向开发商报备时，单条短信或微信允许发送的最大客户数量';
        let ruleInfo = this.props.buildInfo.ruleInfo || {};
        let liberatingStart, liberatingEnd;
        console.log(ruleInfo, 999)
        if (ruleInfo.liberatingStart) {
            liberatingStart = moment(ruleInfo.liberatingStart, "HH:mm")
        }
        if (ruleInfo.liberatingEnd) {
            liberatingEnd = moment(ruleInfo.liberatingEnd, "HH:mm")
        }
        let { rulesOperType } = this.props.operInfo;
        return (
            <Form layout="horizontal" style={{padding: '25px 0',marginTop: '25px',backgroundColor: "#ECECEC" }} className='rules'>
                <Icon type="tags-o" className='content-icon'/> <span className='content-title'>报备规则</span>
                <Row type="flex" style={{marginTop: "25px"}}>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>报备有效期(天)</span>} >
                            {getFieldDecorator('validityDay', {
                                initialValue: ruleInfo.validityDay || 1,
                                rules: [{ required: true, message: '请输入报备有效期' }],
                            })(
                              <InputNumber  min={1} onChange={this.getFieldsValue}/>
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>带看保护期(天)</span>} >
                            {getFieldDecorator('beltProtectDay', {
                                initialValue: ruleInfo.beltProtectDay || 30,
                                rules: [{ required: true, message: '请输入带看保护期' }],
                            })(
                              <InputNumber  min={1}/>
                                )}
                        </FormItem>
                    </Col>
                </Row>
               
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>接访时间(最早)</span>} >
                            {getFieldDecorator('liberatingStart', {
                                initialValue: liberatingStart,
                                rules: [{ required: true, message: '请输入最早接访时间' }],
                            })(
                              <TimePicker format={format} />
                                )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>接访时间(最晚)</span>} >
                            {getFieldDecorator('liberatingEnd', {
                                initialValue: liberatingEnd,
                                rules: [{ required: true, message: '请输入最晚接访时间' }],
                            })(
                              <TimePicker  format={format} />
                            )}
                    </FormItem>
                    </Col>
                </Row>
                <Row>
                  <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>报备开始时间</span>} >
                            {getFieldDecorator('reportTime', {
                                initialValue: ruleInfo.reportTime ? moment(ruleInfo.reportTime, 'YYYY-MM-DD') : null,
                                // rules: [{ required: true, message: '请输入提前开始时间' }],
                            })(
                                <DatePicker />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label={<span>提前报备时间(分钟)</span>} >
                            {getFieldDecorator('advanceTime', {
                                initialValue: ruleInfo.advanceTime,
                                // rules: [{ required: true, message: '请输入提前报备时间' }],
                            })(
                                <InputNumber  min={1}/>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} help={tips}
                        label={<span>最大客户数(个)</span>} >
                            {getFieldDecorator('maxCustomer', {
                                initialValue: ruleInfo.maxCustomer || 20,
                            })(
                              <InputNumber  min={1} max={20} placeholder='最大客户数20个'/>
                                )}
                        </FormItem>
                    </Col>
                </Row>
                <Row >
                
                    <Col span={24}  push={3}>
                        <FormItem {...formItemLayout} label='' >
                              {getFieldDecorator('isCompletenessPhone', {
                                //   initialValue: ruleInfo.isCompletenessPhone
                              })(
                                <Checkbox onChange={this.checkboxChange} checked={this.state.checked}>向开发商报备时，必须提供客户的完整手机号</Checkbox>
                              )}
                          </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={24} pull={3}>
                        <FormItem {...formItemLayout} label={<span>备注</span>} >
                            {getFieldDecorator('mark', {
                                initialValue: ruleInfo.mark 
                            })(
                                <Input type="textarea" placeholder="这里可以填写报备的注意事项"  style={{ height: '100px' }}/>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                {
                    this.props.type === 'dynamic' ? null : 
                    <Row>
                        <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                            <Button 
                            type="primary" 
                            htmlType="submit" 
                            disabled={this.props.buildInfo.isDisabled} 
                            loading={this.props.loadingState} 
                            style={{width: "8rem"}} 
                            onClick={this.handleSave}>保存</Button>
                            {rulesOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                        </Col>
                    </Row>
                }
            </Form >
        )
    }
}

function mapStateToProps(state) {
    // console.log('relshops MapStateToProps:', state.building.buildInfo);
    return {
        basicData: state.basicData,
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.rulesloading,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}

const WrappedForm = Form.create()(RulesEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);
