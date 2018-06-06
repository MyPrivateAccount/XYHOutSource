//应发提成表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchYftcb} from '../../actions/actionCreator'

class YFtcTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: '1', key: '1' },
        { title: '片区', dataIndex: '2', key: '2' },
        { title: '小组', dataIndex: '3', key: '3' },
        { title: '用户', dataIndex: '4', key: '4' },
        { title: '职别', dataIndex: '5', key: '5' },
        { title: '分配业绩', dataIndex: '6', key: '6' },
        { title: '待扣坏佣业绩', dataIndex: '7', key: '7' },
        { title: '实扣坏佣业绩', dataIndex: '8', key: '8' },
        { title: '净业绩', dataIndex: '9', key: '9' },
        { title: '人数', dataIndex: '10', key: '10' },
        { title: '人均业绩', dataIndex: '11', key: '11' },
        { title: '提成比例', dataIndex: '12', key: '12' },
        { title: '本月应发提成', dataIndex: '13', key: '13' },
    ];
    state = {
        isDataLoading:false,
        pagination: {},
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchYftcb(e))
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
export default connect(MapStateToProps, MapDispatchToProps)(YFtcTable);