import { connect } from 'react-redux';
import React, { Component } from 'react'
import {notification, Layout, Form, Modal, DatePicker, Input, Icon, Button, Col, Cascader} from 'antd'
import { NewGuid } from '../../../utils/appUtils';
import { paymentCharge , clearCharge} from '../../actions/actionCreator';
const FormItem = Form.Item;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};

class Payment extends Component {

    state = {
        department: ''
    }
    
    componentDidMount() {
    }

    componentWillUnmount() {
        this.props.dispatch(clearCharge());
    }


    handleSubmit = (e)=> {
        e.preventDefault();
        let self = this;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                self.props.dispatch(paymentCharge({department: values.department, chargeid: self.props.chargeList[0].id}));
            }
        });
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue } = this.props.form;
        return (
            <Form onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout1} label="付款日期">
                    {getFieldDecorator('time', {
                        reules: [{
                            required:true, message: 'please entry Age',
                        }]
                    })(
                        <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="付款单位">
                    {getFieldDecorator('department', {
                        reules: [{
                            required:true, message: 'please entry department',
                        }]
                    })(
                        <Input placeholder="请输入付款单位" />
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
        chargeList: state.basicData.selchargeList,
        //setContractOrgTree: state.basicData.departmentTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Payment));