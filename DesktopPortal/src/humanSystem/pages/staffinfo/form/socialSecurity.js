import React, {Component} from 'react'
import {Row, Col, Checkbox, DatePicker, Form, Input, Icon, InputNumber} from 'antd';
import moment from 'moment'
import {NewGuid} from '../../../../utils/appUtils';
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

    }

    handleOk = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                console.log('社保信息: ', values);
                if (this.props.confirmCallback) {
                    this.props.confirmCallback(values);
                }
                this.handleCancel();
            }
        });
    }
    handleCancel = () => {
        if (this.props.closeDialog) {
            this.props.closeDialog();
        }
    }

    render() {
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 17},
        };
        return (
            <div>
                <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />社保信息</h3>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否参保" >
                            {getFieldDecorator('bankName', {
                                rules: [{
                                    required: true, message: '请输入职称',
                                }]
                            })(
                                <Checkbox disabled={this.props.ismodify == 1}>已参保</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="参保时间">
                            {getFieldDecorator('insuredTime', {
                                initialValue: moment(),
                                rules: [{
                                    required: true,
                                    message: '请选择参保时间'
                                }]
                            })(
                                <DatePicker disabled={this.props.ismodify == 1} format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="参保地点" >
                            {getFieldDecorator('insuredAddress', {
                                rules: []
                            })(
                                <Input disabled={this.props.ismodify == 1} placeholder="请输入参保地点" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否放弃购买" >
                            {getFieldDecorator('isGiveUp', {
                                rules: []
                            })(
                                <Checkbox disabled={this.props.ismodify == 1}>是</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="是否签订承诺书" >
                            {getFieldDecorator('isSignCommitment', {
                                rules: []
                            })(
                                <Checkbox disabled={this.props.ismodify == 1}>是</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}></Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="医保账号" >
                            {getFieldDecorator('medicalInsuranceAccount', {
                                rules: []
                            })(
                                <Input disabled={this.props.ismodify == 1} placeholder="请输入医保账号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="社会保险账号" >
                            {getFieldDecorator('socialSecurityAccount', {
                                rules: []
                            })(
                                <Input disabled={this.props.ismodify == 1} placeholder="请输入社会保险账号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="住房公积金账号" >
                            {getFieldDecorator('housingProvidentFundAccount', {
                                rules: []
                            })(
                                <Input disabled={this.props.ismodify == 1} placeholder="请输入住房公积金账号" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="养老保险" >
                            {getFieldDecorator('endowmentInsurance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入养老保险" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="失业保险" >
                            {getFieldDecorator('unemploymentInsurance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入失业保险" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="医疗保险" >
                            {getFieldDecorator('medicalInsurance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入医疗保险" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="工伤保险" >
                            {getFieldDecorator('employmentInjuryInsurance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入工伤保险" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="生育保险" >
                            {getFieldDecorator('maternityInsurance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入生育保险" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="公积金" >
                            {getFieldDecorator('housingProvidentFund', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入公积金" style={{width: '100%'}} />
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