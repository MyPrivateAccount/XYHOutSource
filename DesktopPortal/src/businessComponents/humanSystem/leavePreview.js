import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, TreeSelect} from 'antd'
import SocialSecurity from './socialSecurity'
import moment from 'moment'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
};

class LeavePreview extends Component {
    state = {

    }
    componentDidMount() {
        if (this.props.subPageLoadCallback) {
            this.props.subPageLoadCallback(this.props.form, 'leave')
        }
    }

    render() {
        let entityInfo = this.props.entityInfo;
        let disabled = (this.props.readOnly || false);
        const {getFieldDecorator} = this.props.form;
        let setDepartmentOrgTree = this.props.setDepartmentOrgTree || [];
        let handleOverList = this.props.handleOverList || [];//交接人
        return (
            <Form >
                <Row style={{marginTop: '10px'}}>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="员工编号">
                            {getFieldDecorator('userID', {
                                initialValue: entityInfo.userID,
                                rules: [{
                                    required: true, message: '请输入员工编号',
                                }]
                            })(
                                <Input disabled={disabled} placeholder="请输入员工编号" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="姓名">
                            {getFieldDecorator('name', {
                                initialValue: entityInfo.name,
                                rules: [{
                                    required: true, message: '请输入姓名',
                                }]
                            })(
                                <Input disabled={disabled} placeholder="请输入姓名" />
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="部门">
                            {getFieldDecorator('departmentId', {
                                initialValue: entityInfo.departmentId,
                                rules: [{
                                    required: true,
                                    message: 'please entry',
                                }]
                            })(
                                <TreeSelect disabled={disabled} treeData={setDepartmentOrgTree} />
                            )}
                        </FormItem>
                    </Col>
                </Row>
                <Row>
                    <Col span={7}>
                        <FormItem {...formItemLayout} label="离职日期">
                            {getFieldDecorator('leaveTime', {
                                initialValue: entityInfo.leaveTime ? moment(entityInfo.leaveTime) : moment(),
                                rules: [{
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
                                initialValue: entityInfo.handover,
                                rules: [{
                                    required: true, message: '请选择交接人',
                                }]
                            })(
                                <Select disabled={this.props.ismodify == 1} placeholder="请选择交接人">
                                    {handleOverList.map(item => <Option key={item.id}>{item.trueName}</Option>)}
                                </Select>
                            )}
                        </FormItem>
                    </Col>
                    <Col span={7}>
                        <FormItem {...formItemLayout} colon={false} label=" ">
                            {getFieldDecorator('isFormalities', {
                                initialValue: entityInfo.isFormalities,
                                rules: []
                            })(
                                <Checkbox >是否办理手续</Checkbox>
                            )}
                        </FormItem>
                    </Col>
                </Row>
            </Form>
        )
    }

}
export default Form.create()(LeavePreview);
