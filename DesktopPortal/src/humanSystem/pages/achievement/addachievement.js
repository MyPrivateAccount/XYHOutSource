import { connect } from 'react-redux';
import { getDicParList, postBlackLst, setSalaryInfo, getcreateStation} from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Select, Form, Button, Row, Col, Checkbox, Cascader, InputNumber } from 'antd'
import { NewGuid } from '../../../utils/appUtils';

const FormItem = Form.Item;
const Option = Select.Option;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};


class Achievement extends Component {

    componentWillMount() {
        this.state = {
            id: NewGuid(),
            department: ""
        };
    }

    componentDidMount() {
        let len = this.props.selAchievementList.length;
        if (this.props.ismodify == 1) {//修改界面
            this.state.id = this.props.selAchievementList[len-1].id;
            this.props.form.setFieldsValue({organize: this.props.selAchievementList[len-1].organize});
            this.props.form.setFieldsValue({position: this.props.selAchievementList[len-1].position});
            this.props.form.setFieldsValue({baseSalary: this.props.selAchievementList[len-1].baseSalary});
            this.props.form.setFieldsValue({subsidy: this.props.selAchievementList[len-1].subsidy});
            this.props.form.setFieldsValue({clothesBack: this.props.selAchievementList[len-1].clothesBack});
            this.props.form.setFieldsValue({administrativeBack: this.props.selAchievementList[len-1].administrativeBack});
            this.props.form.setFieldsValue({portBack: this.props.selAchievementList[len-1].portBack});
        }
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        let self = this;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = self.state.id;
                if (values.organize instanceof Array) {
                    values.organize = values.organize[values.organize.length-1];
                }
                
                let vf = self.props.stationList[self.props.stationList.findIndex(function(v, i) {
                    return v.id == values.position;
                })];
                values.positionName = vf.stationname;
                
                self.props.dispatch(setSalaryInfo(values));
                this.props.form.resetFields();

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
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        let self = this;
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="选择组织">
                        {getFieldDecorator('organize', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Cascader disabled={this.props.ismodify == 1} options={this.props.setContractOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect  placeholder="归属部门"/>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="选择职位">
                        {getFieldDecorator('position', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Select disabled={this.props.ismodify == 1} placeholder="选择职位">
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
        selAchievementList: state.basicData.selAchievementList,
        setContractOrgTree: state.basicData.searchOrgTree,
        stationList: state.search.stationList,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Achievement));