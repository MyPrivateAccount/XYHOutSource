import { connect } from 'react-redux';
import { createStation, getOrgList, savePartTimeJob } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Select, Input, Form, Cascader, Button, Row, Col, TreeSelect, DatePicker, notification, Table, Modal } from 'antd'
import moment from 'moment';
import { NewGuid } from '../../../utils/appUtils';
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


class PartTimeJobPreview extends Component {
    state = {
        department: '',
        columns: [],
        addRowList: [],//新增的兼职列表
        curRowInfo: null//当前行对象
    }

    componentWillMount() {
        this.setState({ columns: formerCompanyColumns });
    }

    render() {
        let humanInfo = this.props.entityInfo;
        let tableColumns = this.state.columns || [];
        let tableDataSource = (humanInfo.jobList || []);
        return (
            <div>
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

                        </Select>
                    </Col>
                </Row>
                <Row style={{ marginTop: '10px', marginBottom: '10px' }}>
                    <Col>
                        <Button type="primary" onClick={() => this.setState({ isDialogShow: true })} >添加</Button>
                    </Col>
                </Row>

                <Table dataSource={tableDataSource} columns={tableColumns} />
            </div >
        );
    }
}

export default PartTimeJobPreview;