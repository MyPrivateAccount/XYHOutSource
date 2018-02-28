import { connect } from 'react-redux';
import { empAdd, empEdit, empDelete, empRoleEdit, empListGet, orgGetPermissionTree, empRestPwd } from '../../actions/actionCreator';
import React, { Component } from 'react'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import EmpEditor from './empEdit';
import EmployeeRoleEditor from './empRoleEdit';
import SearchCondition from '../../constants/searchCondition';

const { Header, Content } = Layout;
const Option = Select.Option;
const TreeNode = TreeSelect.TreeNode;

class EmpManager extends Component {
    state = {
        checkList: [],
        condition: { isSearch: true, keyWords: '', roleId: '', OrganizationIds: [] },//条件
        empDataLoading: false,
        pagination: {}
    }

    componentWillMount = () => {
        if (this.props.permissionOrgTree.AddUserTree.length === 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
        this.handleSearch();
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
        this.setState({ empDataLoading: false });
        let paginationInfo = {
            pageSize: newProps.empSearchResult.pageSize,
            current: newProps.empSearchResult.pageIndex,
            total: newProps.empSearchResult.totalCount
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
    handleUserNameChange = (e) => {
        //console.log("输入内容：", e.target.value);
        let condition = { ...this.state.condition };
        condition.keyWords = e.target.value;
        this.setState({ condition: condition });
    }

    handleSearch = (e) => {
        SearchCondition.empSearchCondition = this.state.condition;
        SearchCondition.empSearchCondition.pageIndex = 0;
        SearchCondition.empSearchCondition.pageSize = 10;
        console.log("查询条件", SearchCondition);
        this.setState({ empDataLoading: true });
        this.props.dispatch(empListGet(SearchCondition.empSearchCondition));
    }

    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.empSearchCondition.pageIndex = (pagination.current - 1);
        SearchCondition.empSearchCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.props.dispatch(empListGet(SearchCondition.empSearchCondition));
    };
    handleRoleChange = (role) => {
        console.log("角色变更:", role);
        let condition = { ...this.state.condition };
        condition.roleId = role;
        this.setState({ condition: condition });
    }
    handleOrgChange = (orgs) => {
        console.log("部门改变:", orgs);
        let condition = { ...this.state.condition };
        condition.OrganizationIds = orgs;
        this.setState({ condition: condition });
    }

    render() {
        let removeBtnDisabled = this.state.checkList.filter((item) => item.status).length > 0 ? false : true;
        let roleOptions = [];
        if (this.props.roleSource) {
            this.props.roleSource.map(role => roleOptions.push(<Option key={role.id}>{role.name}</Option>));
        }
        const loop = data => data.map((item) => {
            if (item.children) {
                return <TreeNode title={item.name} key={item.key} value={item.key}>{loop(item.children)}</TreeNode>;
            }
            return <TreeNode title={item.name} key={item.key} value={item.key} isLeaf={item.isLeaf} />;
        });
        const treeNodes = loop(this.props.treeSource);
        return (
            <div className="relative">
                <Layout>
                    <Header >
                        员工列表
                        {/* <Tooltip title="新增用户">
                            &nbsp;<Button type='primary' shape='circle' icon='plus' onClick={this.handleAddClick} />
                        </Tooltip>
                        <Tooltip title="删除用户">
                            <Popconfirm title="确认要删除选中用户?" onConfirm={this.handleEmpDelete} onCancel={this.cancelDelete} okText="确定" cancelText="取消">
                                &nbsp;<Button type='primary' shape='circle' icon='delete' disabled={removeBtnDisabled} />
                            </Popconfirm>
                        </Tooltip> */}
                    </Header>
                    <Content>
                        <Row>
                            <Col style={{ margin: '7px 5px' }}>
                                <div >
                                    <Row>
                                        <Col span={6}>
                                            用户名: <Input value={this.state.condition.keyWords} style={{ width: '70%', verticalAlign: 'middle' }} onPressEnter={this.handleSearch} onChange={this.handleUserNameChange} />
                                        </Col>
                                        <Col span={6}>
                                            所属角色: <Select style={{ width: '70%' }} value={this.state.condition.roleId} allowClear
                                                onChange={this.handleRoleChange}>
                                                {roleOptions}
                                            </Select>
                                        </Col>
                                        <Col span={6}>
                                            所属部门:
                                    <TreeSelect style={{ width: '70%' }} allowClear multiple
                                                dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                                onChange={this.handleOrgChange} value={this.state.condition.OrganizationIds}
                                                treeData={this.props.permissionOrgTree.AddUserTree}>
                                                {treeNodes}
                                            </TreeSelect>
                                        </Col>
                                        <Col span={6}>
                                            <Button type="primary" icon="search" onClick={this.handleSearch}>查询</Button>
                                        </Col>
                                    </Row>
                                </div>
                            </Col>
                        </Row>
                        <Spin spinning={this.state.empDataLoading}>
                            {<Table rowKey={record => record.userName} pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.empSearchResult.extension} onChange={this.handleTableChange} />}
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
    console.log("empTableMapStat:" + JSON.stringify(state.emp.empSearchResult));
    return {
        operInfo: state.emp.operInfo,
        activeTreeNode: state.org.activeTreeNode,
        empSearchResult: state.emp.empSearchResult,
        permissionOrgTree: state.org.permissionOrgTree,
        roleSource: state.role.roleSource,
        treeSource: state.org.treeSource,
    }
}

function emptableMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(emptableMapStateToProps, emptableMapDispatchToProps)(EmpManager);