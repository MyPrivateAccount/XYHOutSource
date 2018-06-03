//人员分摊表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchPPFt} from '../../actions/actionCreator'

class PPFtTable extends Component{

    state = {
        isDataLoading:false,
        pagination: {},
    }
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '直接人数', dataIndex: 'dealType', key: 'dealType' },
        { title: '分摊入人数', dataIndex: 'wyName', key: 'wyName' },
        { title: '总人数', dataIndex: 'wyAddress', key: 'wyAddress' },
    ];
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchPPFt(e))
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
export default connect(MapStateToProps, MapDispatchToProps)(PPFtTable);