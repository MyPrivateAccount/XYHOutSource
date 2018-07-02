import { connect } from 'react-redux';
import { createStation, getOrgList, leavePosition } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, TreeSelect } from 'antd'
import Layer from '../../../components/Layer'
import LeaveForm from '../../../businessComponents/humanSystem/leavePreview'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 17 },
};


class Leave extends Component {
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


    //子页面回调
    subPageLoadCallback = (formObj, pageName) => {
        console.log("表单对象:", formObj, pageName);
        if (pageName == "socialSecurity") {
            this.setState({ socialSecurityForm: formObj });
        } else if (pageName == "salary") {
            this.setState({ salaryForm: formObj });
        }
    }


    render() {
        const { getFieldDecorator, getFieldsError, getFieldsValue, isFieldTouched } = this.props.form;
        let humanInfo = this.props.location.state;
        return (
            <Layer>
                <div className="page-title" style={{ marginBottom: '10px' }}>离职</div>
                <LeaveForm
                    entityInfo={humanInfo}
                    subPageLoadCallback={(formObj, pageName) => this.subPageLoadCallback(formObj, pageName)}
                    setDepartmentOrgTree={this.props.setDepartmentOrgTree || []}
                />
                {/* <Row style={{ marginTop: '10px' }}>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="员工编号">
                                {getFieldDecorator('userID', {
                                    initialValue: humanInfo.userID,
                                    rules: [{
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
                                    rules: [{
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
                                    rules: [{
                                        required: true,
                                        message: 'please entry',
                                    }]
                                })(
                                    <TreeSelect disabled={true} treeData={this.props.setDepartmentOrgTree} />
                                )}
                            </FormItem>
                        </Col>
                    </Row>
                    <Row>
                        <Col span={7}>
                            <FormItem {...formItemLayout} label="离职日期">
                                {getFieldDecorator('leaveTime', {
                                    rules: [{
                                        required: true, message: '请输入离职日期',
                                    }]
                                })(
                                    <DatePicker format='YYYY-MM-DD' style={{ width: '100%' }} />
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
                                    rules: [{
                                        required: false, message: 'please entry',
                                    }]
                                })(
                                    <Checkbox >是否办理手续</Checkbox>
                                )}
                            </FormItem>
                        </Col>
                    </Row> */}
                <Row>
                    <Col span={20} style={{ textAlign: 'center' }}>
                        <Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button>
                    </Col>
                </Row>
            </Layer >

        );
    }
}

function tableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree,

    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Leave);