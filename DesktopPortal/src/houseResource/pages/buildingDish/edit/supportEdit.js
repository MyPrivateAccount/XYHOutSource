import { connect } from 'react-redux';
import { saveSupportInfo, viewSupportInfo ,supportLoadingStart} from '../../../actions/actionCreator';
import React, { Component } from 'react'
import { Form, Button, Checkbox, Input, Row, Col, Icon, Collapse } from 'antd'

const Panel = Collapse.Panel;
const FormItem = Form.Item;

class SupportEdit extends Component {
    state = {
        buildInfo: {},
        supportInfo: {},
        loadingState: false
    }
    componentDidMount() {
        this.setState({ supportInfo: this.props.buildInfo.supportInfo, buildInfo: this.props.buildInfo });
    }
    componentWillReceiveProps(newProps) {
        this.setState({ loadingState: false });
        // console.log("componentWillReceiveProps:", newProps);
        if (this.state.buildInfo.id !== newProps.buildInfo.id) {
            this.setState({ supportInfo: newProps.buildInfo.supportInfo, buildInfo: this.props.buildInfo });
        }
    }
    handleChkChange = (e, prop) => {
        console.log("checkbox change:", e, prop);
        let supportInfo = { ...this.state.supportInfo };
        supportInfo[prop] = e.target.checked;
        this.setState({ supportInfo: supportInfo });
    }
    handleCancel = (e) => {
        this.props.dispatch(viewSupportInfo());
    }
    handleSave = (e) => {

        e.preventDefault();
        let { supportOperType } = this.props.operInfo;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                // this.setState({ loadingState: true });
                this.props.dispatch(supportLoadingStart())
                let newSupportInfo = values;
                newSupportInfo.id = this.props.buildInfo.id;
                if (supportOperType != 'add') {
                    newSupportInfo = Object.assign({}, this.props.supportInfo, newSupportInfo);
                }
                let method = (supportOperType == 'add' ? 'POST' : "PUT");
                console.log(`method:${method},newSupportInfo:${JSON.stringify(newSupportInfo)}`);
                this.props.dispatch(saveSupportInfo({ 
                    method: method, 
                    entity: newSupportInfo, 
                    ownCity: this.props.user.City  
                }));
            }
        });
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldError, isFieldTouched } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 0 },
            wrapperCol: { span: 24 },
        };
        const supportInfo = this.state.supportInfo;//this.props.buildInfo.supportInfo;
        let { supportOperType } = this.props.operInfo;
        return (

            <Form layout="horizontal" style={{ padding: '25px 0', marginTop: "25px" }} >
                <Icon type="tags-o" className='content-icon' /> <span className='content-title'>配套信息 (必填)</span>
                <Row type="flex" style={{ marginTop: "25px" }}>
                    <Col span={1}></Col>
                    <Col span={12}>
                        <h3>交通情况</h3>
                    </Col>
                </Row>
                <Row type="flex">
                    <Col span={1}></Col>
                    <Col span={22}>
                        <FormItem {...formItemLayout} label="公交" >
                            {getFieldDecorator('hasBus', {
                                initialValue: supportInfo.hasBus
                            })(
                                <label><Checkbox checked={supportInfo.hasBus} onChange={(e) => this.handleChkChange(e, 'hasBus')} /> 公交</label>
                                )}
                        </FormItem>
                        {this.state.supportInfo.hasBus === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('busDesc', { initialValue: supportInfo.busDesc })(
                                <Input type="textarea" placeholder="请完善公交信息" />
                            )}
                        </FormItem> : null}


                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasRail', { initialValue: supportInfo.hasRail })(
                                <label><Checkbox checked={supportInfo.hasRail} onChange={(e) => this.handleChkChange(e, 'hasRail')} /> 轨道交通</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasRail === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('railDesc', { initialValue: supportInfo.railDesc })(
                                <Input type="textarea" placeholder="请完善轨道交通" />
                            )}
                        </FormItem> : null}


                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasOtherTraffic', { initialValue: supportInfo.hasOtherTraffic })(
                                <label><Checkbox checked={supportInfo.hasOtherTraffic} onChange={(e) => this.handleChkChange(e, 'hasOtherTraffic')} /> 其他交通方式</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasOtherTraffic === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('otherTrafficDesc', { initialValue: supportInfo.otherTrafficDesc })(
                                <Input type="textarea" placeholder="填写其他交通方式" />
                            )}
                        </FormItem> : null}

                    </Col>
                    <Col span={1}></Col>
                </Row>
                <Row type="flex" style={{ padding: '1rem 0' }}>
                    <Col span={1}></Col>
                    <Col span={12}>
                        <h3>项目配套</h3>
                    </Col>
                </Row>
                <Row>
                    <Col span={1}></Col>
                    <Col span={22}>
                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasKindergarten', { initialValue: supportInfo.hasKindergarten })(
                                <label><Checkbox checked={supportInfo.hasKindergarten} checked={supportInfo.hasKindergarten} onChange={(e) => this.handleChkChange(e, 'hasKindergarten')} /> 幼儿园</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasKindergarten === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('kindergartenDesc', { initialValue: supportInfo.kindergartenDesc })(
                                <Input type="textarea" placeholder="幼儿园" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasPrimarySchool', { initialValue: supportInfo.hasPrimarySchool })(
                                <label><Checkbox checked={supportInfo.hasPrimarySchool} checked={supportInfo.hasPrimarySchool} onChange={(e) => this.handleChkChange(e, 'hasPrimarySchool')} /> 小学</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasPrimarySchool === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('primarySchoolDesc', { initialValue: supportInfo.primarySchoolDesc })(
                                <Input type="textarea" placeholder="小学" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasMiddleSchool', { initialValue: supportInfo.hasMiddleSchool })(
                                <label><Checkbox checked={supportInfo.hasMiddleSchool} onChange={(e) => this.handleChkChange(e, 'hasMiddleSchool')} /> 中学</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasMiddleSchool === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('middleSchoolDesc', { initialValue: supportInfo.middleSchoolDesc })(
                                <Input type="textarea" placeholder="中学" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasUniversity', { initialValue: supportInfo.hasUniversity })(
                                <label><Checkbox checked={supportInfo.hasUniversity} onChange={(e) => this.handleChkChange(e, 'hasUniversity')} /> 大学</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasUniversity === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('universityDesc', { initialValue: supportInfo.universityDesc })(
                                <Input type="textarea" placeholder="大学" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasMarket', { initialValue: supportInfo.hasMarket })(
                                <label><Checkbox checked={supportInfo.hasMarket} onChange={(e) => this.handleChkChange(e, 'hasMarket')} /> 商场</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasMarket === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('marketDesc', { initialValue: supportInfo.marketDesc })(
                                <Input type="textarea" placeholder="商场" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasSupermarket', { initialValue: supportInfo.hasSupermarket })(
                                <label><Checkbox checked={supportInfo.hasSupermarket} onChange={(e) => this.handleChkChange(e, 'hasSupermarket')} /> 超市</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasSupermarket === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('supermarketDesc', { initialValue: supportInfo.supermarketDesc })(
                                <Input type="textarea" placeholder="超市" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasHospital', { initialValue: supportInfo.hasHospital })(
                                <label><Checkbox checked={supportInfo.hasHospital} onChange={(e) => this.handleChkChange(e, 'hasHospital')} /> 医院</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasHospital === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('hospitalDesc', { initialValue: supportInfo.hospitalDesc })(
                                <Input type="textarea" placeholder="医院" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasBank', { initialValue: supportInfo.hasBank })(
                                <label><Checkbox checked={supportInfo.hasBank} onChange={(e) => this.handleChkChange(e, 'hasBank')} /> 银行</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasBank === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('bankDesc', { initialValue: supportInfo.bankDesc })(
                                <Input type="textarea" placeholder="银行" />
                            )}
                        </FormItem> : null}

                        <FormItem {...formItemLayout} >
                            {getFieldDecorator('hasOther', { initialValue: supportInfo.hasOther })(
                                <label><Checkbox checked={supportInfo.hasOther} onChange={(e) => this.handleChkChange(e, 'hasOther')} /> 其他配套设施</label>
                            )}
                        </FormItem>
                        {this.state.supportInfo.hasOther === true ? <FormItem {...formItemLayout} >
                            {getFieldDecorator('otherDesc', { initialValue: supportInfo.otherDesc })(
                                <Input type="textarea" placeholder="其他配套设施" />
                            )}
                        </FormItem> : null}
                    </Col>
                    <Col span={1}></Col>
                </Row>
                <Row>
                    <Col span={24} style={{ textAlign: 'center' }} className='BtnTop'>
                        <Button 
                            type="primary" 
                            htmlType="submit" 
                            disabled={this.props.buildInfo.isDisabled} 
                            loading={this.props.loadingState} 
                            style={{ width: "8rem" }} 
                            onClick={this.handleSave}>
                        保存</Button>
                        {supportOperType !== "add" ? <Button className="login-form-button" className='formBtn' onClick={this.handleCancel}>取消</Button> : null}
                    </Col>
                </Row>
            </Form>
        )
    }
}

function mapStateToProps(state) {
    return {
        buildInfo: state.building.buildInfo,
        operInfo: state.building.operInfo,
        loadingState: state.building.supportloading,
        user: (state.oidc.user || {}).profile || {},
    }
}

function mapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
const WrappedForm = Form.create()(SupportEdit);
export default connect(mapStateToProps, mapDispatchToProps)(WrappedForm);