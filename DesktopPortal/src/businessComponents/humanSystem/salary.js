import React, { Component } from 'react'
import { Row, Col, Checkbox, InputNumber, Form, Input, Icon } from 'antd';
import moment from 'moment'
import { NewGuid } from '../../utils/appUtils';
const FormItem = Form.Item;
const styles = {
    subHeader: {
        padding: '5px',
        marginBottom: '10px',
        backgroundColor: '#e0e0e0'
    }
}
class Salary extends Component {
    state = {

    }

    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'salary')
        }
    }

    onSalaryChange = (e) => {
        console.log("工资变更");
    }

    render() {
        const { getFieldDecorator } = this.props.form;
        const formItemLayout = {
            labelCol: { span: 6 },
            wrapperCol: { span: 17 },
        };
        let salaryInfo = this.props.entityInfo || {};
        let disabled = (this.props.isReadOnly || false);
        return (
            <div>
                <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />薪资构成</h3>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="基本工资">
                            {getFieldDecorator('baseWages', {
                                initialValue: salaryInfo.baseWages,
                            })(
                                <InputNumber disabled={disabled} style={{ width: '100%' }} onChange={this.onSalaryChange} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="岗位工资" >
                            {getFieldDecorator('postWages', {
                                initialValue: salaryInfo.postWages,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入岗位工资" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="交通补贴" >
                            {getFieldDecorator('trafficAllowance', {
                                initialValue: salaryInfo.trafficAllowance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入交通补贴" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="通讯补贴" >
                            {getFieldDecorator('communicationAllowance', {
                                initialValue: salaryInfo.communicationAllowance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入通讯补贴" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="其他补贴" >
                            {getFieldDecorator('otherAllowance', {
                                initialValue: salaryInfo.otherAllowance,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} placeholder="请输入其他补贴" style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="应发工资" >
                            {getFieldDecorator('grossPay', {
                                initialValue: salaryInfo.grossPay,
                                rules: []
                            })(
                                <InputNumber disabled style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="试用期工资" >
                            {getFieldDecorator('probationaryPay', {
                                initialValue: salaryInfo.probationaryPay,
                                rules: []
                            })(
                                <InputNumber disabled={disabled} style={{ width: '100%' }} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
            </div>
        )
    }
}

const WrappedRegistrationForm = Form.create()(Salary);
export default WrappedRegistrationForm;