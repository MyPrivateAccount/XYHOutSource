//提成成本表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchTccbb} from '../../actions/actionCreator'

class TCCbTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'passDate', key: 'passDate' },
        { title: '片区', dataIndex: 'dealSN', key: 'dealSN' },
        { title: '小组', dataIndex: 'getFeeDate', key: 'getFeeDate' },
        { title: '直接成本', dataIndex: 'dealType', key: 'dealType' },
        { title: '分摊入成本', dataIndex: 'wyName', key: 'wyName' },
        { title: '成本', dataIndex: 'wyAddress', key: 'wyAddress' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="查看详情">
                        <Button type='primary' shape='circle' size='small' icon='edit'/>
                    </Tooltip>
                </span>
            )
        }

    ];
    state = {
        isDataLoading:false,
        pagination: {},
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true });
        this.props.dispatch(searchTccbb(e))
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
export default connect(MapStateToProps, MapDispatchToProps)(TCCbTable);