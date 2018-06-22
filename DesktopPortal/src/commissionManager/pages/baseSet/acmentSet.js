//业绩分摊项设置页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import { acmentParamAdd, acmentParamEdit, acmentParamDel, acmentParamListGet, orgGetPermissionTree } from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import AcmentEditor from './acmentEditor'

class AcmentSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        branchId: ''
    }
    appTableColumns = [
        { title: '分摊项名称', dataIndex: 'name', key: 'name' },
        { title: '分摊类型', dataIndex: 'type', key: 'type' },
        { title: '默认分摊比例', dataIndex: 'percent', key: 'percent' },
        { title: '是否固定比例', dataIndex: 'isfixed', key: 'isfixed' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title='删除'>
                        <Button type='primary' shape='circle' size='small' icon='delete' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                    <Tooltip title='修改'>
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) => {
        info.branchId = this.state.branchId;
        this.props.dispatch(acmentParamDel(info));
    }
    handleModClick = (info) => {
        this.props.dispatch(acmentParamEdit(info));
    }
    handleNew = (info) => {
        this.props.dispatch(acmentParamAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        this.setState({ branchId: e })
        SearchCondition.acmentListCondition.branchId = e;
        console.log("查询条件", SearchCondition.acmentListCondition);
        this.setState({ isDataLoading: true });
        this.props.dispatch(acmentParamListGet(SearchCondition.acmentListCondition));
    }
    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.acmentListCondition.pageIndex = (pagination.current - 1);
        SearchCondition.acmentListCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.props.dispatch(acmentParamListGet(SearchCondition.acmentListCondition));
    };
    componentDidMount = () => {
        this.setState({ isDataLoading: true })
        this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
    }
    componentWillReceiveProps = (newProps) => {

        this.setState({ isDataLoading: false });

        if (newProps.operInfo.operType === 'ACMENT_PARAM_LIST_UPDATE') {

            let paginationInfo = {
                pageSize: newProps.scaleSearchResult.pageSize,
                current: newProps.scaleSearchResult.pageIndex,
                total: newProps.scaleSearchResult.totalCount
            };
            console.log("分页信息：", paginationInfo);
            this.setState({ pagination: paginationInfo });
            newProps.operInfo.operType = ''
        }

        if (newProps.operInfo.operType === 'org_update') {
            console.log('org_update')
            this.handleSearch(newProps.permissionOrgTree.AddUserTree[0].key)
            this.setState({ branchId: newProps.permissionOrgTree.AddUserTree[0].key })
            newProps.operInfo.operType = ''
        }
        if (newProps.acmOp.operType === 'ACMENT_PARAM_DEL_UPDATE'||
            newProps.acmOp.operType === 'ACMENT_PARAM_UPDATE') {
            this.handleSearch(this.state.branchId)
            newProps.acmOp.operType = ''
        }
    }
    getListData = () => {
        if (this.props.scaleSearchResult.extension == null) {
            return null
        }
        let data = this.props.scaleSearchResult.extension;
        if(this.props.acmOp.operType!=='ACMENT_PARAM_LIST_UPDATE'){
            return data
        }
        for (let i = 0; i < data.length; i++) {
            if (data[i].isfixed) {
                data[i].isfixed = '是'
            }
            else {
                data[i].isfixed = '否'
            }
            if (data[i].type === 1) {
                data[i].type = '外部佣金'
            }
            else {
                data[i].type = '内部分配项'
            }
            data[i].percent = data[i].percent * 100 + '%'
        }
        return data
    }
    render() {
        return (
            <Layout>
                <Spin spinning={this.state.isDataLoading}>
                    <div style={{ 'margin': 5 }}>
                        组织：
                    <TreeSelect style={{ width: 300 }}
                            dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                            treeData={this.props.permissionOrgTree.AddUserTree}
                            placeholder="所属组织"
                            onChange={this.handleSearch}
                            value={this.state.branchId}
                        >
                        </TreeSelect>
                    </div>
                    <Tooltip title="新增">
                        <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{ 'margin': '10' }} />
                    </Tooltip>
                    <Table columns={this.appTableColumns} dataSource={this.getListData()}></Table>
                    <AcmentEditor orgid={this.state.branchId} />
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        scaleSearchResult: state.acm.scaleSearchResult,
        operInfo: state.org.operInfo,
        acmOp: state.acm.operInfo,
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(AcmentSet);