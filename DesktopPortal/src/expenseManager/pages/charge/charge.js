import { connect } from 'react-redux';
import React, { Component } from 'react'
import {Table, Layout, Input, Select, Icon, Button, Row, Col, Checkbox, Tag, Pagination, Spin} from 'antd'
import { NewGuid } from '../../../../utils/appUtils';
import { getDepartment } from '../../actions/actionCreator'

const { Header, Sider, Content } = Layout;
const CheckboxGroup = Checkbox.Group;
const ButtonGroup = Button.Group;


class ChargeInfo extends Component {

    state = {
        id: NewGuid(),
        costlist: []
    }

    componentWillMount() {
        this.props.dispatch(getDepartment());
        //this.props.dispatch(searchConditionType(SearchCondition.topteninfo));
    }

    handleSubmit() {

    }

    render() {
        const uploadButton = (
            <div>
              <Icon type='plus' />
              <div className="ant-upload-text">Upload</div>
            </div>
          );

        return (
            <Form className="onchargeContent" onSubmit={this.handleSubmit}>
                <FormItem {...formItemLayout}/>
                <FormItem {...formItemLayout} label="编号" >
                    {getFieldDecorator('id', {
                        reules: [{
                            required:true, message: 'please entry IDcard',
                        }, {
                            validator: this.isCardID
                        }]
                    })(
                        <Input enable="false" />
                    )}
                </FormItem>
                <FormItem {...formItemLayout1} label="报销门店">
                    {getFieldDecorator('name', {
                        reules: [{
                            required:true, message: 'please entry Name',
                        }]
                    })(
                        <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                    )}
                </FormItem>
                {
                    this.state.costlist.map(
                        function(v, i) {
                            return (
                                <div>
                                    <FormItem {...formItemLayout1} label="性别">
                                        {getFieldDecorator('sex', {
                                            reules: [{
                                                required:true, message: 'please entry Age',
                                            }]
                                        })(
                                            <Select
                                                placeholder="选择性别">
                                                <Option value="1">男</Option>
                                                <Option value="2">女</Option>
                                            </Select>
                                        )}
                                    </FormItem>
                                    <FormItem {...formItemLayout1} label="年龄">
                                        {getFieldDecorator('age', {
                                            reules: [{
                                                required:true, message: 'please entry Age',
                                            }]
                                        })(
                                            <InputNumber style={{width: '100%'}} />
                                        )}
                                    </FormItem>
                                    <FormItem {...formItemLayout1} label="生日">
                                        {getFieldDecorator('birthday', {
                                            reules: [{
                                                required:true, message: 'please entry Birthday',
                                            }]
                                        })(
                                            <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                                        )}
                                    </FormItem>
                                </div>
                            )}
                    )
                }
                <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                    <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                    <Col span={6}><Button type="primary" onClick={this.handleReset}>清空</Button></Col>
                </FormItem>
            </Form>
        );
    }
}

function chargetableMapStateToProps(state) {
    return {
        department: state.eachDepartment
    }
}

function chargetableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(chargetableMapStateToProps, chargetableMapDispatchToProps)(Form.create()(ChargeInfo));