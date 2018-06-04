import { connect } from 'react-redux';
import { getDicParList, postBlackLst } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Select, Form, Button, Row, Col, Checkbox, Pagination, Spin} from 'antd'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};


class Black extends Component {

    componentWillMount() {
    }

    componentDidMount() {
        let len = this.props.selBlacklist.length;
        if (this.props.ismodify == 1) {//修改界面
            this.props.form.setFieldsValue({idcard: this.props.selBlacklist[len-1].idCard});
            this.props.form.setFieldsValue({name: this.props.selBlacklist[len-1].name});
            this.props.form.setFieldsValue({reason: this.props.selBlacklist[len-1].reason});
        }
    }

    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }

    handleSubmit = (e) => {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                this.props.dispatch(postBlackLst(values));
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
                    <FormItem {...formItemLayout1} label="身份证号码">
                        {getFieldDecorator('idcard', {
                            reules: [{
                                required:true, message: 'please entry idcard',
                            }]
                        })(
                            <Input disabled={this.props.ismodify==1} placeholder="请输入身份证号码" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="姓名">
                        {getFieldDecorator('name', {
                            reules: [{
                                required:true, message: 'please entry name',
                            }]
                        })(
                            <Input placeholder="请输入姓名" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="备注">
                        {getFieldDecorator('reason', {
                            reules: [{
                                required:true, message: 'please entry name',
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
        selBlacklist: state.basicData.selBlacklist,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Black));