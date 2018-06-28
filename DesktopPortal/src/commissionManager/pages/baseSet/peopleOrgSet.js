//人数分摊组织设置页面
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import { orgFtParamAdd, orgFtParamUpdate, orgFtParamSave, orgFtDialogClose, orgGetPermissionTree, orgFtParamDelete, orgFtParamListGet } from '../../actions/actionCreator'
import PeopleOrgFtEditor from './peopleOrgFtEditor'
import SearchCondition from '../../constants/searchCondition'

const { Header, Content } = Layout;
const Option = Select.Option;

class PeopleSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        branchId: '',
        requirePermission:['YJ_RSFTZZSZ']
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'shareName', key: 'shareName' },
        { title: '分摊比例', dataIndex: 'shareRatio', key: 'shareRatio' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                this.hasPermission(this.state.requirePermission)?
                <span>
                    <Popconfirm title="是否删除该分摊项?" onConfirm={(e)=>this.zfconfirm(recored)} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                        <Button type='primary' shape='circle' size='small' icon='delete' />
                    </Popconfirm>
                    <Tooltip title='修改'>
                            <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleEditClick(recored)} />
                    </Tooltip>
                </span>
                :null
            )
        }
    ];
    zfconfirm=(e)=>{
        this.handleDelClick(e)
    }
    zfcancel=(e)=>{

    }
    handleDelClick = (info) => {
        this.props.dispatch(orgFtParamDelete(info));
    }
    handleEditClick = (info) => {
        this.props.dispatch(orgFtParamUpdate(info));
    }
    handleNew = (info) => {
        console.log(info);
        this.props.dispatch(orgFtParamAdd(this.state.branchId));
    }
    handleSearch = (e) => {
        console.log(e)
        this.setState({branchId:e})
        SearchCondition.ppFtListCondition.branchId = e;
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
    componentDidMount = () => {
        this.setState({isDataLoading:true})
        this.props.dispatch(orgGetPermissionTree("YJ_RSFTZZSZ_CK"));
    }
    componentWillReceiveProps = (newProps) => {
        this.setState({ isDataLoading: false });
        let paginationInfo = {
            pageSize: newProps.ppFtSearchResult.pageSize,
            current: newProps.ppFtSearchResult.pageIndex,
            total: newProps.ppFtSearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });
        if (newProps.operInfo.operType === 'org_update') {
            console.log('org_update')
            this.setState({isDataLoading: true ,branchId:newProps.permissionOrgTree.BaseSetOrgTree[0].key})
            this.handleSearch(newProps.permissionOrgTree.BaseSetOrgTree[0].key)
            newProps.operInfo.operType = ''
        }
        if(newProps.ppftOp.operType === 'ORG_FT_PARAM_DELETE_UPDATE'){
            this.handleSearch(this.state.branchId)
            newProps.ppftOp.operType = ''
        }
        if(newProps.ppftOp.operType === 'FT_PARAM_SAVE_SUCCESS'){
            this.handleSearch(this.state.branchId)
            this.props.dispatch(orgFtDialogClose())
            newProps.ppftOp.operType = ''
        }

    }
    getListData=()=>{
        if(this.props.ppFtSearchResult.extension == null){
            return null
        }
        let data = this.props.ppFtSearchResult.extension;
        if(this.props.ppftOp.operType!=='ORG_FT_PARAMLIST_UPDATE'){
            return data
        }
            for(let i=0;i<data.length;i++){
                data[i].shareRatio = data[i].shareRatio*100+'%'
                data[i].shareName = data[i].shareName
            }
        return data
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
    render() {
        return (
            <Layout>
                <div style={{ 'margin': 5 }}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                        treeData={this.props.permissionOrgTree.BaseSetOrgTree}
                        placeholder="所属组织"
                        value = {this.state.branchId}
                        onChange={this.handleSearch}>
                    </TreeSelect>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{ 'margin': 10 }} />
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.getListData()}></Table>
                </Spin>
                <PeopleOrgFtEditor/>
            </Layout>
        )
    }
}
function peoMapStateToProps(state) {
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        ppFtSearchResult: state.ppft.ppFtSearchResult,
        operInfo:state.org.operInfo,
        ppftOp:state.ppft.operInfo,
    }
}

function peoMapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(peoMapStateToProps, peoMapDispatchToProps)(PeopleSet);