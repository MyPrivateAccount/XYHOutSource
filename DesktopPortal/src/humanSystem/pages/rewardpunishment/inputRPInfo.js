import { connect } from 'react-redux';
import { getDicParList, postBlackLst } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Select, Form, Button, Row, Col, InputNumber, DatePicker, Cascader} from 'antd'

const {MonthPicker} = DatePicker;
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

//ADMINISTRATIVE_REWARD  ADMINISTRATIVE_PUNISHMENT  ADMINISTRATIVE_DEDUCT
class Black extends Component {
    state = {
        type: 0,
        department:"",
    }

    componentWillMount() {
        this.props.dispatch(getDicParList(["ADMINISTRATIVE_REWARD, ADMINISTRATIVE_PUNISHMENT, ADMINISTRATIVE_DEDUCT"]));
    }

    componentDidMount() {
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            values.userID;
            if (!err) {
                this.props.dispatch(postBlackLst(values));
            }
        });
    }

    onHandleChange = (e) => {
        this.setState({type: e});
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e[e.length - 1];
    }

    handleSelectChange = (e) => {
        //this.props.dispatch(getSalaryItem(e));
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;

        let detailtype = null;
        switch (this.state.type) {
            case 1:detailtype = this.props.administrativereward;break;
            case 2:detailtype = this.props.administrativepunishment;break;
            case 3:detailtype = this.props.administrativededuct;break;
            default:detailtype = [];break;
        }

        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="行政类型">
                        {getFieldDecorator('type', {
                            reules: [{
                                required:true, message: 'please entry idCard',
                            }]
                        })(
                            <Select placeholder="选择类型" onChange={this.onHandleChange}>
                                <Option key='1' value="1">行政奖励</Option>
                                <Option key='2' value="2">行政惩罚</Option>
                                <Option key='3' value="3">行政扣款</Option>
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="详细类型">
                        {getFieldDecorator('detail', {
                            reules: [{
                                required:true, message: 'please entry name',
                            }]
                        })(
                            <Select placeholder="选择类型">
                            {
                                detailtype.map(function(v, i) {
                                    return (
                                        <Option key={i} value={v.value}>{v.key}</Option>
                                    );
                                })
                            }
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="部门">
                        {getFieldDecorator('departmentID', {
                            reules: [{
                                required:true, message: 'please entry name',
                            }]
                        })(
                            <Cascader options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="员工">
                        {getFieldDecorator('name', {
                            reules: [{
                                required:true, message: 'please entry name',
                            }]
                        })(
                            <Select placeholder="选择员工">
                            {
                                this.props.rewardpunishhumanlst.map(function(v, i) {
                                    return (
                                        <Option key={i} value={v.value}>{v.key}</Option>
                                    );
                                })
                            }
                            </Select>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="有效日期">
                        {getFieldDecorator('workDate', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <MonthPicker format='YYYY-MM-DD' style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="金额">
                        {getFieldDecorator('money', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <InputNumber style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="备注">
                        {getFieldDecorator('comments', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Input placeholder="请输入备注" />
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
        rewardpunishhumanlst: state.basicData.rewardpunishhumanlst,
        setDepartmentOrgTree: state.basicData.searchOrgTree,
        administrativereward: state.basicData.administrativereward,
        administrativepunishment: state.basicData.administrativepunishment,
        administrativededuct: state.basicData.administrativededuct,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Black));