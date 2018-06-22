//组织参数设置
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { orgParamAdd, orgParamEdit, orgParamListGet, orgGetPermissionTree } from '../../actions/actionCreator'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import SearchCondition from '../../constants/searchCondition'
import OrgParamEditor from './orgParamEditor'
const { Header, Content } = Layout;
const Option = Select.Option;

class OrgParamSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        branchId: ''
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'name', key: 'name' },
        { title: '参数名称', dataIndex: 'parValue', key: 'parValue' },
        { title: '参数值', dataIndex: 'parCode', key: 'parCode' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Popconfirm title="是否删除该项?" onConfirm={this.zfconfirm} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                        <Button type='primary' shape='circle' size='small' icon='delete' />
                    </Popconfirm>
                    <Tooltip title='修改'>
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleModClick = (info) => {
        this.props.dispatch(orgParamEdit(info));
    }
    handleNew = (e) => {
        console.log(e);
        this.props.dispatch(orgParamAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        this.setState({ branchId: e })
        SearchCondition.orgParamListCondition.branchId = e;
        console.log("查询条件", SearchCondition.orgParamListCondition);
        this.setState({ isDataLoading: true });
        this.props.dispatch(orgParamListGet(SearchCondition.orgParamListCondition));
    }
    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.orgParamListCondition.pageIndex = (pagination.current - 1);
        SearchCondition.orgParamListCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.props.dispatch(orgParamListGet(SearchCondition.orgParamListCondition));
    };
    componentDidMount = () => {
        this.setState({ isDataLoading: true })
        this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
    }
    componentWillReceiveProps = (newProps) => {
        this.setState({ isDataLoading: false });
        let paginationInfo = {
            pageSize: newProps.orgParamSearchResult.pageSize,
            current: newProps.orgParamSearchResult.pageIndex,
            total: newProps.orgParamSearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
        if (newProps.operInfo.operType === 'org_update') {
            console.log('org_update')
            this.setState({ isDataLoading: true, branchId: newProps.permissionOrgTree.AddUserTree[0].key })
            this.handleSearch(newProps.permissionOrgTree.AddUserTree[0].key)
            newProps.operInfo.operType = ''
        }
    }
    getListData = () => {
        if (this.props.orgParamSearchResult.extension == null) {
            return null
        }
        let data = this.props.orgParamSearchResult.extension;
        for (let i = 0; i < data.length; i++) {
            data[i].name = this.getOrgNameById(data[i].branchId)
            }
        return data
    }
    //根据组织id获取组织名称
    getOrgNameById = (branchId) => {
        return '测试'
    }
    render() {
        return (
            <Layout>
                <div style={{ 'margin': 5 }}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                        treeData={this.props.permissionOrgTree.AddUserTree}
                        placeholder="所属组织"
                        value={this.state.branchId}
                        onChange={this.handleSearch}>
                    </TreeSelect>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{ 'margin': '10' }} />
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.getListData()} ></Table>
                </Spin>
                <OrgParamEditor orgid={this.state.branchId} />
            </Layout>
        )
    }
}
function MapStateToProps(state) {
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        orgParamSearchResult: state.orgparam.orgParamSearchResult,
        operInfo: state.org.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(OrgParamSet);