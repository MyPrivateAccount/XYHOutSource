import {connect} from 'react-redux';
import {createStation, getOrgList, leavePosition} from '../../actions/actionCreator';
import React, {Component} from 'react'
import {Select, Input, Form, Cascader, Button, Row, Col, Checkbox, DatePicker, Spin, Table} from 'antd'
import Layer from '../../../components/Layer'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: {span: 6},
    wrapperCol: {span: 17},
};

const formerCompanyColumns = [{
    title: '兼职部门',
    dataIndex: 'company',
    key: 'company',
}, {
    title: '岗位',
    dataIndex: '兼职职位',
    key: 'position',
}, {
    title: '开始时间',
    dataIndex: 'startTime',
    key: 'startTime',
}, {
    title: '结束时间',
    dataIndex: 'endTime',
    key: 'endTime',
}];


class PartTimeJob extends Component {
    state = {
        department: '',
        columns: []
    }

    componentWillMount() {
        let operColumn = {
            title: '操作',
            dataIndex: 'proveMan',
            render: (text, record) => {
                return (
                    <div>
                        <Button type="primary" size='small' style={{marginRight: '5px'}} onClick={() => this.Invalid(record)} >失效</Button>
                    </div>
                )
            },
            key: 'proveMan',
        }
        this.setState({columns: formerCompanyColumns.concat(operColumn)});
    }
    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }
    //兼职失效
    Invalid = (record) => {

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
        let tableColumns = this.state.columns || [];
        return (
            <Layer>
                <div className="page-title" style={{marginBottom: '10px'}}>兼职</div>
                <Form onSubmit={this.handleSubmit}>
                    <Row style={{marginTop: '10px'}}>
                        <Col span={6}>
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
                        <Col span={6}>
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
                        <Col span={6}>
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
                        <Col span={6}>
                            <FormItem {...formItemLayout} label="职位">
                                {getFieldDecorator('orgStation', {
                                    reules: [{
                                        required: true, message: 'please entry',
                                    }],
                                    initialValue: null
                                })(
                                    <Select disabled={true} onChange={this.handleSelectChange} placeholder="选择职位">

                                    </Select>
                                )}
                            </FormItem>
                        </Col>
                    </Row>

                    <Table dataSource={[]} columns={tableColumns} />

                    <Row>
                        <Col span={20} style={{textAlign: 'center', marginTop: '10px'}}>
                            <Button type="primary" htmlType="submit" disabled={this.hasErrors(getFieldsValue())} >提交</Button>
                        </Col>
                    </Row>

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
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(PartTimeJob));