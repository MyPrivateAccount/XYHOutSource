//组织参数设置
import { connect } from 'react-redux';
import React,{Component} from 'react';
import {orgParamAdd,orgParamEdit,orgParamListGet,orgGetPermissionTree} from '../../actions/actionCreator'
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import SearchCondition from '../../constants/searchCondition'
import OrgParamEditor from './orgParamEditor'
const { Header, Content } = Layout;
const Option = Select.Option;

class OrgParamSet extends Component{
    state = {
        pagination: {},
        isDataLoading:false,
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '参数名称', dataIndex: 'paramName', key: 'paramName' },
        { title: '参数值', dataIndex: 'paramVal', key: 'paramVal' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='修改'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleModClick = (info) =>{
        this.props.dispatch(orgParamEdit(info));
    }
    handleNew = (e)=>{
        console.log(e);
        this.props.dispatch(orgParamAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        SearchCondition.orgParamListCondition.pageIndex = 0;
        SearchCondition.orgParamListCondition.pageSize = 10;
        SearchCondition.orgParamListCondition.OrganizationId = e;
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
    componentDidMount = ()=>{

        if (this.props.permissionOrgTree.AddUserTree.length == 0) {
            this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        }
        this.handleSearch();
    }
    componentWillReceiveProps = (newProps)=>{
        this.setState({isDataLoading:false});
        let paginationInfo = {
            pageSize: newProps.orgParamSearchResult.pageSize,
            current: newProps.orgParamSearchResult.pageIndex,
            total: newProps.orgParamSearchResult.totalCount
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
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                 <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.orgParamSearchResult.ext} onChange={this.handleTableChange}></Table>
                 </Spin>
                 <OrgParamEditor/>
            </Layout>
        )
    }
}
function MapStateToProps(state){
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        orgParamSearchResult:state.orgparam.orgParamSearchResult
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(OrgParamSet);