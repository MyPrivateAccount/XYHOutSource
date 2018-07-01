//业绩分摊项设置页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import { acmentParamAdd, acmentParamEdit, acmentParamDel, acmentParamListGet, orgGetPermissionTree } from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import AcmentEditor from './acmentEditor'
import {keepTwoDecimalFull} from '../../constants/utils'

class AcmentSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        branchId: '',
        orgList: [],
        requirePermission:['YJ_YJFTSZ']
    }
    appTableColumns = [
        { title: '分摊项名称', dataIndex: 'name', key: 'name' },
        { title: '分摊类型', dataIndex: 'type', key: 'type' },
        { title: '默认分摊比例', dataIndex: 'percent', key: 'percent' },
        { title: '是否固定比例', dataIndex: 'isfixed', key: 'isfixed' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                (this.state.branchId === recored.branchId && this.hasPermission(this.state.requirePermission)) ?
                    <span>
                        <Tooltip title='删除'>
                            <Button type='primary' shape='circle' size='small' icon='delete' onClick={(e) => this.handleDelClick(recored)} />
                        </Tooltip>
                        <Tooltip title='修改'>
                            <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleModClick(recored)} />
                        </Tooltip>
                    </span>
                    : null
            )
        }
    ];
    handleDelClick = (info) => {
        info.branchId = this.state.branchId;
        this.props.dispatch(acmentParamDel(info));
    }
    handleModClick = (info) => {
        info.branchId = this.state.branchId
        this.props.dispatch(acmentParamEdit(info));
    }
    handleNew = (info) => {
        this.props.dispatch(acmentParamAdd(this.state.branchId));
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
        console.log("AcmentSet dispatch user info:")
        console.log(this.props.user)
        this.props.dispatch(orgGetPermissionTree("YJ_YJFTSZ_CK"));
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

        if (newProps.operInfo.operType === 'YJ_YJFTSZ_CK') {
            console.log('YJ_YJFTSZ_CK')
            this.handleSearch(newProps.permissionOrgTree.BaseSetOrgTree[0].key)
            this.setState({ branchId: newProps.permissionOrgTree.BaseSetOrgTree[0].key })
            newProps.operInfo.operType = ''
        }
        if (newProps.acmOp.operType === 'ACMENT_PARAM_DEL_UPDATE' ||
            newProps.acmOp.operType === 'ACMENT_PARAM_UPDATE') {
            this.handleSearch(this.state.branchId)
            newProps.acmOp.operType = ''
        }
    }
    //是否有权限
    hasPermission=(requirePermission)=>{
        let hasPermission = false;
        if (this.props.judgePermissions && requirePermission) {
            for (let i = 0; i < requirePermission.length; i++) {
                if (this.props.judgePermissions.includes(requirePermission[i])) {
                    hasPermission = true;
                    break;
                }
            }
        } else {
            hasPermission = true;
        }
        return hasPermission;
    }
    getListData = () => {
        if (this.props.scaleSearchResult.extension == null) {
            return null
        }
        let data = this.props.scaleSearchResult.extension;
        if (this.props.acmOp.operType !== 'ACMENT_PARAM_LIST_UPDATE') {
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
            let percent = keepTwoDecimalFull(data[i].percent*100)
            data[i].percent = percent + '%'
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
                            treeData={this.props.permissionOrgTree.BaseSetOrgTree}
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