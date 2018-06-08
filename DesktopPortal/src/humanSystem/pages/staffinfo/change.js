import { connect } from 'react-redux';
import { createStation, postChangeHuman, getDicParList, getcreateStation } from '../../actions/actionCreator';
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

    componentDidMount() {
        let len = this.props.selHumanList.length;
        if (this.props.ismodify == 1) {//修改界面
            if (len > 0) {

                let lstvalue = [];
                this.findCascaderLst(this.props.selHumanList[len-1].departmentId, this.props.setDepartmentOrgTree, lstvalue);

                this.state.id = this.props.selHumanList[len-1].id;
                this.props.form.setFieldsValue({name: this.props.selHumanList[len-1].name});
                this.props.form.setFieldsValue({idcard: this.props.selHumanList[len-1].idcard});
                this.props.form.setFieldsValue({orgDepartmentId: lstvalue});

                this.props.form.setFieldsValue({baseSalary: this.props.selHumanList[len-1].baseSalary});
                this.props.form.setFieldsValue({subsidy: this.props.selHumanList[len-1].subsidy});
                this.props.form.setFieldsValue({clothesBack: this.props.selHumanList[len-1].clothesBack});
                this.props.form.setFieldsValue({administrativeBack: this.props.selHumanList[len-1].administrativeBack});
                this.props.form.setFieldsValue({portBack: this.props.selHumanList[len-1].portBack});
            }
        }
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.props.dispatch(postChangeHuman(values));
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

    findCascaderLst(id, tree, lst) {
        if (tree) {
            if (tree.children&&tree.children.length === 0&&tree.id === id) {
                lst.unshift(tree.id);
                return true;
            } else {
                if (tree.children.findIndex(org => this.getChildrenID(org)) !== -1) {
                    lst.unshift(tree.id);
                    return true;
                }
            }
        }
        return false;
    }

    render() {
        let self = this;
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="姓名">
                        {getFieldDecorator('name', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input disabled={true} placeholder="请输入姓名" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="身份证号">
                        {getFieldDecorator('idcard', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input disabled={true} placeholder="请输入身份证号" />
                        )}
                    </FormItem>
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
                        {getFieldDecorator('orgDepartmentId', {
                                    reules: [{
                                        required:true,
                                        message: 'please entry',
                                    }]
                                })(
                                    <Cascader disabled={true} style={{ width: '70%' }} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect  placeholder="归属部门"/>
                                )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="原职位">
                        {getFieldDecorator('orgStation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Cascader style={{ width: '70%' }} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect  placeholder="归属部门"/>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="新职位">
                        {getFieldDecorator('newStation', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select disabled={this.props.ismodify == 1} style={{ width: '70%' }} onChange={this.handleSelectChange} placeholder="选择职位">
                                {
                                    (self.props.stationList && self.props.stationList.length > 0) ?
                                        self.props.stationList.map(
                                            function (params) {
                                                return <Option key={params.key} value={params.id}>{params.stationname}</Option>;
                                            }
                                        ):null
                                }
                            </Select>
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
        stationList: state.search.stationList,
        selSalaryItem: state.basicData.selSalaryItem,
        changeResonList: state.basicData.changeResonList,
        changeTypeList: state.basicData.changeTypeList,
        selHumanList: state.basicData.selHumanList,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Change));