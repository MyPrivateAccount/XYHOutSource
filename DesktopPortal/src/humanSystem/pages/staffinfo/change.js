import { connect } from 'react-redux';
import { createStation, getOrgList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {InputNumber, Input, Form, Select, Button, Row, Col, Checkbox, DatePicker} from 'antd'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};


class Change extends Component {
    state = {
        department: ''
    }

    componentWillMount() {
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                //this.props.dispatch(postBlackLst(values));
            }
        });
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="异动类型">
                        {getFieldDecorator('changetype', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select placeholder="选择异动类型">
                                {/* {
                                    (self.props.chargeCostTypeList && self.props.chargeCostTypeList.length > 0) ?
                                        self.props.chargeCostTypeList.map(
                                            function (params) {
                                                return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                            }
                                        ):null
                                } */}
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="异动原因">
                        {getFieldDecorator('changereason', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select placeholder="选择异动原因">
                                {/* {
                                    (self.props.chargeCostTypeList && self.props.chargeCostTypeList.length > 0) ?
                                        self.props.chargeCostTypeList.map(
                                            function (params) {
                                                return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                            }
                                        ):null
                                } */}
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="其他原因简述">
                        {getFieldDecorator('otherreason', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input.TextArea rows={4} placeholder="请输入原因" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="原职位">
                        {getFieldDecorator('orgstation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input disabled={true} placeholder="请输入职位" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="原部门">
                        {getFieldDecorator('orgdepartment', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input disabled={true} placeholder="请输入部门" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新职位">
                        {getFieldDecorator('newstation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input placeholder="请输入新职位" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新部门">
                        {getFieldDecorator('newdepartment', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input placeholder="请输入原因" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新工资信息">
                        
                    </FormItem>
                    <FormItem {...formItemLayout1} label="基本工资">
                        {getFieldDecorator('baseSalary', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber placeholder="请输入基本工资" style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="岗位补贴">
                        {getFieldDecorator('subsidy', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber placeholder="请输入岗位补贴" style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="工装扣款">
                        {getFieldDecorator('clothesBack', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber placeholder="请输入工装扣款" style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="行政扣款">
                        {getFieldDecorator('administrativeBack', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber placeholder="请输入行政扣款" style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="端口扣款">
                        {getFieldDecorator('portBack', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber placeholder="请输入端口扣款" style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                        <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                    </FormItem>
                </Form>
            </div>
            
        );
    }
}

function tableMapStateToProps(state) {
    return {
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Change));