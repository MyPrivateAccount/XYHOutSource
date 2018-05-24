import { connect } from 'react-redux';
import { createStation, getOrgList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Select, Form, Cascader, Button, Row, Col, Checkbox, Pagination, Spin} from 'antd'

const FormItem = Form.Item;
const ButtonGroup = Button.Group;
const formItemLayout1 = {
    labelCol:{ span:6},
    wrapperCol:{ span:6 },
};


class Station extends Component {
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

    handleChooseDepartmentChange = (e) => {
        this.state.department = e;
    }

    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        return (
            <div>
                <Form onSubmit={this.handleSubmit}>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1}/>
                    <FormItem {...formItemLayout1} label="职位名称">
                        {getFieldDecorator('station', {
                            reules: [{
                                required:true, message: 'please entry idcard',
                            }]
                        })(
                            <Input placeholder="请输入身份证号码" />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} label="所属组织">
                        {getFieldDecorator('org', {
                            reules: [{
                                required:true, message: 'please entry name',
                            }]
                        })(
                            <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                        )}
                    </FormItem>
                    <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                        <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >新建</Button></Col>
                    </FormItem>
                </Form>
            </div>
            
        );
    }
}

function tableMapStateToProps(state) {
    return {
        setContractOrgTree: state.basicData.searchOrgTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Station));