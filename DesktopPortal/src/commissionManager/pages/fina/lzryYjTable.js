//离职人员业绩确认表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchLzryyjqrb} from '../../actions/actionCreator'

class LZRYTJTable extends Component{
    appTableColumns = [
        { title: '员工编号', dataIndex: 'passDate', key: 'passDate' },
        { title: '员工姓名', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '归属组织', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '成交报告编号', dataIndex: 'dealType', key: 'dealType' },
        { title: '业绩产生人', dataIndex: 'wyName', key: 'wyName' },
        { title: '成交日期', dataIndex: '1', key: '1' },
        { title: '上业绩日期', dataIndex: '2', key: '2' },
        { title: '离职日期', dataIndex: '3', key: '3' },
        { title: '业绩金额', dataIndex: '4', key: '4' },
        { title: '是否包含', dataIndex: '5', key: '5' },

    ];
    state = {
        isDataLoading:false,
        pagination: {},
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchLzryyjqrb(e))
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = this.props.SearchCondition;
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(this.state.type);
    };
    componentWillReceiveProps(newProps){
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
    render(){
        return (
            <Layout>
                <Layout.Content>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <SearchCondition handleSearch={this.handleSearch}/>
                    </Col>
                </Row>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.props.dataSource} pagination={this.state.pagination} onChange={this.handleTableChange}></Table> 
                    </Spin>
                    </Col>
                </Row> 
                </Layout.Content>
            </Layout>
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
export default connect(MapStateToProps, MapDispatchToProps)(LZRYTJTable);