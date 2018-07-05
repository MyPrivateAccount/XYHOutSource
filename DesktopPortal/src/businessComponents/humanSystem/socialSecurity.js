import React, { Component } from 'react'
import { Row, Col, Checkbox, DatePicker, Form, Input, Icon, InputNumber } from 'antd';
import moment from 'moment'
const FormItem = Form.Item;
const styles = {
    subHeader: {
        padding: '5px',
        marginBottom: '10px',
        backgroundColor: '#e0e0e0'
    }
}
class SocialSecurity extends Component {
    state = {

    }

    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'socialSecurity')
        }
    }


    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 17 },
        };
        let socialSecurityInfo = this.props.entityInfo || {};
        let disabled = (this.props.isReadOnly || false);
        return (
            <div>
                <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />社保信息</h3>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否参保" >
                            {getFieldDecorator('bankName', {
                                initialValue: socialSecurityInfo.bankName,
                                rules: [{
                                    required: true, message: '请选择是否参保',
                                }]
                            })(
                                <Checkbox disabled={disabled}>已参保</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="参保时间">
                            {getFieldDecorator('insuredTime', {
                                initialValue: socialSecurityInfo.insuredTime ? moment(socialSecurityInfo.insuredTime) : null,
                                rules: [{
                                    required: true,
                                    message: '请选择参保时间'
                                }]
                            })(
                                <DatePicker disabled={disabled} format='YYYY-MM-DD' style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="参保地点" >
                            {getFieldDecorator('insuredAddress', {
                                initialValue: socialSecurityInfo.insuredAddress,
                                rules: []
                            })(
                                <Input disabled={disabled} placeholder="请输入参保地点" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否放弃购买" >
                            {getFieldDecorator('isGiveUp', {
                                initialValue: socialSecurityInfo.isGiveUp,
                                rules: []
                            })(
                                <Checkbox disabled={disabled}>是</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否签订承诺书" >
                            {getFieldDecorator('isSignCommitment', {
                                initialValue: socialSecurityInfo.isSignCommitment,
                                rules: []
                            })(
                                <Checkbox disabled={disabled}>是</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}></Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="医保账号" >
                            {getFieldDecorator('medicalInsuranceAccount', {
                                initialValue: socialSecurityInfo.medicalInsuranceAccount,
                                rules: []
                            })(
                                <Input disabled={disabled} placeholder="请输入医保账号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="社会保险账号" >
                            {getFieldDecorator('socialSecurityAccount', {
                                initialValue: socialSecurityInfo.socialSecurityAccount,
                                rules: []
                            })(
                                <Input disabled={disabled} placeholder="请输入社会保险账号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="住房公积金账号" >
                            {getFieldDecorator('housingProvidentFundAccount', {
                                initialValue: socialSecurityInfo.housingProvidentFundAccount,
                                rules: []
                            })(
                                <Input disabled={disabled} placeholder="请输入住房公积金账号" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="养老保险" >
                            {getFieldDecorator('endowmentInsurance', {
                                initialValue: socialSecurityInfo.endowmentInsurance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入养老保险" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="失业保险" >
                            {getFieldDecorator('unemploymentInsurance', {
                                initialValue: socialSecurityInfo.unemploymentInsurance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入失业保险" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="医疗保险" >
                            {getFieldDecorator('medicalInsurance', {
                                initialValue: socialSecurityInfo.medicalInsurance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入医疗保险" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="工伤保险" >
                            {getFieldDecorator('employmentInjuryInsurance', {
                                initialValue: socialSecurityInfo.employmentInjuryInsurance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入工伤保险" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="生育保险" >
                            {getFieldDecorator('maternityInsurance', {
                                initialValue: socialSecurityInfo.maternityInsurance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入生育保险" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="公积金" >
                            {getFieldDecorator('housingProvidentFund', {
                                initialValue: socialSecurityInfo.housingProvidentFund,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入公积金" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
            </div>
        )
    }
}

const WrappedRegistrationForm = Form.create()(SocialSecurity);
export default WrappedRegistrationForm;