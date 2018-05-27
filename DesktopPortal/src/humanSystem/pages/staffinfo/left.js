import { connect } from 'react-redux';
import { createStation, getOrgList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, Spin} from 'antd'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};


class Left extends Component {
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
                    <FormItem {...formItemLayout1} label="起止时间">
                        {getFieldDecorator('station', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} colon={false} label=" ">
                        {getFieldDecorator('org', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Checkbox >是否办理手续</Checkbox>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="离职办理时间">
                        {getFieldDecorator('station', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} colon={false} label=" ">
                        {getFieldDecorator('station', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Checkbox >社保是否减少</Checkbox>
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
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Left));