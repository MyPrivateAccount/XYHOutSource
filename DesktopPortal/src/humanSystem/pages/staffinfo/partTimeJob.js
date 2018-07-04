import { connect } from 'react-redux';
import { createStation, getOrgList, savePartTimeJob } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Input, Form, Cascader, Button, Row, Col, TreeSelect, DatePicker, notification, Table, Modal } from 'antd'
import Layer from '../../../components/Layer'
import moment from 'moment';
import { NewGuid } from '../../../utils/appUtils';
import { getHumanDetail, getPosition } from '../../serviceAPI/staffService'
const FormItem = Form.Item;
const Option = Select.Option;
const formItemLayout = {
    labelCol: { span: 6 },
    wrapperCol: { span: 17 },
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
        columns: [],
        isModify: false,
        isDialogShow: false,
        addRowList: [],//新增的兼职列表
        showLoading: false,
        positionList: [],//职位列表
        humanInfo: null//当前行对象
    }

    componentDidMount() {
        let operColumn = {
            title: '操作',
            dataIndex: 'proveMan',
            render: (text, record) => {
                return (
                    <div>
                        <Button type="primary" size='small' style={{ marginRight: '5px' }} onClick={() => this.Invalid(record)} >失效</Button>
                    </div>
                )
            },
            key: 'proveMan',
        }
        this.setState({ columns: formerCompanyColumns.concat(operColumn) });
        let humanInfo = this.props.location.state;
        if (humanInfo.id && humanInfo.departmentId) {
            this.setState({ showLoading: true });
            getPosition(humanInfo.departmentId).then(res => {
                if (res.isOk) {
                    this.setState({ positionList: res.extension || [] });
                    getHumanDetail(humanInfo.id).then(res => {
                        this.setState({ humanInfo: res.extension || {}, showLoading: true });
                    })
                }
            });

        }
    }
    hasErrors(fieldsError) {
        return !Object.keys(fieldsError).some(field => fieldsError[field]);
    }
    //兼职失效
    Invalid = (record) => {
        let addRowList = this.state.addRowList;
        let index = addRowList.findIndex(item => item.id == record.id);
        if (index != -1) {
            addRowList.splice(index, 1);
            this.setState({ addRowList: addRowList });
        } else {

        }
    }

    handleSubmit = (e) => {
        this.props.dispatch(savePartTimeJob({}));
    }
    AddRow = (e) => {
        // e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                let addRowList = this.state.addRowList;
                values.id = NewGuid();
                addRowList.push(values);
                this.setState({ addRowList: addRowList });
            }
        });
    }

    render() {
        let humanInfo = this.state.humanInfo || {};
        const { getFieldDecorator } = this.props.form;
        let tableColumns = this.state.columns || [];
        let tableDataSource = (this.state.addRowList || []);
        return (
            <Layer showLoading={this.state.showLoading}>
                <div className="page-title" style={{ marginBottom: '10px' }}>兼职</div>
                <Row style={{ marginTop: '10px' }}>
                    <Col span={6}>
                        <label class="ant-form-item-label">员工编号：</label>
                        <Input disabled={true} style={{ width: '200px' }} value={humanInfo.userID} />
                    </Col>
                    <Col span={6}>
                        <label class="ant-form-item-label">姓名：</label>
                        <Input disabled={true} style={{ width: '200px' }} value={humanInfo.name} />
                    </Col>
                    <Col span={6}>
                        <label class="ant-form-item-label">部门：</label>
                        <TreeSelect disabled={true} style={{ width: '200px' }} value={humanInfo.departmentId} treeData={this.props.setDepartmentOrgTree} />
                    </Col>
                    <Col span={6}>
                        <label class="ant-form-item-label">职位：</label>
                        <Select style={{ width: '200px' }} disabled={true} onChange={this.handleSelectChange} placeholder="选择职位">
                            {(this.state.positionList || []).map(p => <Option key={p.id} value={p.id}>{p.positionName}</Option>)}
                        </Select>
                    </Col>
                </Row>
                <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                    <Col>
                        <Button type="primary" onClick={() => this.setState({ isDialogShow: true })} >添加</Button>
                    </Col>
                </Row>

                <Table dataSource={tableDataSource} columns={tableColumns} />

                <Row>
                    <Col span={20} style={{ textAlign: 'center', marginTop: '10px' }}>
                        <Button type="primary" onClick={() => this.handleSubmit()}  >提交</Button>
                    </Col>
                </Row>

                <Modal title={this.state.isModify ? "修改" : "新增兼职"}
                    visible={this.state.isDialogShow}
                    onOk={() => this.AddRow()} onCancel={() => this.setState({ isDialogShow: false })}>
                    <Form onSubmit={this.handleSubmit}>
                        <Row>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label="部门">
                                    {getFieldDecorator('departmentId', {
                                        rules: [{
                                            required: true,
                                            message: '选择部门',
                                        }]
                                    })(
                                        // <Cascader disabled={true} options={this.props.setDepartmentOrgTree} onChange={this.handleChooseDepartmentChange} onPopupVisibleChange={this.handleDepartmentChange} changeOnSelect placeholder="归属部门" />
                                        <TreeSelect allowClear treeData={this.props.setDepartmentOrgTree} />
                                    )}
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label="职位">
                                    {getFieldDecorator('position', {
                                        rules: [{
                                            required: true, message: '请选择职位',
                                        }]
                                    })(
                                        <Select onChange={this.handleSelectChange} placeholder="选择职位">

                                        </Select>
                                    )}
                                </FormItem>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label="开始时间">
                                    {getFieldDecorator('startTime', {
                                        initialValue: moment(),
                                        rules: [{
                                            required: true,
                                            message: '请选择开始时间'
                                        }]
                                    })(
                                        <DatePicker format='YYYY-MM-DD' style={{ width: '100%' }} />
                                    )}
                                </FormItem>
                            </Col>
                            <Col span={12}>
                                <FormItem {...formItemLayout} label="结束时间">
                                    {getFieldDecorator('endTime', {
                                        initialValue: moment(),
                                        rules: [{
                                            required: true,
                                            message: '请选择结束时间'
                                        }]
                                    })(
                                        <DatePicker format='YYYY-MM-DD' style={{ width: '100%' }} />
                                    )}
                                </FormItem>
                            </Col>
                        </Row>
                    </Form>
                </Modal>
            </Layer >

        );
    }
}

function tableMapStateToProps(state) {
    return {
        setDepartmentOrgTree: state.basicData.searchOrgTree
    }
}

function tableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(PartTimeJob));