import { connect } from 'react-redux';
import { createStation, getOrgList } from '../../actions/actionCreator';
import React, { Component } from 'react'
import {Table, Input, Form, Cascader, Button, Row, Col, Spin} from 'antd'

const FormItem = Form.Item;

const styles = {
    conditionRow: {
        width: '80px',
        display: 'inline-block',
        fontWeight: 'bold',
    },
    bSpan: {
        fontWeight: 'bold',
    },
    otherbtn: {
        padding: '0px, 5px',
    }
}
const ListColums = [
    { title: '职位名称', dataIndex: 'stationname', key: 'stationname' },
]
const rowSelection = {
    onChange: (selectedRowKeys, selectedRows) => {
      console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows);
    },
    getCheckboxProps: record => ({
      disabled: record.name === 'Disabled User', // Column configuration not to be checked
      name: record.name,
    }),
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
            <div style={{display: "block"}}>
                <Row className='searchBox'>
                    <Col span={12}>
                        <label style={styles.conditionRow}>选择分公司 ：</label>
                        <Cascader options={this.props.setContractOrgTree}  onChange={this.handleChooseDepartmentChange } changeOnSelect  placeholder="归属部门"/>
                    </Col>
                </Row>
                <Row>
                    <Col span={6}>
                        <Button type="primary" onClick={this.handleSearchBoxToggle}>新建</Button>
                    </Col>
                    <Col span={6}>
                        <Button type="primary" onClick={this.handleSearchBoxToggle}>修改</Button>
                    </Col>
                    <Col span={6}>
                        <Button type="primary" onClick={this.handleSearchBoxToggle}>删除</Button>
                    </Col>
                </Row>
                <div style={{display: this.props.searchInfo.expandSearchBox ? "block" : "none"}}>
                <Table rowSelection={rowSelection} rowKey={record => record.key} columns={ListColums} dataSource={this.props.searchInfo.searchResult.extension} onChange={this.handleTableChange} />
                </div>
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
export default connect(tableMapStateToProps, tableMapDispatchToProps)(Form.create()(Station));