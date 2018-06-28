import {connect} from 'react-redux';
import {createStation, getOrgList, leavePosition} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, Spin} from 'antd'
import Layer from '../../../components/Layer'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
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
                values.id = this.props.selHumanList[this.props.selHumanList.length - 1].id;
                values.idCard = this.props.selHumanList[this.props.selHumanList.length - 1].idcard;
                this.props.dispatch(leavePosition(values));
            }
        });
    }

    render() {
        const {getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched} = this.props.form;
        return (
            <Layer>
                <div className="page-title" style={{marginBottom: '10px'}}>离职</div>
                <Form onSubmit={this.handleSubmit}>
                    <Row style={{marginTop: '10px'}}>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="员工编号">
                                {getFieldDecorator('id', {
                                    reules: [{
                                        required: true, message: '请输入员工编号',
                                    }]
                                })(
                                    <Input disabled={true} placeholder="请输入员工编号" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="姓名">
                                {getFieldDecorator('name', {
                                    reules: [{
                                        required: true, message: '请输入姓名',
                                    }]
                                })(
                                    <Input disabled={true} placeholder="请输入姓名" />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="部门">
                                {getFieldDecorator('orgDepartmentId', {
                                    reules: [{
                                        required: true,
                                        message: 'please entry',
                                    }]
                                })(
                                    <Cascader disabled={true} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="离职日期">
                                {getFieldDecorator('leaveTime', {
                                    reules: [{
                                        required: true, message: 'please entry',
                                    }]
                                })(
                                    <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="交接人">
                                {getFieldDecorator('handover', {
                                    rules: [{
                                        required: true, message: '请选择交接人',
                                    }]
                                })(
                                    <Select disabled={this.props.ismodify == 1} placeholder="请选择交接人">

                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                        <Col span={7}>
                            <FormItem {...formItemLayout} colon={false} label=" ">
                                {getFieldDecorator('isFormalities', {
                                    reules: [{
                                        required: true, message: 'please entry',
                                    }]
                                })(
                                    <Checkbox >是否办理手续</Checkbox>
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={20} style={{textAlign: 'center'}}>
                            <Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button>
                        </Col>
                    </Row>
                    {/* <FormItem {...formItemLayout1} label="离职办理时间">
                        {getFieldDecorator('leaveTime', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <DatePicker format='YYYY-MM-DD' style={{width: '100%'}} />
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} colon={false} label=" ">
                        {getFieldDecorator('isFormalities', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Checkbox >是否办理手续</Checkbox>
                        )}
                    </FormItem>
                    <FormItem {...formItemLayout1} colon={false} label=" ">
                        {getFieldDecorator('isReduceSocialEnsure', {
                            reules: [{
                                required:true, message: 'please entry',
                            }]
                        })(
                            <Checkbox >社保是否减少</Checkbox>
                        )}
                    </FormItem>
                    <FormItem wrapperCol={{ span: 12, offset: 6 }}>
                        <Col span={6}><Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button></Col>
                    </FormItem> */}
                </Form>
            </Layer >

        );
    }
}

function tableMapStateToProps(state) {
    return {
        selHumanList: state.basicData.selHumanList,
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Left));