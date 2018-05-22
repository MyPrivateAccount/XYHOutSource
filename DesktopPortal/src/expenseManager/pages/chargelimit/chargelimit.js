import { connect } from 'react-redux';
import React, { Component } from 'react'
import {notification, Layout, Form, Modal, Input, Icon, Select, Button, Col, InputNumber} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { getLimitChargeHuman } from '../../actions/actionCreator';
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class ChargeLimit extends Component {

    state = {
        department: ''
    }
    
    componentWillMount() {
        this.props.dispatch(getLimitChargeHuman());
    }

    componentDidMount() {
        let dt = new Date();
        let show = dt.getFullYear()+"/"+dt.getMonth()+"/"+dt.getDay();
        this.props.form.setFieldsValue({time: show});
    }

    handleChooseDepartmentChange = (e) => {
        this.state.department = e;
    }

    handleSubmit = (e)=> {
        e.preventDefault();
        let self = this;
        this.props.form.validateFields((err, values) => {
            if (!err) {

            }
        });
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        let self = this;
        return (
            <Form onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout1}/>
                <FormItem {...formItemLayout1}/>
                <FormItem {...formItemLayout1} label="选择员工">
                    {getFieldDecorator('member', {
                        reules: [{
                            required:true, message: 'please entry member',
                        }]
                    })(
                        <Select placeholder="选择费用类型">
                            {
                                (self.props.limitHumanlst && self.props.limitHumanlst.length > 0) ?
                                    self.props.limitHumanlst.map(
                                        function (params) {
                                            return <Option key={params.value} value={params.value+""}>{params.key}</Option>;
                                        }
                                    ):null
                            }
                        </Select>
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="额度设置">
                    {getFieldDecorator('limit', {
                        reules: [{
                            required:true, message: 'please entry limit',
                        }]
                    })(
                        <InputNumber placeholder="请输入金额" style={{width: '100%'}} />
                    )}
                </FormItem>
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                </FormItem>
            </Form>
        );
    }
}


function tableMapStateToProps(state) {
    return {
        limitHumanlst: state.basicData.limitHumanlst,
        chargeList: state.basicData.selchargeList,
        setContractOrgTree: state.basicData.departmentTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(ChargeLimit));