import {connect} from 'react-redux';
import {createStation, getOrgList, leavePosition} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, TreeSelect} from 'antd'
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
        let humanInfo = this.props.location.state;
        this.props.form.validateFields((err, values) => {
            if (!err) {
                values.id = humanInfo.id;
                values.idCard = humanInfo.idcard;
                this.props.dispatch(leavePosition(values));
            }
        });
    }

    render() {
        const {getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched} = this.props.form;
        let humanInfo = this.props.location.state;
        console.log("部门id:",humanInfo.departmentId);
        return (
            <Layer>
                <div className="page-title" style={{marginBottom: '10px'}}>离职</div>
                <Form onSubmit={this.handleSubmit}>
                    <Row style={{marginTop: '10px'}}>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="员工编号">
                                {getFieldDecorator('userID', {
                                    initialValue: humanInfo.userID,
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
                                    initialValue: humanInfo.name,
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
                                {getFieldDecorator('departmentId', {
                                    initialValue: humanInfo.departmentId,
                                    reules: [{
                                        required: true,
                                        message: 'please entry',
                                    }]
                                })(
                                    // <Cascader disabled={true} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                                    <TreeSelect disabled={true} treeData={this.props.setDepartmentOrgTree} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="离职日期">
                                {getFieldDecorator('leaveTime', {
                                    reules: [{
                                        required: true, message: '请输入离职日期',
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
        setDepartmentOrgTree: state.basicData.searchOrgTree,

    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Left));