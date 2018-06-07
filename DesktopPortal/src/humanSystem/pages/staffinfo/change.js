import { connect } from 'react-redux';
import { createStation, getOrgList, getDicParList, getcreateStation } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {InputNumber, Input, Form, Select, Button, Row, Col, Checkbox, DatePicker, Cascader} from 'antd'

const FormItem = Form.Item;
const Option = Select.Option;
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
        this.props.dispatch(getDicParList(["HUMAN_CHANGE_TYPE", "HUMAN_CHANGEREASON_TYPE"]));
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

    handleDepartmentChange = (e) => {
        if (!e) {
            this.props.dispatch(getcreateStation(this.state.department));
        }
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e[e.length-1];
    }

    render() {
        let self = this;
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="异动类型">
                        {getFieldDecorator('changeType', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select placeholder="选择异动类型">
                                {
                                    (self.props.changeTypeList && self.props.changeTypeList.length > 0) ?
                                        self.props.changeTypeList.map(
                                            function (params) {
                                                return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                            }
                                        ):null
                                }
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="异动原因">
                        {getFieldDecorator('changeReason', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select placeholder="选择异动原因">
                               {
                                    (self.props.changeResonList && self.props.changeResonList.length > 0) ?
                                        self.props.changeResonList.map(
                                            function (params) {
                                                return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                            }
                                        ):null
                                }
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="其他原因简述">
                        {getFieldDecorator('otherReason', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input.TextArea rows={4} placeholder="请输入原因" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="所属部门">
                        {getFieldDecorator('orgdepartmentId', {
                                    reules: [{
                                        required:true,
                                        message: 'please entry',
                                    }]
                                })(
                                    <Cascader disabled={this.props.ismodify == 1} style={{ width: '70%' }} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect  placeholder="归属部门"/>
                                )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="原职位">
                        {getFieldDecorator('orgStation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input disabled={true} placeholder="请输入职位" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新职位">
                        {getFieldDecorator('newStation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input placeholder="请输入新职位" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新部门">
                        {getFieldDecorator('newDepartmentId', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input placeholder="请输入部门" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} colon={false} label="新工资信息">
                    </FormItem>
                    <FormItem {...formItemLayout1} label="基本工资">
                        {getFieldDecorator('baseSalary', {
                            initialValue: self.props.selSalaryItem? self.props.selSalaryItem.baseSalary:null
                        })(
                                        <InputNumber disabled={this.props.ismodify == 1} style={{ width: '70%' }} />
                                    )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="岗位补贴">
                        {getFieldDecorator('subsidy', {
                            initialValue: self.props.selSalaryItem? self.props.selSalaryItem.subsidy:null
                        })(
                                            <InputNumber disabled={this.props.ismodify == 1} style={{ width: '70%' }} />
                                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="工装扣款">
                        {getFieldDecorator('clothesBack', {
                            initialValue: self.props.selSalaryItem? self.props.selSalaryItem.clothesBack:null
                        })(
                                        <InputNumber disabled={this.props.ismodify == 1} style={{width: '70%'}} />
                                    )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="行政扣款">
                        {getFieldDecorator('administrativeBack', {
                            initialValue: self.props.selSalaryItem? self.props.selSalaryItem.administrativeBack:null
                        })(
                                        <InputNumber disabled={this.props.ismodify == 1} style={{width: '70%'}} />
                                    )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="端口扣款">
                        {getFieldDecorator('portBack', {
                            initialValue: self.props.selSalaryItem? self.props.selSalaryItem.portBack:null
                        })(
                                        <InputNumber disabled={this.props.ismodify == 1} style={{width: '70%'}} />
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
        selSalaryItem: state.basicData.selSalaryItem,
        changeResonList: state.basicData.changeResonList,
        changeTypeList: state.basicData.changeTypeList,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Change));