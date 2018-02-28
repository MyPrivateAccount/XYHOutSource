import { connect } from 'react-redux';
import { setLoadingVisible, empAdd, empEdit, empDelete, empRoleEdit, empListGet, orgGetPermissionTree, empRestPwd } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin } from 'antd'
import EmpEditor from './empEdit';
import EmployeeRoleEditor from './empRoleEdit';
import SearchCondition from '../../constants/searchCondition';

const { Header, Sider, Content } = Layout;

class EmpTable extends Component {
    state = {
        checkList: [],
        condition: { userName: '' },
        pagination: {}
    }

    componentWillMount = () => {
        //this.props.getDicGroupList();
        //this.props.dispatch(roleGetList());//加载角色列表
        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
    }

    handleEmpRoleClick = (empInfo) => {
        console.log("员工编辑：", empInfo);
        this.props.dispatch(empRoleEdit(empInfo));
    }

    appTableColumns = [
        {
            title: '选择', dataIndex: 'clientID', key: 'check', render: (text, recored) => (
                <span>
                    <Checkbox defaultChecked={false} onChange={(e) => this.handleCheckChange(e, recored)} />
                </span>
            )
        },
        { title: '用户名', dataIndex: 'userName', key: 'userName' },
        { title: '真实姓名', dataIndex: 'trueName', key: 'trueName' },
        { title: '联系电话', dataIndex: 'phoneNumber', key: 'phoneNumber' },
        { title: '邮箱', dataIndex: 'email', key: 'email' },
        {
            title: '编辑', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="用户编辑">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                    <Tooltip title='用户权限设置'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleEmpRoleClick(recored)} />
                    </Tooltip>
                    <Tooltip title="重置密码">
                        <Popconfirm title="确认要重置选中用户密码?" onConfirm={(e) => this.handleResetPwd(recored)} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                            &nbsp;<Button type='primary' shape='circle' size='small' icon='retweet' />
                        </Popconfirm>
                    </Tooltip>
                </span>
            )
        }
    ];
    componentWillReceiveProps(newProps) {
        let paginationInfo = {
            pageSize: newProps.empList.pageSize,
            current: newProps.empList.pageIndex,
            total: newProps.empList.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
    }
    handleEmpDelete = (e) => {//员工删除
        let deleteIds = [];
        this.state.checkList.map((item, i) => {
            if (item.status == true) {
                deleteIds.push(item.id);
            }
        });
        console.log("删除列表：", deleteIds);
        this.props.dispatch(empDelete(deleteIds));
    }
    handleResetPwd = (empInfo) => {//重置密码
        console.log("重置密码：", empInfo);
        this.props.dispatch(empRestPwd(empInfo.userName));
    }
    handleAddClick = (event) => {
        console.log('handleAddClick');
        this.props.dispatch(empAdd());
    }
    handleEditClick = (empInfo) => {
        console.log("handleEditClick", empInfo);
        this.props.dispatch(empEdit(empInfo));
    }
    handleCheckChange = (e, empInfo) => {
        //console.log("checkbox change：" + JSON.stringify(e.target.checked));
        let empId = empInfo.id;
        let checked = e.target.checked;
        let checkList = this.state.checkList.slice();
        var hasValue = false;
        for (var i in checkList) {
            if (checkList[i].id == empId) {
                checkList[i].status = checked;
                hasValue = true;
                break;
            }
        }
        if (!hasValue) {
            checkList.push({ id: empId, status: checked });
        }
        this.setState({ checkList: checkList });
    }
    // handleUserNameChange = (e) => {
    //     console.log("输入内容：", e.target.value);
    //     this.setState({ condition: { userName: e.target.value } });
    // }

    // handleSearch = (e) => {
    //     // var condition = Object.assign({}, SearchCondition.empListCondition);
    //     // condition.OrganizationIds = [];
    //     // condition.keyWords = this.state.condition.userName;
    //     SearchCondition.empListCondition.OrganizationIds = [];//[this.props.activeTreeNode.id];
    //     SearchCondition.empListCondition.keyWords = this.state.condition.userName;
    //     console.log("查询条件", SearchCondition);
    //     this.setState({ empDataLoading: true });
    //     this.props.dispatch(empListGet(SearchCondition.empListCondition));
    // }

    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.empListCondition.pageIndex = (pagination.current - 1);
        SearchCondition.empListCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.props.dispatch(empListGet(SearchCondition.empListCondition));
    };

    render() {
        let removeBtnDisabled = this.state.checkList.filter((item) => item.status).length > 0 ? false : true;
        return (
            <div className="relative">
                <Layout>
                    <Header style={{ backgroundColor: '#ececec' }}>
                        员工列表
                            &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                        <Popconfirm title="确认要删除选中用户?" onConfirm={this.handleEmpDelete} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                            &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} />
                        </Popconfirm>
                    </Header>
                    <Content>
                        {/* <Row>
                            <Col style={{ margin: '7px 5px' }}>
                                <div >
                                    用户名: <Input value={this.state.condition.userName} style={{ width: '220px', verticalAlign: 'middle' }} onPressEnter={this.handleSearch} onChange={this.handleUserNameChange} />
                                    &nbsp;<Button type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                                </div>
                            </Col>
                        </Row> */}
                        <Spin spinning={this.props.showLoading}>
                            {<Table rowKey={record => record.userName} pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.empList.extension} onChange={this.handleTableChange} />}
                        </Spin>
                        <EmpEditor />
                        <EmployeeRoleEditor />
                    </Content>
                </Layout>
            </div >
        )
    }
}

function emptableMapStateToProps(state) {
    console.log("empTableMapStat:" + JSON.stringify(state.emp.empList));
    return {
        operInfo: state.emp.operInfo,
        activeTreeNode: state.org.activeTreeNode,
        empList: state.emp.empList,
        permissionOrgTree: state.org.permissionOrgTree,
        showLoading: state.emp.showLoading
    }
}

function emptableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(emptableMapStateToProps, emptableMapDispatchToProps)(EmpTable);