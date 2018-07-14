import React, {Component} from 'react'
import {Row, Col, Modal, DatePicker, Form, Input, Select} from 'antd';
import moment from 'moment'
import {NewGuid} from '../../../utils/appUtils';
const FormItem = Form.Item;
const Option = Select.Option;
class Education extends Component {
    state = {

    }

    handleOk = (e) => {
        e.preventDefault();
        let editInfo = this.props.entityInfo || {};
        this.props.form.validateFields((err, values) => {
            if (!err) {
                if (editInfo.id) {
                    values.id = editInfo.id;
                } else {
                    values.id = NewGuid();
                }
                console.log('学历信息: ', values);
                values.enrolmentTime = values.enrolmentTime ? values.enrolmentTime.format('YYYY-MM-DD') : '';
                values.graduationTime = values.graduationTime ? values.graduationTime.format("YYYY-MM-DD") : '';
                values.getDegreeTime = values.getDegreeTime ? values.getDegreeTime.format("YYYY-MM-DD") : '';
                if (this.props.confirmCallback) {
                    this.props.confirmCallback(values);
                }
                this.handleCancel();
            }
        });
    }
    handleCancel = () => {
        this.props.form.resetFields();
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
        let editInfo = this.props.entityInfo || {};
        console.log("教育经历:", editInfo);
        return (
            <Modal title="学历信息" maskClosable={false} width='700px' visible={this.props.showDialog} onOk={this.handleOk} onCancel={this.handleCancel}>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="学历">
                            {getFieldDecorator('education', {
                                initialValue: editInfo.education
                            })(
                                <Select onChange={this.handleSelectChange} placeholder="选择学历" style={{width: '100%'}}>
                                    {
                                        (this.props.dicEducation || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                    }
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="所学专业" >
                            {getFieldDecorator('major', {
                                initialValue: editInfo.major,
                                rules: [{
                                    required: true, message: '请输入所学专业',
                                }]
                            })(
                                <Input placeholder="请输入所学专业" />
                            )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="学习形式">
                            {getFieldDecorator('learningType', {
                                initialValue: editInfo.learningType
                            })(
                                <Input placeholder="请输入学习形式" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="毕业证书" >
                            {getFieldDecorator('graduationCertificate', {
                                initialValue: editInfo.graduationCertificate,
                                rules: [{
                                    required: true, message: '请输入毕业证书',
                                }]
                            })(
                                <Input placeholder="请输入毕业证书" />
                            )}
                        </FormItem>
                    </Col>
                </Row>

                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="入学时间">
                            {getFieldDecorator('enrolmentTime', {
                                initialValue: editInfo.enrolmentTime ? moment(editInfo.enrolmentTime) : null,
                                rules: [{
                                    required: true,
                                    message: '请选择入学时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' disabledDate={current=>current && current > moment().endOf('day')} style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="毕业时间">
                            {getFieldDecorator('graduationTime', {
                                initialValue: editInfo.graduationTime ? moment(editInfo.graduationTime) : null,
                                rules: [{
                                    required: true,
                                    message: '请选择毕业时间'
                                }]
                            })(
                                <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="获得学位">
                            {getFieldDecorator('getDegree', {
                                initialValue: editInfo.getDegree
                            })(
                                <Select onChange={this.handleSelectChange} placeholder="获得学位" style={{width: '100%'}}>
                                    {
                                        (this.props.dicDegree || []).map(item => <Option key={item.value} value={item.value}>{item.key}</Option>)
                                    }
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="学位授予时间">
                            {getFieldDecorator('getDegreeTime', {
                                initialValue: editInfo.getDegreeTime ? moment(editInfo.getDegreeTime) : null,
                                rules: []
                            })(
                                <DatePicker format='YYYY-MM-DD' disabledDate={current=>current && current > moment().endOf('day')} style={{width: '100%'}} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={12}>
                        <FormItem {...formItemLayout} label="学位授予单位" >
                            {getFieldDecorator('getDegreeCompany', {
                                initialValue: editInfo.getDegreeCompany,
                                rules: []
                            })(
                                <Input placeholder="请输入学位授予单位" />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <FormItem labelCol={{span: 3}} wrapperCol={{span: 16}} label="毕业学校" >
                            {getFieldDecorator('graduationSchool', {
                                initialValue: editInfo.graduationSchool,
                                rules: [{
                                    required: true, message: '请输入毕业学校',
                                }]
                            })(
                                <Input placeholder="请输入毕业学校" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}></Col>
                </Row>
            </Modal>
        )
    }
}

const WrappedRegistrationForm = Form.create()(Education);
export default WrappedRegistrationForm;