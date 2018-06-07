//
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Spin } from 'antd'
import {searchYjtzhz } from '../../actions/actionCreator'

class YjtzhzTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dataSource: [],
            pagination: {},
            isDataLoading: false
        }
    }
    appTableColumns = [
        { title: '序号', dataIndex: '1', key: '1' },
        { title: '成交编号', dataIndex: '2', key: '2' },
        { title: '调整日期', dataIndex: '3', key: '3' },
        { title: '业主调整金额', dataIndex: '4', key: '4' },
        { title: '客户调整金额', dataIndex: '5', key: '5' },
        { title: '合计', dataIndex: '6', key: '6' },
        { title: '调整人', dataIndex: '7', key: '7' },
        { title: '原因', dataIndex: '8', key: '8' }
    ];
    handleSearch = (e,type) => {
        console.log(e)
        console.log("查询条件", e);
        this.setState({type:type})
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchYjtzhz(e));
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
export default connect(MapStateToProps, MapDispatchToProps)(YjtzhzTable);