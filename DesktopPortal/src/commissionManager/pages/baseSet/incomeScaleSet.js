//提成比例设置
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Layout, Table, Button, Checkbox, Popconfirm, Tooltip, Row, Col, Input, Spin, Select, TreeSelect } from 'antd'
import { incomeScaleAdd,incomeScaleDlgClose, incomeScaleEdit, incomeScaleDel, incomeScaleListGet, orgGetPermissionTree, getDicParList } from '../../actions/actionCreator'
import SearchCondition from '../../constants/searchCondition'
import InComeScaleEditor from './incomeScaleEditor'
import {keepTwoDecimalFull} from '../../constants/utils'

const Option = Select.Option;

class InComeScaleSet extends Component {
    state = {
        pagination: {},
        isDataLoading: false,
        params: { branchName: "测试", codeName: "测试", branchId: '', code: '' },
        requirePermission:['YJ_SZ_TCBLSZ']
    }
    appTableColumns = [
        { title: '组织', dataIndex: 'branchName', key: 'branchName' },
        { title: '职位类别', dataIndex: 'codeName', key: 'codeName' },
        { title: '起始业绩', dataIndex: 'startYj', key: 'startYj' },
        { title: '结束业绩', dataIndex: 'endYj', key: 'endYj' },
        { title: '提成比例', dataIndex: 'percent', key: 'percent' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                this.hasPermission(this.state.requirePermission)?
                <span>
                    <Popconfirm title="是否删除该项?" onConfirm={(e) => this.zfconfirm(recored)} onCancel={this.zfcancel} okText="确认" cancelText="取消">
                        <Button type='primary' shape='circle' size='small' icon='delete' />
                    </Popconfirm>
                    <Tooltip title='修改'>
                        <Button type='primary' shape='circle' size='small' icon='edit' onClick={(e) => this.handleModClick(recored)} />
                    </Tooltip>
                </span>
                :null
            )
        }
    ];
    zfconfirm = (e) => {
        this.handleDelClick(e)
    }
    zfcancel = (e) => {

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
    componentWillMount = () => {
    }
    componentDidMount = () => {
        this.setState({ isDataLoading: true })
        this.props.dispatch(orgGetPermissionTree("YJ_TCBLSZ"));
        this.props.dispatch(getDicParList(['POSITION_TYPE']));
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

        if (newProps.operInfo.operType === 'YJ_TCBLSZ') {
            console.log('YJ_TCBLSZ')
            let params = { ...this.state.params }
            params.branchId = newProps.permissionOrgTree.BaseSetOrgTree[0].key
            params.branchName = newProps.permissionOrgTree.BaseSetOrgTree[0].label
            this.setState({ params }, () => {
                if (this.state.params.branchId !== '' && this.state.params.code !== '') {
                    this.setState({ isDataLoading: true })
                    this.handleSearch()
                }
            })
            newProps.operInfo.operType = ''
        }
        if (newProps.basicOper.operType === 'DIC_GET_PARLIST_COMPLETE') {
            let params = { ...this.state.params }
            params.code = newProps.basicData.zwTypes[0].value
            params.codeName = newProps.basicData.zwTypes[0].key
            this.setState({ params }, () => {
                if (this.state.params.branchId !== '' && this.state.params.code !== '') {
                    this.setState({ isDataLoading: true })
                    this.handleSearch()
                }
            })
            newProps.basicOper.operType = ''
        }
        if (newProps.incomeOp.operType === 'INCOME_SCALE_DEL_UPDATE'||
            newProps.incomeOp.operType === 'INCOME_SCALE_SAVE_SUCCESS') {
            this.props.dispatch(incomeScaleDlgClose())
            this.handleSearch()
            newProps.incomeOp.operType = ''
        }
    }
    treeChange = (value, label, extra) => {
        let params = { ...this.state.params }
        params.branchId = value
        params.branchName = label
        this.setState({ params })
    }
    zwlbChange = (value, option) => {
        let params = { ...this.state.params }
        params.code = value
        params.codeName = this.getLevel(value)
        this.setState({ params })
    }
    getListData = () => {
        if (this.props.scaleSearchResult.extension == null) {
            return null
        }
        let data = this.props.scaleSearchResult.extension;

        if (this.props.incomeOp.operType !== 'INCOME_SCALE_LIST_UPDATE') {
            return data
        }

        this.props.incomeOp.operType = ''

        for (let i = 0; i < data.length; i++) {
            let percent = keepTwoDecimalFull(data[i].percent*100)
            data[i].percent = percent + '%'
        }
        return data
    }
    getOrgName = (branchId) => {
        return '测试'
    }
    getLevel = (code) => {
        let temps = this.props.basicData.zwTypes
        for(let i=0;i<temps.length;i++){
            if(temps[i].value === code){
                return temps[i].key
            }
        }
    }
    //是否有权限
    hasPermission = (requirePermission) => {
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
        let zwTypes = this.props.basicData.zwTypes;

        return (
            <Layout>
                <div style={{ 'margin': 5 }}>
                    组织：
                    <TreeSelect style={{ width: 300 }}
                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                        treeData={this.props.permissionOrgTree.BaseSetOrgTree}
                        placeholder="所属组织"
                        value={this.state.params.branchId}
                        onChange={this.treeChange}
                    >
                    </TreeSelect>
                    <span style={{ 'margin-left': 10 }}>职位类别：</span>
                    <Select value={this.state.params.code} style={{ width: 120 }} onChange={this.zwlbChange}>
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
                    <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.getListData()} onChange={this.handleTableChange}></Table>

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
        basicOper: state.base.operInfo,
        incomeOp: state.scale.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(InComeScaleSet);