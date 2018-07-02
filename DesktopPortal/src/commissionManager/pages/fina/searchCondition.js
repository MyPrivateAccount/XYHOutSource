//查询条件组件
import React, { Component } from 'react'
import { Layout, Row, Col, Button, TreeSelect, DatePicker,Spin } from 'antd';
import { connect } from 'react-redux';
import { orgGetPermissionTree } from '../../actions/actionCreator'
import moment from 'moment'

const { MonthPicker} = DatePicker;

class SearchCondition extends Component {

    state = {
        branchId:'',
        isDataLoading:false
    }
    handleChangeTime = (e, field) => {
        if (field === 'yjMonth') {
            this.props.searchCondition.yyyymm = moment(e).format('YYYYMM')
        }
    }
    handleSelect = (e, field) => {
        this.props.searchCondition.branchId = e
    }
    handleSearch = (e) => {
        this.props.searchCondition.pageSize = 10
        this.props.searchCondition.pageIndex = 0
        this.props.handleSearch(this.props.searchCondition)
    }
    componentWillReceiveProps(newProps){
        this.setState({isDataLoading:false})
        if(newProps.operInfo.operType === 'YJ_CW_RY_QUERY'||
        newProps.operInfo.operType === 'YJ_CW_YFTCB'||
        newProps.operInfo.operType === 'YJ_CW_SFTCB'||
        newProps.operInfo.operType === 'YJ_CW_TCCBB'||
        newProps.operInfo.operType === 'YJ_CW_YFTCCJB'||
        newProps.operInfo.operType === 'YJ_CW_LZRYYJQRB'||
        newProps.operInfo.operType === 'YJ_CW_SFKJQRB'){
            this.setState({branchId:newProps.permissionOrgTree.FinaOrgTree[0].key})
            this.props.searchCondition.branchId = newProps.permissionOrgTree.FinaOrgTree[0].key
            this.props.searchCondition.yyyymm = moment().format('YYYYMM')
            this.handleSearch()
            newProps.operInfo.operType = ''
        }
    }
    componentDidMount() {
        if (this.props.orgPermission) {
            this.setState({isDataLoading:true})
            this.props.dispatch(orgGetPermissionTree(this.props.orgPermission));
        }
    }
    render() {
        return (
            <Layout>
                <Layout.Content>
                    <Spin spinning={this.state.isDataLoading}>
                    <Row>
                        <Col span={24}>
                            <label style={{ margin: 10 }}>
                                <span>分公司</span>
                                <TreeSelect style={{ width: 300 }}
                                    dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                                    treeData={this.props.permissionOrgTree.FinaOrgTree}
                                    placeholder="分公司"
                                    value = {this.state.branchId}
                                    onChange={(e) => this.handleSelect(e, 'organizationId')}>
                                </TreeSelect>
                            </label>
                            <label style={{ margin: 10 }}>
                                <span>月结月份</span>
                                <MonthPicker defaultValue={moment()} format={'YYYY/MM'} style={{ width: 100 }} onChange={(e, dateString) => this.handleChangeTime(dateString, 'yjMonth')} />
                            </label>
                            <Button type="primiary" onClick={this.handleSearch}>查询</Button>
                        </Col>
                    </Row>
                    </Spin>
                </Layout.Content>
            </Layout>
        )
    }
}
function MapStateToProps(state) {

    return {
        basicData: state.base,
        activeTreeNode: state.org.activeTreeNode,
        permissionOrgTree: state.org.permissionOrgTree,
        searchCondition:state.fina.searchCondition,
        operInfo:state.org.operInfo
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(SearchCondition);