//业绩分摊项设置页面
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import {acmentParamAdd,acmentParamEdit,acmentParamDel,acmentParamListGet,orgGetPermissionTree} from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import AcmentEditor from './acmentEditor'

class AcmentSet extends Component{
    state = {
        pagination: {},
        isDataLoading:false,
    }
    appTableColumns = [
        { title: '分摊项名称', dataIndex: 'ftName', key: 'ftName' },
        { title: '分摊类型', dataIndex: 'ftType', key: 'ftType' },
        { title: '默认分摊比例', dataIndex: 'ftScale', key: 'ftScale' },
        { title: '是否固定比例', dataIndex: 'isFixScale', key: 'isFixScale' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="作废">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                    <Tooltip title='修改'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) =>{
        this.props.dispatch(acmentParamDel(info));
    }
    handleModClick = (info) =>{
        this.props.dispatch(acmentParamEdit(info));
    }
    handleNew = (info)=>{
        this.props.dispatch(acmentParamAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        SearchCondition.acmentListCondition.pageIndex = 0;
        SearchCondition.acmentListCondition.pageSize = 10;
        SearchCondition.acmentListCondition.OrganizationId = e;
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
    componentDidMount = ()=>{
        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
        this.handleSearch();
    }
    componentWillReceiveProps = (newProps)=>{
        this.setState({isDataLoading:false});
        let paginationInfo = {
            pageSize: newProps.scaleSearchResult.pageSize,
            current: newProps.scaleSearchResult.pageIndex,
            total: newProps.scaleSearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
    }
    render(){
        return (
            <Layout>
                <div style={{'margin':5}}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.AddUserTree}
                                    placeholder="所属组织"
                                    defaultValue={this.props.orgid}
                                    onChange={this.handleSearch}
                                    >
                    </TreeSelect>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                 <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.scaleSearchResult.ext} onChange={this.handleTableChange}></Table>
                 </Spin>
                 <AcmentEditor/>
            </Layout>
        )
    }
}
function MapStateToProps(state){
    return {
        activeTreeNode:    state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        scaleSearchResult: state.acm.scaleSearchResult
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(AcmentSet);