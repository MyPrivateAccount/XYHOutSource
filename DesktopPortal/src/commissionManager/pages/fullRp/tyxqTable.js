//
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Spin } from 'antd'
import {searchTyxq } from '../../actions/actionCreator'

class TyxqTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            pagination: {},
            isDataLoading: false
        }
    }
    appTableColumns = [
        { title: '申请日期', dataIndex: '1', key: '1' },
        { title: '调佣上报日期', dataIndex: '2', key: '2' },
        { title: '成交报告编号', dataIndex: '3', key: '3' },
        { title: '交易类型', dataIndex: '4', key: '4' },
        { title: '所属部门', dataIndex: '5', key: '5' },
        { title: '成交人', dataIndex: '6', key: '6' },
        { title: '成交金额', dataIndex: '7', key: '7' },
        { title: '当前状态', dataIndex: '8', key: '8' },
        { title: '当前处理步骤', dataIndex: '9', key: '9' },
        { title: '流程审批人', dataIndex: '10', key: '10' },
        { title: '审批记录', dataIndex: '11', key: '11' },
        { title: '操作', dataIndex: '12', key: '12' },
    ];
    handleSearch = (e,type) => {
        console.log(e)
        console.log("查询条件", e);
        this.setState({type:type})
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchTyxq(e));
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = this.props.SearchCondition;
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(this.state.type);
    };
    componentDidMount = () => {
        this.props.onRpTable(this)
    }
    componentWillReceiveProps = (newProps) => {
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });

        let paginationInfo = {
            pageSize: newProps.dataSource.pageSize,
            current: newProps.dataSource.pageIndex,
            total: newProps.dataSource.totalCount
        };
        console.log("分页信息：", paginationInfo);
        this.setState({ pagination: paginationInfo });

    }
    render() {
        return (
            <Spin spinning={this.state.isDataLoading}>
                <Table pagination={this.state.pagination} columns={this.appTableColumns} dataSource={this.props.dataSource} onChange={this.handleTableChange}></Table>
            </Spin>
        )
    }
}
function MapStateToProps(state) {

    return {
        dataSource:state.fina.dataSource,
        searchCondition:state.fina.SearchCondition
    }
}

function MapDispatchToProps(dispatch) {
    return {
        dispatch
    };
}
export default connect(MapStateToProps, MapDispatchToProps)(TyxqTable);