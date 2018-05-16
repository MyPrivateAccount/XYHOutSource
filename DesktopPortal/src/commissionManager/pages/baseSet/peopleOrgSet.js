//人数分摊组织设置页面
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import {orgFtParamAdd, orgFtParamUpdate, orgFtParamSave, orgFtDialogClose,orgGetPermissionTree, orgFtParamDelete,orgFtParamListGet} from '../../actions/actionCreator'
import PeopleOrgFtEditor from './peopleOrgFtEditor'
import SearchCondition from '../../constants/searchCondition'

const { Header, Content } = Layout;
const Option = Select.Option;

class PeopleSet extends Component{
    state = {
        pagination: {},
        isDataLoading:false,
        orgid:''
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '分摊比例', dataIndex: 'ftScale', key: 'ftScale' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title='编辑'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                    <Tooltip title='删除'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) =>{
        this.props.dispatch(orgFtParamDelete(info));
    }
    handleEditClick = (info) =>{
        this.props.dispatch(orgFtParamUpdate(info));
    }
    handleNew = (info)=>{
        console.log(info);
        this.props.dispatch(orgFtParamAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        SearchCondition.ppFtListCondition.pageIndex = 0;
        SearchCondition.ppFtListCondition.pageSize = 10;
        SearchCondition.ppFtListCondition.OrganizationId = e;
        console.log("查询条件", SearchCondition);
        this.setState({ isDataLoading: true });
        this.props.dispatch(orgFtParamListGet(SearchCondition.ppFtListCondition));
    }
    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.ppFtListCondition.pageIndex = (pagination.current - 1);
        SearchCondition.ppFtListCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.props.dispatch(orgFtParamListGet(SearchCondition.empSearchCondition));
    };
    componentDidMount = ()=>{
        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
        this.handleSearch();
    }
    componentWillReceiveProps = (newProps)=>{
        this.setState({ isDataLoading: false });
        let paginationInfo = {
            pageSize: newProps.ppFtSearchResult.pageSize,
            current: newProps.ppFtSearchResult.pageIndex,
            total: newProps.ppFtSearchResult.totalCount
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
                                    onChange={this.handleSearch}>
                    </TreeSelect>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':10}}/>
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                 <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.ppFtSearchResult.ext} onChange={this.handleTableChange}></Table>
                 </Spin>
                <PeopleOrgFtEditor/>
            </Layout>
        )
    }
}
function peoMapStateToProps(state){
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        ppFtSearchResult:state.ppft.ppFtSearchResult,
        orgid:state.org.permissionOrgTree.AddUserTree[0].value
    }
}

function peoMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(peoMapStateToProps, peoMapDispatchToProps)(PeopleSet);