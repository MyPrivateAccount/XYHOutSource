//提成比例设置
import { connect } from 'react-redux';
import React,{Component} from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import {incomeScaleAdd,incomeScaleEdit,incomeScaleDel,incomeScaleListGet,orgGetPermissionTree} from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import InComeScaleEditor from './incomeScaleEditor'

const Option = Select.Option;

class InComeScaleSet extends Component{
    state = {
        pagination: {},
        isDataLoading:false,
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'orgName', key: 'orgName' },
        { title: '职位类别', dataIndex: 'jobType', key: 'jobType' },
        { title: '起始业绩', dataIndex: 'stIncome', key: 'stIncome' },
        { title: '结束业绩', dataIndex: 'endIncome', key: 'endIncome' },
        { title: '提成比例', dataIndex: 'incomeScale', key: 'incomeScale' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>

                    <Tooltip title='修改'>
                        &nbsp;<Button type='primary' shape='circle' size='small' icon='team' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                    <Tooltip title="删除">
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleDelClick(recored)} />
                    </Tooltip>
                </span>
            )
        }
    ];
    handleDelClick = (info) =>{
        this.props.dispatch(incomeScaleDel(info));
    }
    handleModClick = (info) =>{
        this.props.dispatch(incomeScaleEdit(info));
    }
    handleNew = (info)=>{
        this.props.dispatch(incomeScaleAdd());
    }
    handleSearch = (e) => {
        console.log(e)
        SearchCondition.incomeScaleListCondition.pageIndex = 0;
        SearchCondition.incomeScaleListCondition.pageSize = 10;
        SearchCondition.incomeScaleListCondition.OrganizationId = '';
        SearchCondition.incomeScaleListCondition.rankPos = '';
        console.log("查询条件", SearchCondition.incomeScaleListCondition);
        this.setState({ isDataLoading: true });
        this.props.dispatch(incomeScaleListGet(SearchCondition.incomeScaleListCondition));
    }
    handleTableChange = (pagination, filters, sorter) => {
        SearchCondition.incomeScaleListCondition.pageIndex = (pagination.current - 1);
        SearchCondition.incomeScaleListCondition.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.props.dispatch(incomeScaleListGet(SearchCondition.incomeScaleListCondition));
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
                                    >
                    </TreeSelect>
                    <span style={{'margin-left':10}}>职位类别：</span>
                    <Select defaultValue="lucy" style={{ width: 120 }}>
                        <Option value="jack">Jack</Option>
                        <Option value="lucy">Lucy</Option>
                        <Option value="disabled" disabled>Disabled</Option>
                        <Option value="Yiminghe">yiminghe</Option>
                    </Select>
                    <Button style={{'width':80,'margin-left':10}} onClick={this.handleSearch}>查询</Button>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{'margin':'10'}}/>
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                 <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.scaleSearchResult.ext} onChange={this.handleTableChange}></Table>
                 </Spin>
                 <InComeScaleEditor/>
            </Layout>
        )
    }
}
function MapStateToProps(state){
    return {
        activeTreeNode:    state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        scaleSearchResult: state.scale.scaleSearchResult
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(InComeScaleSet);