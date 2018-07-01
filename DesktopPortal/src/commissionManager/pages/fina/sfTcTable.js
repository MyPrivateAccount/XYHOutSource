//实发提成表
import { connect } from 'react-redux';
import React, { Component } from 'react';
import { Table, Button,Tooltip, Layout, Row, Col,Spin} from 'antd'
import SearchCondition from './searchCondition';
import {searchSftcb} from '../../actions/actionCreator'

class SFTcTable extends Component{
    appTableColumns = [
        { title: '事业部', dataIndex: 'branchLevel1Name', key: 'branchLevel1Name' },
        { title: '片区', dataIndex: 'branchLevel2Name', key: 'branchLevel2Name' },
        { title: '小组', dataIndex: 'branchLevel3Name', key: 'branchLevel3Name' },
        { title: '用户', dataIndex: 'userInfo.trueName', key: 'userInfo.trueName' },
        { title: '职别', dataIndex: 'userInfo.positionName', key: 'userInfo.positionName' },
        { title: '本月实收业绩', dataIndex: 'byTc', key: 'byTc' },
        { title: '本月实收业绩提成', dataIndex: 'byTc', key: 'byTc' },
        { title: '待扣追佣金额', dataIndex: 'byKjJe', key: 'byKjJe' },
        { title: '本月实发', dataIndex: 'bySf', key: 'bySf' },
        {
            title: '操作', dataIndex: 'edit', key: 'edit', render: (text, recored) => (
                <span>
                    <Tooltip title="分月查看">
                        <Button type='primary' shape='circle' size='small' icon='edit'/>
                    </Tooltip>
                </span>
            )
        }

    ];
    state = {
        isDataLoading:false,
        pagination: {},
        searchCondition:{}
    }
    handleSearch = (e) => {
        this.setState({ isDataLoading: true,searchCondition:e});
        this.props.dispatch(searchSftcb(e))
    }
    handleTableChange = (pagination, filters, sorter) => {
        let cd = {...this.state.searchCondition};
        cd.pageIndex = (pagination.current - 1);
        cd.pageSize = pagination.pageSize;
        console.log("table改变，", pagination);
        this.setState({ isDataLoading: true });
        this.handleSearch(cd);
    };
    componentWillReceiveProps(newProps){
        console.log("new Props:" + newProps.dataSource)
        this.setState({ isDataLoading: false });
        if(newProps.dataSource){
            let paginationInfo = {
                pageSize: newProps.dataSource.pageSize,
                current: newProps.dataSource.pageIndex,
                total: newProps.dataSource.totalCount
            };
            console.log("分页信息：", paginationInfo);
            this.setState({ pagination: paginationInfo });
        }
    }
    render(){
        return (
            <Layout>
                <Layout.Content>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <SearchCondition handleSearch={this.handleSearch} orgPermission={'YJ_CW_SFTCB'}/>
                    </Col>
                </Row>
                <Row style={{margin:10}}>
                    <Col span={24}>
                    <Spin spinning={this.state.isDataLoading}>
                    <Table columns={this.appTableColumns} dataSource={this.props.dataSource.extension} pagination={this.state.pagination} onChange={this.handleTableChange}></Table> 
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
export default connect(MapStateToProps, MapDispatchToProps)(SFTcTable);