import React, {Component} from 'react'
import {Row, Col, Select, DatePicker, Form, Input, Icon, InputNumber} from 'antd';
import moment from 'moment'
const FormItem = Form.Item;
const Option = Select.Option;
const styles = {
    subHeader: {
        padding: '5px',
        marginBottom: '10px',
        backgroundColor: '#e0e0e0'
    }
}
class Contract extends Component {
    state = {

    }

    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'humanContractInfo')
        }
    }


    render() {
        const {getFieldDecorator} = this.props.form;
        const formItemLayout = {
            labelCol: {span: 6},
            wrapperCol: {span: 17},
        };
        let humanContractInfo = this.props.entityInfo || {};
        let disabled = (this.props.isReadOnly || false);
        return (
            <div>
                <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />合同信息</h3>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="合同编号" >
                            {getFieldDecorator('contractNo', {
                                initialValue: humanContractInfo.contractNo,
                                rules: [{
                                    required: true, message: '请输入合同编号',
                                }]
                            })(
                                <Input disabled={disabled} placeholder="请输入合同编号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="签订单位" >
                            {getFieldDecorator('contractCompany', {
                                initialValue: humanContractInfo.contractCompany,
                                rules: [{
                                    required: true, message: '请输入签订单位',
                                }]
                            })(
                                <Input disabled={disabled} placeholder="请输入签订单位" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="合同类型">
                            {getFieldDecorator('contractType', {
                                initialValue: humanContractInfo.contractType,
                            })(
                                <Select disabled={disabled} onChange={this.handleSelectChange} placeholder="选择职位">
                                    {
                                        (this.props.dicContractCategories || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                    }
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="合同签订日期">
                            {getFieldDecorator('contractSignDate', {
                                initialValue: humanContractInfo.contractSignDate ? moment(humanContractInfo.contractSignDate) : '',
                                rules: [{
                                    required: true,
                                    message: '请选择合同签订日期'
                                }]
                            })(
                                <DatePicker disabled={disabled} format='YYYY-MM-DD' disabledDate={current=>current && current > moment().endOf('day')} style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="合同有效期">
                            {getFieldDecorator('contractStartDate', {
                                initialValue: humanContractInfo.contractStartDate ? moment(humanContractInfo.contractStartDate) : '',
                                rules: [{
                                    required: true,
                                    message: '请选择合同有效期'
                                }]
                            })(
                                <DatePicker disabled={disabled} format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="合同到期日">
                            {getFieldDecorator('contractEndDate', {
                                initialValue: humanContractInfo.contractEndDate ? moment(humanContractInfo.contractEndDate) : '',
                                rules: [{
                                    required: true,
                                    message: '请选择合同到期日'
                                }]
                            })(
                                <DatePicker disabled={disabled} format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
            </div>
        )
    }
}

const WrappedRegistrationForm = Form.create()(Contract);
export default WrappedRegistrationForm;