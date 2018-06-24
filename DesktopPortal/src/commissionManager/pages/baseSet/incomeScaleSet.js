//提成比例设置
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import { incomeScaleAdd, incomeScaleEdit, incomeScaleDel, incomeScaleListGet, orgGetPermissionTree,getDicParList } from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import InComeScaleEditor from './incomeScaleEditor'

const Option = Select.Option;

class InComeScaleSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        params: { name: "测试", jobType: "测试" ,branchId:'',code:''}
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'name', key: 'name' },
        { title: '职位类别', dataIndex: 'jobType', key: 'jobType' },
        { title: '起始业绩', dataIndex: 'startYj', key: 'startYj' },
        { title: '结束业绩', dataIndex: 'endYj', key: 'endYj' },
        { title: '提成比例', dataIndex: 'percent', key: 'percent' },
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
    zfconfirm=(e)=>{
        this.handleDelClick(e)
    }
    zfcancel=(e)=>{

    }
    handleDelClick = (info) => {
        this.props.dispatch(incomeScaleDel(info));
    }
    handleModClick = (info) => {
        this.props.dispatch(incomeScaleEdit(info));
    }
    handleNew = (info) => {
        this.props.dispatch(incomeScaleAdd(this.state.params));
    }
    handleSearch = (e) => {
        console.log(e)
        SearchCondition.incomeScaleListCondition.branchId = this.state.params.branchId;
        SearchCondition.incomeScaleListCondition.code = this.state.params.code;
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
    componentWillMount=()=>{
    }
    componentDidMount = () => {
        this.setState({ isDataLoading: true })
        this.props.dispatch(orgGetPermissionTree("UserInfoCreate"));
        this.props.dispatch(getDicParList(['COMMISSION_ZW_LEVEL']));
    }
    componentWillReceiveProps = (newProps) => {
        this.setState({ isDataLoading: false });
        let paginationInfo = {
            pageSize: newProps.scaleSearchResult.pageSize,
            current: newProps.scaleSearchResult.pageIndex,
            total: newProps.scaleSearchResult.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });

        if (newProps.operInfo.operType === 'org_update') {
            console.log('org_update')
            let params = { ...this.state.params }
            params.branchId = newProps.permissionOrgTree.AddUserTree[0].key
            this.setState({ params },()=>{
                if(this.state.params.branchId!==''&&this.state.params.code!==''){
                    this.setState({isDataLoading:true})
                    this.handleSearch()
                }
            })
            newProps.operInfo.operType = ''
        }
        if(newProps.basicOper.operType === 'DIC_GET_PARLIST_COMPLETE'){
            let params = { ...this.state.params }
            params.code = newProps.basicData.zwTypes[0].value
            this.setState({ params },()=>{
                if(this.state.params.branchId!==''&&this.state.params.code!==''){
                    this.setState({isDataLoading:true})
                    this.handleSearch()
                }
            })
            newProps.basicOper.operType = ''
        }
    }
    treeChange = (e) => {
        let params = { ...this.state.params }
        params.branchId = e
        this.setState({ params })
    }
    zwlbChange = (e) => {
        let params = { ...this.state.params }
        params.code = e
        this.setState({ params })
    }
    getListData=()=>{
        if(this.props.scaleSearchResult.extension == null){
            return null
        }
        let data = this.props.scaleSearchResult.extension;
            for(let i=0;i<data.length;i++){
                data[i].name = this.getOrgName(data[i].branchId)
                data[i].jobType = this.getLevel(data[i].code)
            }
        return data
    }
    getOrgName=(branchId)=>{
        return '测试'
    }
    getLevel=(code)=>{
        return '测试'
    }
    render() {
        let zwTypes = this.props.basicData.zwTypes;

        return (
            <Layout>
                <div style={{ 'margin': 5 }}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                        treeData={this.props.permissionOrgTree.AddUserTree}
                        placeholder="所属组织"
                        value={this.state.params.branchId}
                        onChange={this.treeChange}
                    >
                    </TreeSelect>
                    <span style={{ 'margin-left': 10 }}>职位类别：</span>
                    <Select value={this.state.params.code}  style={{ width: 120 }} onChange={this.zwlbChange}>
                    {
                        zwTypes.map(tp => <Option key={tp.key} value={tp.value}>{tp.key}</Option>)
                    }
                    </Select>
                    <Button style={{ 'width': 80, 'margin-left': 10 }} onClick={this.handleSearch.bind(this)}>查询</Button>
                </div>
                <Tooltip title="新增">
                    <Button type='primary' shape='circle' icon='plus' onClick={this.handleNew} style={{ 'margin': '10' }} />
                </Tooltip>
                <Spin spinning={this.state.isDataLoading}>
                    <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.scaleSearchResult.extension} onChange={this.handleTableChange}></Table>

                    <InComeScaleEditor />
                </Spin>
            </Layout>
        )
    }
}
function MapStateToProps(state) {
    return {
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        scaleSearchResult: state.scale.scaleSearchResult,
        operInfo: state.org.operInfo,
        basicData: state.base,
        basicOper:state.base.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(InComeScaleSet);