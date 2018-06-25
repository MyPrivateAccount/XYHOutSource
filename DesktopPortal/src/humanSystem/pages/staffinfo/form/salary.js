import React, {Component} from 'react'
import {Row, Col, Checkbox, InputNumber, Form, Input, Icon} from 'antd';
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
class Salary extends Component {
    state = {

    }

    componentDidMount() {

    }

    handleOk = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = NewGuid();
                console.log('薪资构成信息: ', values);
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
                <h3 style={styles.subHeader}><Icon type="tags-o" className='content-icon' />薪资构成</h3>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="基本工资">
                            {getFieldDecorator('baseWages', {
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="岗位工资" >
                            {getFieldDecorator('postWages', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入岗位工资" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="交通补贴" >
                            {getFieldDecorator('trafficAllowance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入交通补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="通讯补贴" >
                            {getFieldDecorator('communicationAllowance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入通讯补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="其他补贴" >
                            {getFieldDecorator('otherAllowance', {
                                rules: []
                            })(
                                <InputNumber disabled={this.props.ismodify == 1} placeholder="请输入其他补贴" style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="应发工资" >
                            {getFieldDecorator('bankName', {
                                rules: []
                            })(
                                <InputNumber disabled style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="试用期工资" >
                            {getFieldDecorator('bankName', {
                                rules: []
                            })(
                                <InputNumber disabled style={{width: '100%'}} />
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